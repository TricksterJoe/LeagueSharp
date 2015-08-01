using Color = System.Drawing.Color;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    internal class MenuManager
    {
        #region Variable Declaration
        public const string Menuname = "Slutty Ryze";

        public static Orbwalking.Orbwalker Orbwalker;
        private static Menu _config;
        #endregion
        #region Public Functions
        public static Menu GetMenu()
        {
            _config = new Menu(Menuname, Menuname, true);
            _config.AddSubMenu(HumanizerMenu());
            _config.AddSubMenu(DrawingMenu());
            _config.AddSubMenu(ComboMenu());
            _config.AddSubMenu(MixedMenu());
            _config.AddSubMenu(FarmMenu());
            _config.AddSubMenu(MiscMenu());
            _config.AddSubMenu(OrbWalkingMenu());
            Orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));
            return _config;
        }
        #endregion
        #region Private Functions

        private static Menu HumanizerMenu()
        {
            var humanizerMenu = new Menu("Humanizer", "Humanizer");

            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("minDelay", "Minimum delay for actions (ms)").SetValue(new Slider(10, 0, 200)));
            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("maxDelay", "Maximum delay for actions (ms)").SetValue(new Slider(75, 0, 250)));
            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("doHuman", "Humanize").SetValue(true));

            return humanizerMenu;
        }

        private static Menu OrbWalkingMenu()
        {
            Menu orbWalkingMenu = new Menu("Orbwalking", "Orbwalking");
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            orbWalkingMenu.AddSubMenu(targetSelectorMenu);
            return orbWalkingMenu;
        }
        private static Menu DrawingMenu()
        {
            var drawMenu = new Menu("Drawing Settings", "Drawings");
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("Draw", "Display Drawing").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("wDraw", "w Drawing").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("notdraw", "Float Text").SetValue(true));

           // drawMenu.SubMenu("Drawings").AddItem(new MenuItem("keyBindDisplay", "Display KeyBinds").SetValue(false));

            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo damage").SetValue(true);
            var drawFill =
                new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            drawMenu.SubMenu("Drawings").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Drawings").AddItem(drawFill);

            GlobalManager.EnableDrawingDamage = drawDamageMenu.GetValue<bool>();
            GlobalManager.EnableFillDamage = drawFill.GetValue<Circle>().Active;
            GlobalManager.DamageFillColor = drawFill.GetValue<Circle>().Color;

            return drawMenu;
        }

        private static Menu ComboMenu()
        {
            var combo1Menu = new Menu("Combo Settings (SB)", "combospells");
            {
                combo1Menu
                    .AddItem(
                        new MenuItem("combooptions", "Combo Mode").SetValue(
                            new StringList(new[] {"Improved Combo System"})));
                combo1Menu.AddItem(new MenuItem("useQ", "Use Q (Over Load)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useW", "Use W (Rune Prison)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useE", "Use E (Spell Flux)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useR", "Use R (Desperate Power)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useRww", "Only R if Target Is Rooted").SetValue(true));
                combo1Menu.AddItem(new MenuItem("AAblock", "Block auto attack in combo").SetValue(false));
            }
            return combo1Menu;
        }

        private static Menu MixedMenu()
        {

            var mixedMenu = new Menu("Mixed Settings (C)", "mixedsettings");
            {
                mixedMenu.AddItem(new MenuItem("mMin", "Minimum Mana For Spells").SetValue(new Slider(40)));
                mixedMenu.AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));
                mixedMenu.AddItem(new MenuItem("UseQMl", "Use Q last hit minion").SetValue(true));
                mixedMenu.AddItem(new MenuItem("UseEM", "Use E").SetValue(false));
                mixedMenu.AddItem(new MenuItem("UseWM", "Use W").SetValue(false));
                mixedMenu.AddItem(new MenuItem("UseQauto", "Auto Q").SetValue(false));
            }
            return mixedMenu;
        }

        private static Menu FarmMenu()
        {
            var farmMenu = new Menu("Farming Settings", "farmingsettings");
            var laneMenu = new Menu("Lane Clear (V)", "lanesettings");
            {
                laneMenu.AddItem(
                    new MenuItem("disablelane", "Lane Clear Toggle").SetValue(new KeyBind('T', KeyBindType.Toggle)));
                laneMenu.AddItem(
                    new MenuItem("presslane", "Press Lane Clear").SetValue(new KeyBind('H', KeyBindType.Press)));
                laneMenu.AddItem(new MenuItem("useEPL", "Minimum %Mana For Lane Clear").SetValue(new Slider(50)));
                laneMenu.AddItem(new MenuItem("passiveproc", "Don't Use Spells If Passive Will Proc").SetValue(true));
                laneMenu.AddItem(new MenuItem("useQlc", "Use Q Last Hit").SetValue(true));
                laneMenu.AddItem(new MenuItem("useWlc", "Use W Last Hit").SetValue(false));
                laneMenu.AddItem(new MenuItem("useElc", "Use E Last Hit").SetValue(false));
                laneMenu.AddItem(new MenuItem("useQ2L", "Use Q To Lane Clear").SetValue(true));
                laneMenu.AddItem(new MenuItem("useW2L", "Use W To Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("useE2L", "Use E To Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("useRl", "Use R In Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("rMin", "Minimum Minions For R").SetValue(new Slider(3, 1, 20)));
            }

            var jungleMenu = new Menu("Jungle Settings (V)", "junglesettings");
            {
                jungleMenu.AddItem(new MenuItem("useJM", "Minimum Mana For Jungle Clear").SetValue(new Slider(50)));
                jungleMenu.AddItem(new MenuItem("useQj", "Use Q").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useWj", "Use W").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useEj", "Use E").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useRj", "Use R").SetValue(true));
            }


            var lastMenu = new Menu("Last Hit Settings (X)", "lastsettings");
            {
                lastMenu.AddItem(new MenuItem("useQl2h", "Use Q Last Hit").SetValue(true));
                lastMenu.AddItem(new MenuItem("useWl2h", "Use W Last Hit").SetValue(false));
                lastMenu.AddItem(new MenuItem("useEl2h", "Use E Last Hit").SetValue(false));
            }

            farmMenu.AddSubMenu(laneMenu);
            farmMenu.AddSubMenu(jungleMenu);
            farmMenu.AddSubMenu(lastMenu);
            return farmMenu;
        }

        private static Menu MiscMenu()
        {
            var miscMenu = new Menu("Miscellaneous (Background)", "miscsettings");

            var passiveMenu = new Menu("Auto Passive", "passivesettings");
            {
                passiveMenu.AddItem(new MenuItem("ManapSlider", "Minimum %Mana"))
                    .SetValue(new Slider(30));
                passiveMenu.AddItem(
                    new MenuItem("autoPassive", "Stack Passive").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
                passiveMenu.AddItem(new MenuItem("stackSlider", "Keep passive count at"))
                    .SetValue(new Slider(3, 1, 4));
                passiveMenu.AddItem(new MenuItem("autoPassiveTimer", "Refresh passive ever"))
                    .SetValue(new Slider(5, 1, 10));
                passiveMenu.AddItem(new MenuItem("stackMana", "Minimum %Mana")).SetValue(new Slider(50));
            }

            var itemMenu = new Menu("Items", "itemsettings");
            {
                itemMenu.AddItem(new MenuItem("tearS", "Stack Tear").SetValue(new KeyBind('G', KeyBindType.Toggle)));
                itemMenu.AddItem(new MenuItem("tearoptions", "Stack Tear Only in Fountain").SetValue(false));
                itemMenu.AddItem(new MenuItem("tearSM", "Min Mana").SetValue(new Slider(95)));
                itemMenu.AddItem(new MenuItem("staff", "Use Seraphs Embrace").SetValue(true));
                itemMenu.AddItem(new MenuItem("staffhp", "Seraph's when %HP >").SetValue(new Slider(30)));
                itemMenu.AddItem(new MenuItem("muramana", "Muramana").SetValue(true));
            }

            var hpMenu = new Menu("Auto Potions", "hpsettings");
            {
                hpMenu.AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
                hpMenu.AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
                hpMenu.AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("MANA", "Auto Mana Potion").SetValue(true));
                hpMenu.AddItem(new MenuItem("MANASlider", "Minimum %Mana for Potion")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
                hpMenu.AddItem(new MenuItem("bSlider", "Minimum %Health for Biscuit")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("flask", "Auto Flask").SetValue(true));
                hpMenu.AddItem(new MenuItem("fSlider", "Minimum %Health for flask")).SetValue(new Slider(30));
            }

            var eventMenu = new Menu("Events", "eventssettings");
            {
                eventMenu.AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));
                eventMenu.AddItem(new MenuItem("useQW2D", "W/Q On Dashing").SetValue(true));
                eventMenu.AddItem(new MenuItem("level", "Auto Skill Level Up").SetValue(true));
                eventMenu.AddItem(new MenuItem("autow", "Auto W enemy under turret").SetValue(true));
            }

            var ksMenu = new Menu("Kill Steal", "kssettings");
            {
                ksMenu.AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
                ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
                ksMenu.AddItem(new MenuItem("useW2KS", "Use W for ks").SetValue(true));
                ksMenu.AddItem(new MenuItem("useE2KS", "Use E for ks").SetValue(true));
            }

            miscMenu.AddSubMenu(passiveMenu);
            miscMenu.AddSubMenu(itemMenu);
            miscMenu.AddSubMenu(hpMenu);
            miscMenu.AddSubMenu(eventMenu);
            miscMenu.AddSubMenu(ksMenu);
            return miscMenu;
        }
        #endregion
    }

}
