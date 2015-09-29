using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Lucian
{
    internal class MenuConfig : Helper
    {
        public static void OnLoad()
        {
            Config = new Menu(MenuName, MenuName, true);
            var orbwalkerMenu = new Menu("Orbwalker", "Orbwalker");
            Config.AddSubMenu(orbwalkerMenu);

            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            var combo = new Menu("Combo Settings", "Combo Settings");
            {
                AddBool(combo, "Use [Q]", "useqc");
                AddBool(combo, "Use Smart [Q]", "useqcs");
                AddBool(combo, "Use [W]", "usewc");
                AddBool(combo, "Use [E]", "useec");
                AddBool(combo, "Use [R]", "userc");
                AddBool(combo, "Move to Target During [R]", "usercmove");
            }
            Config.AddSubMenu(combo);

            var mixed = new Menu("Mixed Settings", "Mixed Settings");
            {
                AddBool(mixed, "Use [Q]", "useqh");
                AddBool(mixed, "Use Smart [Q]", "useqhs");
                AddBool(mixed, "Use [W]", "usewh");
                AddValue(mixed, "Min. % Mana", "minmanah", 30);
            }
            Config.AddSubMenu(mixed);

            var laneclear = new Menu("Lane Clear", "Lane Clear");
            {
                AddValue(laneclear, "Min. % Mana", "minmanal", 30);
                AddBool(laneclear, "Use [Q]", "useq");
                AddBool(laneclear, "Use [W]", "usew");
                AddValue(laneclear, "Use [Q] if X minions", "xminions", 1, 1, 6);
                AddValue(laneclear, "Use [W] if X minions", "xminionsw", 1, 1, 6);
            }
            Config.AddSubMenu(laneclear);

            var drawing = new Menu("Drawings", "Drawins");
            {
                AddBool(drawing, "Draw [Q]", "drawq");
                AddBool(drawing, "Draw [W]", "draww");
                AddBool(drawing, "Draw [E]", "drawe");
                AddBool(drawing, "Draw [R]", "drawr");
            }
            Config.AddSubMenu(drawing);
            Config.AddToMainMenu();
        }
    }
}
