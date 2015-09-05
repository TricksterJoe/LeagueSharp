using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

/*
 * Add Harras
 * Add Events
 * More options 
 * zzz
 */
namespace The_Slutty_Xerath
{
    internal class MenuConfigs
    {
        public static Menu Config;
        public static string Menuname = "The Slutty Xerath";

        public static void LoadMenu()
        {
            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Xerath.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));


            var drawMenu = new Menu("Drawing Settings", "Drawings");
            {
                drawMenu.AddItem(new MenuItem("Draw", "Display Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("wDraw", "W Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("FillDamage", "Show Combo Damage").SetValue(true));
                drawMenu.AddItem(
                    new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(true));
                Config.AddSubMenu(drawMenu);
            }

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("comboMenu.useq", "Use Q")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usew", "Use W")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usee", "Use E")).SetValue(true);
                var rmenu = new Menu("R Settings", "R Settings");
                {
                    rmenu.AddItem(new MenuItem("comboMenu.usertarget", "Ult Selected Target")).SetValue(true);
                    rmenu.AddItem(new MenuItem("comboMenu.usertap", "Use R")).SetValue(new KeyBind(67, KeyBindType.Press));
                    rmenu.AddItem(new MenuItem("comboMenu.user", "R Mode"))
                        .SetValue(new StringList(new[] { "Delayed", "Not Delayed" }, 1));
                    rmenu.AddItem(new MenuItem("comboMenu.userdelay", "R Delay")).SetValue(new Slider(500, 0, 2000));
                    rmenu.AddItem(new MenuItem("comboMenu.userblock", "Block Movement When R")).SetValue(true);
                    comboMenu.AddSubMenu(rmenu);
                }
                Config.AddSubMenu(comboMenu);
            }

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                var laneMenu = new Menu("Lane Clear", "Lane Clear");
                {
                    laneMenu.AddItem(new MenuItem("laneMenu.minmana", "Min Mana")).SetValue(new Slider(30));
                    laneMenu.AddItem(new MenuItem("laneMenu.useq", "Use Q")).SetValue(true);
                    laneMenu.AddItem(new MenuItem("laneMenu.useqhit", "Use Q if hits X")).SetValue(new Slider(3, 1, 20));
                    laneMenu.AddItem(new MenuItem("laneMenu.usew", "Use W")).SetValue(true);
                    laneMenu.AddItem(new MenuItem("laneMenu.usewhit", "Use W if hits X")).SetValue(new Slider(3, 1, 20));
                    clearMenu.AddSubMenu(laneMenu);
                }

                var jungleMenu = new Menu("Jungle Clear", "Jungle Clear");
                {
                    jungleMenu.AddItem(new MenuItem("jungleMenu.minmana", "Min Mana")).SetValue(new Slider(30));
                    jungleMenu.AddItem(new MenuItem("jungleMenu.useq", "Use Q")).SetValue(true);
                    jungleMenu.AddItem(new MenuItem("jungleMenu.usew", "Use W")).SetValue(true);
                    clearMenu.AddSubMenu(jungleMenu);
                }
                Config.AddSubMenu(clearMenu);
            }


            var harassMenu = new Menu("harass Settings", "harass Settings");
            {
                harassMenu.AddItem(new MenuItem("harassMenu.minmana", "Min Mana")).SetValue(new Slider(30));
                harassMenu.AddItem(new MenuItem("harassMenu.useq", "Use Q")).SetValue(true);
                harassMenu.AddItem(new MenuItem("harassMenu.usew", "Use W")).SetValue(true);
                harassMenu.AddItem(new MenuItem("harassMenu.usee", "Use E")).SetValue(true);
                Config.AddSubMenu(harassMenu);
            }

            var miscMenu = new Menu("Misc Settings", "Misc Settings");
            {
                miscMenu.AddItem(new MenuItem("miscMenu.edash", "E On Dash").SetValue(true));
                miscMenu.AddItem(new MenuItem("miscMenu.egap", "E On Gap").SetValue(true));
                miscMenu.AddItem(new MenuItem("miscMenu.eint", "E To Intterupt").SetValue(true));
                miscMenu.AddItem(new MenuItem("miscMenu.autolevel", "Auto Level Ult").SetValue(true));
                miscMenu.AddItem(new MenuItem("miscMenu.scrybebuy", "Auto Scrybing Orb Buy").SetValue(true));
                miscMenu.AddItem(new MenuItem("miscMenu.scrybebuylevel", "Buy At Level").SetValue(new Slider(8, 1, 18)));
                miscMenu.AddItem(new MenuItem("miscMenu.scrybe", "Use Scrybing Orb when Ultying").SetValue(true));
                miscMenu.AddItem(new MenuItem("miscMenu.aaminion", "AA Minions when have passive - BETA").SetValue(true));
                Config.AddSubMenu(miscMenu);
            }

            Config.AddToMainMenu();

        }
    }
}
