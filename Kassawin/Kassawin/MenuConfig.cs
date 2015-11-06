using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Kassawin
{
    class MenuConfig : Helper
    {
        public static void OnLoad()
        {
            Config = new Menu(Menuname, Menuname, true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var combo = new Menu("Combo Settings", "Combo Settings");
            {
                AddBools(combo, "Use [Q]", "useq", "Q Usage");
                AddBools(combo, "Use [W]", "usew", "W Usage");
                AddBools(combo, "Use [E]", "usee", "E Usage");
                AddBools(combo, "Use [R]", "user", "R Usage");
                AddBools(combo, "Use [Ignite]", "useignite", "Ignite Usage");
                AddValue(combo, "Only [R] When Count Below", "rcount", 1, 1, 5, "50 = 1, 100 = 2, Etc");
            }
            Config.AddSubMenu(combo);

            var harass = new Menu("Harass Settings", "Harass Settings");
            {
                AddValue(harass, "Minimum Mana", "harassmana", 30, 0, 100, "Minimum Mana To Use Harass");
                AddBools(harass, "Use [Q]", "useqharass", "Q Usage");
                AddBools(harass, "Use [E]", "useeharass", "E Usage");
            }
            Config.AddSubMenu(harass);

            var farm = new Menu("Farm Settings", "Farm Settings");
            {
                var laneclear = new Menu("Lane Clear", "Lane Clear");
                {
                    AddValue(laneclear, "Minimum Mana", "minmanalaneclear", 30, 0, 100, "Minimum Mana To Use Lane Clear");
                    AddBools(laneclear, "Use [Q]", "useql", "Q A Minion");
                    AddBools(laneclear, "Use [W]", "usewl", "W usage");
                    AddBools(laneclear, "Use [E]", "useel", "E minions");
                    AddBools(laneclear, "Use [R]", "userl", "R Minions");
                    AddValue(laneclear, "Minimum Minions For E", "useels", 3, 1, 10, "Minion count to use E");
                    AddValue(laneclear, "Minimum Minions For R", "userls", 3, 1, 10, "Minion count to use R");
                    AddValue(laneclear, "Only [R] When Count Below", "rcountl", 1, 1, 5, "50 = 1, 100 = 2, Etc");
                }
                farm.AddSubMenu(laneclear);

                var jungleclear = new Menu("Jungle Clear", "Jungle Clear");
                {
                    AddValue(jungleclear, "Minimum Mana", "minmanajungleclear", 30, 0, 100, "Minimum Mana To Use Jungle Clear");
                    AddBools(jungleclear, "Use [Q]", "useqj", "Q Usage");
                    AddBools(jungleclear, "Use [W]", "usewj", "W Usage");
                    AddBools(jungleclear, "Use [E]", "useej", "E Usage");
                    AddBools(jungleclear, "Use [R]", "userj", "R Usage");
                    AddValue(jungleclear, "Only [R] When Count Below", "rcountj", 1, 1, 5, "50 = 1, 100 = 2, Etc");
                }
                farm.AddSubMenu(jungleclear);

                var lasthit = new Menu("Last Hit", "Last Hit");
                {
                    AddValue(lasthit, "Minimum Mana", "minmanalasthit", 30, 0, 100, "Minimum mana To Use Last Hit");
                    AddBools(lasthit, "Use [Q]", "useqlh", "Q Usage");
                }
                farm.AddSubMenu(lasthit);
            }
            Config.AddSubMenu(farm);
            var ks = new Menu("Kill Steal Settings", "Kill Steal Settings");
            {
                AddBools(ks, "Activate [KS]", "ks", "Turn off To disable Kill Steal");
                AddBools(ks, "Use [Q]", "qks", "Use Q To Ks");
                AddBools(ks, "Use [E]", "eks", "Use E To Ks");
                AddBools(ks, "Use [R]", "rks", "Use R To Ks");
                AddBools(ks, "Use [R] As Gap Closer", "rgks", "Gap Close With R");
            }
            Config.AddSubMenu(ks);

            var inte = new Menu("Interruptable Menu", "Interruptable Menu");
            {
                AddBools(inte, "Use [Q]", "useqint", "Q Interruptable Spell");
                AddValue(inte, "Interrupt Danager Level", "interruptlevel", 1, 1, 3, "1 = Low, 2 = Medium, 3 = High");
            }
            Config.AddSubMenu(inte);

            var drawings = new Menu("Drawings", "Drawings");
            {
                AddBools(drawings, "Enable Drawings", "enabledraw", "Turn Off To disable Drawings");
                AddBool(drawings, "Draw [Q] Range", "drawq");
                AddBool(drawings, "Draw [E] Range", "drawe");
                AddBool(drawings, "Draw [R] Range", "drawr");
                AddBool(drawings, "Draw Killable Minions with [Q]", "drawqkill");
                AddBool(drawings, "Draw [R] Count", "drawcount");
                AddBool(drawings, "Draw Damage", "drawdamage");
            }
            Config.AddSubMenu(drawings);

            AddKeyBind(Config, "Flee Mode", "fleemode", 'A', KeyBindType.Press );
            AddBool(Config, "Use [R] Flee Mode", "userflee");
            Config.AddToMainMenu();
        }

    }
}
