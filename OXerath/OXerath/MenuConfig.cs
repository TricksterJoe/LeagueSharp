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
namespace OXerath 
{
    internal class MenuConfig
    {
        public static Menu Config;
        public static string Menuname = "OXerath";

        public static void LoadMenu()
        {
            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Xerath.Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("comboMenu.useq", "Use Q")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usew", "Use W")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usee", "Use E")).SetValue(true);
                comboMenu.AddItem(new MenuItem("comboMenu.usertap", "Use R")).SetValue(new KeyBind(67, KeyBindType.Press));
                comboMenu.AddItem(new MenuItem("comboMenu.user", "R Mode"))
                    .SetValue(new StringList(new[] {"On Tap", "Regular R"}, 1));
                Config.AddSubMenu(comboMenu);
                comboMenu.AddItem(new MenuItem("comboMenu.userdelay", "R Delay")).SetValue(new Slider(500, 0, 2000));
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

            Config.AddToMainMenu(); 

        }
    }
}
