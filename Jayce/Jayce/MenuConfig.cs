using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Jayce
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

            AddKeyBind(Config, "Manual E->Q", "manualeq", 'A', KeyBindType.Press);
            AddKeyBind(Config, "R Spam", "flee", 'T', KeyBindType.Toggle);
           // AddKeyBind(Config, "Insec", "insec", 'Z', KeyBindType.Press);
            var combo = new Menu("Combo Settings", "Combo Settings");
            {
                var melee = new Menu("Melee Settings", "Melee Settings");
                {
                    AddBool(melee, "Use [Q]", "useqcm");
                    AddBool(melee, "Use [W]", "usewcm");
                    AddBool(melee, "Use [E]", "useecm");
                    AddBool(melee, "Smart [E]", "useecme");
                }
                var range = new Menu("Ranged Settings", "Ranged Settings");
                {
                    AddBool(range, "Use [Q]", "useqcr");
                    AddBool(range, "Use [W]", "usewcr");
                    AddBool(range, "Use [E]", "useecr");
                }
                
                AddBool(combo, "Auto Change Forms ([R])", "usercf");
                combo.AddSubMenu(melee);
                combo.AddSubMenu(range);
            }

            var harass = new Menu("Harass Settings", "harass Settings");
            {
                var melee = new Menu("Melee Settings", "Melee Settingss");
                {
                    AddBool(melee, "Use [Q]", "useqhm");
                   // AddBool(combo, "Use [W]", "usewhm");
                   // AddBool(combo, "Use [E]", "useehm");
                }
                var range = new Menu("Ranged Settings", "Ranged Settingss");
                {
                    AddBool(range, "Use [Q]", "useqhr");
                    AddBool(range, "Use [W]", "usewhr");
                  //  AddBool(combo, "Use [E]", "useehr");
                }
               
                harass.AddSubMenu(melee);
                harass.AddSubMenu(range);
            }

            var laneclear = new Menu("Lane Clear Settings", "Lane Clear Settings");
            {
                AddValue(laneclear, "Minimum minions hit For W/Q", "minhitwq", 2, 0, 10);
                AddValue(laneclear, "Minimum Mana", "minmana", 30);
                var melee = new Menu("Melee Settings", "Melee Settingssss");
                {
                    AddBool(melee, "Use [Q]", "useqlm");
                    AddBool(melee, "Use [W]", "usewlm");
                    AddBool(melee, "Use [E]", "useelm");
                }
                var range = new Menu("Ranged Settings", "Ranged Settingss");
                {
                    AddBool(range, "Use [Q]", "useqlr");
                    AddBool(range, "Use [W]", "usewlr");
                }

                laneclear.AddSubMenu(melee);
                laneclear.AddSubMenu(range);
            }

            var drawings = new Menu("Drawings", "Drawings");
            {
                AddBool(drawings, "Draw [Q]", "drawq");
                AddBool(drawings, "Draw [E]", "drawe");
                AddBool(drawings, "Draw Timers", "drawtimers");
            }

            var misc = new Menu("Misc Settings", "Misc Settings");
            {
                AddBool(misc, "Auto E On Interruptable", "autoeint");
                AddBool(misc, "Auto E On Dash", "autoedash");
                AddBool(misc, "Auto E On Gap Closers", "autoegap");
            }


            Config.AddSubMenu(combo);
            Config.AddSubMenu(harass);
            Config.AddSubMenu(laneclear);
            Config.AddSubMenu(drawings);
            Config.AddSubMenu(misc);
            Config.AddToMainMenu();
        }
    }
}
