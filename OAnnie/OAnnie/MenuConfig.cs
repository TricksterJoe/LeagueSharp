using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace OAnnie
{
    internal class MenuConfig
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public const string Menuname = "Annie";

        public static void CreateMenu()
        {
            #region General

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var drawMenu = new Menu("Drawing Settings", "Drawings");
            {
                drawMenu.AddItem(new MenuItem("Draw", "Display Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("wDraw", "E Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("rfDraw", "Flash->R Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("FillDamage", "Show Combo Damage").SetValue(true));
                drawMenu.AddItem(
                    new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(true));
                Config.AddSubMenu(drawMenu);
            }

            #endregion

            #region combo

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("comboMenu.useignite", "Use Ignite")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.useq", "Use [Q]")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usew", "Use [W]")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usee", "Use [E]")).SetValue(true);
                var emenu = new Menu("[E] Settings", "[E] Settings");
                {
                    emenu.AddItem(new MenuItem("comboMenu.emenu.eaa", "[E] Against AA")).SetValue(true);
                    emenu.AddItem(new MenuItem("comboMenu.emenu.emode", "[E] Mode"))
                        .SetValue(new StringList(new[] {"E When Passive 3", "Always E"}, 1));
                }
                var rmenu = new Menu("[R] Settings", "[R] Settings");
                {
                    rmenu.AddItem(new MenuItem("comboMenu.user", "Use [R]")).SetValue(true);
                    rmenu.AddItem(new MenuItem("comboMenu.user.smart", "Smart [R] 1v1 Logic")).SetValue(true);
                    rmenu.AddItem(new MenuItem("comboMenu.user.Slider", "R If Hit X Enemies"))
                        .SetValue(new Slider(3, 1, 5));
                    comboMenu.AddSubMenu(rmenu);
                }
                var passivemanagement = new Menu("Passive Utillization", "passiveutil");
                {
                    passivemanagement.AddItem(new MenuItem("comboMenu.passivemanagement.e.before",
                        "Use E Before Q To Gain Stun")).SetValue(true);
                    passivemanagement.AddItem(new MenuItem("comboMenu.passivemanagement.e.stack", "Use E To Stack Stun"))
                        .SetValue(false);
                    passivemanagement.AddItem(new MenuItem("comboMenu.passivemanagement.w.stack", "Use W To Stack Stun"))
                        .SetValue(false);
                }
                comboMenu.AddSubMenu(passivemanagement);
                comboMenu.AddSubMenu(emenu);
                Config.AddSubMenu(comboMenu);
            }

            #endregion

            #region flash

            var flashmenu = new Menu("Flash Modes", "Flash Modes");
            {
                flashmenu.AddItem(new MenuItem("comboMenu.flashmenu.flashr", "Regular Flash R"))
                    .SetValue(new KeyBind(66, KeyBindType.Press));

                flashmenu.AddItem(new MenuItem("comboMenu.flashmenu.flasher", "Ninja Flash R"))
                    .SetValue(new KeyBind(68, KeyBindType.Press));
                Config.AddSubMenu(flashmenu);
            }

            #endregion

            #region tibbers

            var tibbersMenu = new Menu("Tibbers Settings", "Tibbers Settings");
            {
                tibbersMenu.AddItem(new MenuItem("tibbersMenu.move", "Dynamic Tibbers Movement")).SetValue(true);
                tibbersMenu
                    .AddItem(new MenuItem("tibmove", "Move Tibbers").SetValue(new KeyBind(65, KeyBindType.Press)));
                Config.AddSubMenu(tibbersMenu);
            }

            #endregion

            #region Kill Steal

            var killstealMenu = new Menu("Kill Steal", "Kill Steal");
            {
                killstealMenu.AddItem(new MenuItem("killstealMenu.ks", "Kill Steal")).SetValue(true);
                killstealMenu.AddItem(new MenuItem("killstealMenu.q", "Use [Q]")).SetValue(true);
                killstealMenu.AddItem(new MenuItem("killstealMenu.w", "Use [W]")).SetValue(true);
                killstealMenu.AddItem(new MenuItem("killstealMenu.r", "Use [R]")).SetValue(true);
                Config.AddSubMenu(killstealMenu);
            }

            #endregion

            #region clear

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                var laneMenu = new Menu("Lane Settings", "Lane Settings");
                {
                    laneMenu.AddItem(new MenuItem("clearMenu.laneMenu.keepstun", "Keep Stun")).SetValue(true);
                    laneMenu.AddItem(new MenuItem("clearMenu.laneMenu.manaslider", "> % Mana to Lane clear"))
                        .SetValue(new Slider(30));
                    laneMenu.AddItem(new MenuItem("clearMenu.laneMenu.useq", "Use [Q]")).SetValue(false);
                    laneMenu.AddItem(new MenuItem("clearMenu.laneMenu.useqlast", "Use [Q] To Last Hit")).SetValue(true);
                    laneMenu.AddItem(new MenuItem("clearMenu.laneMenu.usew", "Use [W]")).SetValue(false);
                    laneMenu.AddItem(new MenuItem("clearMenu.laneMenu.usewslider", "Use [W] If Hits X Enemies"))
                        .SetValue(new Slider(3, 1, 20));
                    clearMenu.AddSubMenu(laneMenu);
                }

                var jungleMenu = new Menu("Jungle Settings", "Jungle Settings");
                {
                    jungleMenu.AddItem(new MenuItem("clearMenu.jungleMenu.useq", "Use [Q]")).SetValue(true);
                    jungleMenu.AddItem(new MenuItem("clearMenu.jungleMenu.usee", "Use [E]")).SetValue(true);
                    jungleMenu.AddItem(new MenuItem("clearMenu.jungleMenu.usew", "Use [W]")).SetValue(true);
                    clearMenu.AddSubMenu(jungleMenu);
                }

                var lastMenu = new Menu("Last Hit Settings", "Last Hit Settings");
                {
                    laneMenu.AddItem(new MenuItem("clearMenu.lastMenu.keepstun", "Keep Stun")).SetValue(true);
                    lastMenu.AddItem(new MenuItem("clearMenu.lastMenu.useqlast", "Use [Q] To Last Hit")).SetValue(true);
                    clearMenu.AddSubMenu(lastMenu);
                }              
                Config.AddSubMenu(clearMenu);
            }

            #endregion

            #region misc

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            {
                miscMenu.AddItem(new MenuItem("miscMenu.qwdash", "[Q]/[W] Dash")).SetValue(true);
                miscMenu.AddItem(new MenuItem("miscMenu.qwgap", "[Q]/[W] Gap Closer")).SetValue(true);
                Config.AddSubMenu(miscMenu);
            }

            #endregion

            #region harras

            var harrasMenu = new Menu("Harras Settings", "Harras Settings");
            {
                harrasMenu.AddItem(new MenuItem("harrasMenu.useq", "Use [Q]")).SetValue(true);
                harrasMenu.AddItem(new MenuItem("harrasMenu.usew", "Use [W]")).SetValue(true);
                Config.AddSubMenu(harrasMenu);
            }

            #endregion
            
            Config.AddToMainMenu();
        }
    }
}
           