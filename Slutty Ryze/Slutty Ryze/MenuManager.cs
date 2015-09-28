    using System.Drawing;
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
            _config.AddItem(new MenuItem("test", "Level 3-5 Oriented Combo").SetValue(new KeyBind('Z', KeyBindType.Press)));
            Orbwalker = new Orbwalking.Orbwalker(_config.SubMenu("Orbwalking"));
            return _config;
        }
        #endregion
        #region Private Functions

        private static Menu HumanizerMenu()
        {
            var humanizerMenu = new Menu("Humanizer", "Humanizer");

            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("minDelay", "Minimum Delay for Actions (ms)").SetValue(new Slider(10, 0, 200)));
            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("maxDelay", "Maximum Delay for Actions (ms)").SetValue(new Slider(75, 0, 250)));
            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("minCreepHPOffset", "Minimum HP for a Minion to Have Before CSing Damage >= HP+(%)").SetValue(new Slider(5, 0, 25)));
            humanizerMenu.SubMenu("Humanizer").AddItem(new MenuItem("maxCreepHPOffset", "Maximum HP for a Minion to Have Before CSing Damage >= HP+(%)").SetValue(new Slider(15, 0, 25)));
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
            drawMenu
                .AddItem(
                    new MenuItem("drawoptions", "Drawing Mode Mode").SetValue(
                        new StringList(new[] { "Normal Mode", "Colorblind Mode" })));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("Draw", "Display Drawings").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Draw Q").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("eDraw", "Draw E").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("wDraw", "Draw W").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("notdraw", "Draw Floating Text").SetValue(true));
            drawMenu.SubMenu("Drawings").AddItem(new MenuItem("keyBindDisplay", "Display Keybinds").SetValue(true));  

            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo Damage").SetValue(true);
            var drawFill =
                new MenuItem("RushDrawWDamageFill", "Draw Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            drawMenu.SubMenu("Drawings").AddItem(drawDamageMenu);
            drawMenu.SubMenu("Drawings").AddItem(drawFill);

            //GlobalManager.EnableDrawingDamage = drawDamageMenu.GetValue<bool>();
            //GlobalManager.EnableFillDamage = drawFill.GetValue<Circle>().Active;
            //GlobalManager.DamageFillColor = drawFill.GetValue<Circle>().Color;

            return drawMenu;
        }

        private static Menu ComboMenu()
        {
            var combo1Menu = new Menu("Combo Settings", "combospells");
            {
                combo1Menu
                    .AddItem(
                        new MenuItem("combooptions", "Combo Mode").SetValue(
                            new StringList(new[] {"Improved Combo"})));
                combo1Menu.AddItem(new MenuItem("useQ", "Use Q (Overload)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useW", "Use W (Rune Prison)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useE", "Use E (Spell Flux)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useR", "Use R (Desperate Power)").SetValue(true));
                combo1Menu.AddItem(new MenuItem("useRww", "Only Use R if Target is Rooted").SetValue(true));
                combo1Menu.AddItem(new MenuItem("AAblock", "Block Auto Attack in Combo").SetValue(false));
            }
            return combo1Menu;
        }

        private static Menu MixedMenu()
        {

            var mixedMenu = new Menu("Mixed Settings", "mixedsettings");
            {
                mixedMenu.AddItem(new MenuItem("mMin", "Min. Mana for Spells").SetValue(new Slider(40)));
                mixedMenu.AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));
                mixedMenu.AddItem(new MenuItem("UseQMl", "Use Q to Last Hit Minions").SetValue(true));
                mixedMenu.AddItem(new MenuItem("UseEM", "Use E").SetValue(false));
                mixedMenu.AddItem(new MenuItem("UseWM", "Use W").SetValue(false));
                mixedMenu.AddItem(new MenuItem("UseQauto", "Auto Use Q").SetValue(false));
            }
            return mixedMenu;
        }

        private static Menu FarmMenu()
        {
            var farmMenu = new Menu("Farming Settings", "farmingsettings");
            var laneMenu = new Menu("Lane Clear", "lanesettings");
            {
                laneMenu.AddItem(
                    new MenuItem("disablelane", "Lane Clear Toggle").SetValue(new KeyBind('T', KeyBindType.Toggle)));
                laneMenu.AddItem(new MenuItem("useEPL", "Min. % Mana For Lane Clear").SetValue(new Slider(50)));
                laneMenu.AddItem(new MenuItem("passiveproc", "Don't Use Spells if Passive Will Proc").SetValue(true));
                laneMenu.AddItem(new MenuItem("useQlc", "Use Q to Last Hit").SetValue(true));
                laneMenu.AddItem(new MenuItem("useWlc", "Use W to Last Hit").SetValue(false));
                laneMenu.AddItem(new MenuItem("useElc", "Use E to Last Hit").SetValue(false));
                laneMenu.AddItem(new MenuItem("useQ2L", "Use Q to Lane Clear").SetValue(true));
                laneMenu.AddItem(new MenuItem("useW2L", "Use W to Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("useE2L", "Use E to Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("useRl", "Use R to Lane Clear").SetValue(false));
                laneMenu.AddItem(new MenuItem("rMin", "Min. Minions to Use R").SetValue(new Slider(3, 1, 20)));
            }

            var jungleMenu = new Menu("Jungle Settings", "junglesettings");
            {
                jungleMenu.AddItem(new MenuItem("useJM", "Min. % Mana for Jungle Clear").SetValue(new Slider(50)));
                jungleMenu.AddItem(new MenuItem("useQj", "Use Q").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useWj", "Use W").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useEj", "Use E").SetValue(true));
                jungleMenu.AddItem(new MenuItem("useRj", "Use R").SetValue(true));
            }


            var lastMenu = new Menu("Last Hit Settings", "lastsettings");
            {
                lastMenu.AddItem(new MenuItem("useQl2h", "Use Q to Last Hit").SetValue(true));
                lastMenu.AddItem(new MenuItem("useWl2h", "Use W to Last Hit").SetValue(false));
                lastMenu.AddItem(new MenuItem("useEl2h", "Use E to Last Hit").SetValue(false));
            }

            farmMenu.AddSubMenu(laneMenu);
            farmMenu.AddSubMenu(jungleMenu);
            farmMenu.AddSubMenu(lastMenu);
            return farmMenu;
        }

        private static Menu MiscMenu()
        {
            var miscMenu = new Menu("Miscellaneous", "miscsettings");

            var passiveMenu = new Menu("Auto Passive", "passivesettings");
            {
                passiveMenu.AddItem(new MenuItem("ManapSlider", "Min. % Mana"))
                    .SetValue(new Slider(30));
                passiveMenu.AddItem(
                    new MenuItem("autoPassive", "Stack Passive").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
                passiveMenu.AddItem(new MenuItem("stackSlider", "Keep Passive Count At"))
                    .SetValue(new Slider(3, 1, 4));
                passiveMenu.AddItem(new MenuItem("autoPassiveTimer", "Refresh Passive Every (s)"))
                    .SetValue(new Slider(5, 1, 10));
                passiveMenu.AddItem(new MenuItem("stackMana", "Min. % Mana")).SetValue(new Slider(50));
            }

            var itemMenu = new Menu("Items", "itemsettings");
            {
                itemMenu.AddItem(new MenuItem("tearS", "Auto Stack Tear").SetValue(new KeyBind('G', KeyBindType.Toggle)));
                itemMenu.AddItem(new MenuItem("tearoptions", "Stack Tear Only at Fountain").SetValue(false));
                itemMenu.AddItem(new MenuItem("tearSM", "Min % Mana to Stack Tear").SetValue(new Slider(95)));
                itemMenu.AddItem(new MenuItem("staff", "Use Seraph's Embrace").SetValue(true));
                itemMenu.AddItem(new MenuItem("staffhp", "Seraph's When % HP <").SetValue(new Slider(30)));
                itemMenu.AddItem(new MenuItem("muramana", "Use Muramana").SetValue(true));
            }

            var hpMenu = new Menu("Auto Potions", "hpsettings");
            {
                hpMenu.AddItem(new MenuItem("autoPO", "Enable Consumable Usage").SetValue(true));
                hpMenu.AddItem(new MenuItem("HP", "Auto Health Potions")).SetValue(true);
                hpMenu.AddItem(new MenuItem("HPSlider", "Min. % Health for Potion")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("MANA", "Auto Mana Potion").SetValue(true));
                hpMenu.AddItem(new MenuItem("MANASlider", "Min. % Mana for Potion")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
                hpMenu.AddItem(new MenuItem("bSlider", "Min. % Health for Biscuit")).SetValue(new Slider(30));
                hpMenu.AddItem(new MenuItem("flask", "Auto Flask").SetValue(true));
                hpMenu.AddItem(new MenuItem("fSlider", "Min. % Health for Flask")).SetValue(new Slider(30));
            }

            var eventMenu = new Menu("Events", "eventssettings");
            {
                eventMenu.AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));
                eventMenu.AddItem(new MenuItem("useQW2D", "W/Q on Dashing").SetValue(true));
                eventMenu.AddItem(new MenuItem("level", "Auto Level-Up").SetValue(true));
                eventMenu.AddItem(new MenuItem("autow", "Auto W Enemy Under Turret").SetValue(true));
            }

            var ksMenu = new Menu("Kill Steal", "kssettings");
            {
                ksMenu.AddItem(new MenuItem("KS", "Killsteal")).SetValue(true);
                ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q to KS").SetValue(true));
                ksMenu.AddItem(new MenuItem("useW2KS", "Use W to KS").SetValue(true));
                ksMenu.AddItem(new MenuItem("useE2KS", "Use E to KS").SetValue(true));
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
