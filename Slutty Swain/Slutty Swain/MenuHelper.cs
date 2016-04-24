using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Swain
{
    class MenuHelper : Helper
    {
        public static void MenuOnLoad()
        {
            Config = new Menu(Menuname, Menuname, true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var combo = new Menu("Combo Settings", "Combo Settings");
            {
                AddBool(combo, "Use [Q]", "useq");
                AddBool(combo, "Use [W]", "usew");
                AddBool(combo, "Use [E]", "usee");
                AddBool(combo, "Use [R]", "user");
                AddValue(combo, "Min %Mana for [R]", "minmanarc", 30);
            }
            combo.AddSubMenu(Config);

            var mixed = new Menu("Mixed Settings", "Mixed Settings");
            {
                AddBool(combo, "Use [Q]", "useqm");
                AddBool(combo, "Use [E]", "useem");
            }
            mixed.AddSubMenu(mixed);

            var laneclear = new Menu("Lane Clear Settings", "Lane Clear Settings");
            {
                AddBool(laneclear, "Use [Q]", "useql");
                AddBool(laneclear, "Use [E]", "useel");
                AddBool(laneclear, "Use [R]", "userl");
                AddValue(laneclear, "Min Minions for [R]", "minminionsrl", 3, 1, 10);
                AddValue(laneclear, "Min %Mana for [R]", "minmanarl", 30);
            }
            laneclear.AddSubMenu(Config);

            var drawings = new Menu("Drawing Settings", "Drawing Settings");
            {
                AddBool(drawings, "Draw [Q]", "drawq", false);
                AddBool(drawings, "Draw [W]", "draww", false);
                AddBool(drawings, "Draw [E]", "drawe", false);
            }

            Config.AddToMainMenu();
        }
    }
}
