using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Kindred
{
    class MenuConfig : Helper
    {
        public static void OnLoad()
        {

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var combomenu = new Menu("Combo Settings", "Combo Settings");
            {
                combomenu.AddItem(new MenuItem("qmodes", "Q Modes"))
                    .SetValue(new StringList(new[] {"Q To Mouse", "Safe Q", "Dont Use Q"}));
                AddBool(combomenu, "Use W", "usew", true);
                AddBool(combomenu, "Use E", "use", true);
                AddBool(combomenu, "Focus E target", "focusetarget", true);
                AddBool(combomenu, "Use R", "user", true);
                AddValue(combomenu, "Minimum Allies in Range R", "minallies", 2, 1, 5);
                AddValue(combomenu, "Minimum Enemies in Range R", "minenemies", 2, 1, 5);
                AddValue(combomenu, "Min HP To R", "minhpr", 30);
            }
            Config.AddSubMenu(combomenu);

            var laneclear = new Menu("Lane Clear Settings", "Lane Clear Settings");
            {
                AddBool(laneclear, "Use [Q]", "useql", true);
                AddBool(laneclear, "Use [W]", "usewl", true);
                AddBool(laneclear, "Use [E]", "useel", true);
                AddValue(laneclear, "[Q] Minion Slider", "qminslider", 3, 1, 10);
                AddValue(laneclear, "[W] Minion Slider", "wminslider", 3, 1, 10);
                AddValue(laneclear, "Min Mana", "minmana", 30);

            }
            Config.AddSubMenu(laneclear);

            var jungleclear = new Menu("Jungle Clear Settings", "Jungle Clear settings");
            {
                AddBool(jungleclear, "Use [Q]", "useqjungleclear", true);
                AddBool(jungleclear, "Use [W]", "usewjungleclear", true);
                AddBool(jungleclear, "Use [E]", "useejungleclear", true);
            }
            Config.AddSubMenu(jungleclear);

            var drawmenu = new Menu("Drawing Settings", "Drawing Settings");
            {
                AddBool(drawmenu, "Enable Drawings", "draws", true);
                AddBool(drawmenu, "Draw [Q]", "drawq", true);
                AddBool(drawmenu, "Draw [W]", "draww", true);
                AddBool(drawmenu, "Draw [E]", "drawe", true);
                AddBool(drawmenu, "Draw [R]", "drawr", true);
            }
            Config.AddSubMenu(drawmenu);

            AddKeyBind(Config, "Wall Hops", "wallhops", 'Z', KeyBindType.Press);
            Config.AddToMainMenu();
        }
    }
}
