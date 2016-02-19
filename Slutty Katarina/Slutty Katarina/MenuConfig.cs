using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Katarina
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
                combo.AddItem(new MenuItem("combomode", "[Combo] Mode"))
                    .SetValue(new StringList(new[] {"QEW"}));
                AddBools(combo, "Use [Q]", "useq", "Q Usage");
                AddBools(combo, "Use [W]", "usew", "W Usage");
                AddBools(combo, "Use [E]", "usee", "E Usage");
                AddBools(combo, "Use [R]", "user", "R Usage");
                AddBools(combo, "Don't [E] Under Turret", "useet", "Prevents you From E under turret");
                AddBools(combo, "Use [Ignite]", "useignite", "Ignite Usage");
                AddValue(combo, "Only [R] When Enemy Count >", "rcount", 1, 1, 4);
            }
            Config.AddSubMenu(combo);

            var harass = new Menu("Mixed Settings", "Mixed Settings");
            {
                AddBools(harass, "Use [Q]", "useqh", "Use Q");
                AddBools(harass, "Use [W]", "usewh", "Use W");
            }
            Config.AddSubMenu(harass);

            var laneclear = new Menu("Lane Clear Settings", "Lane Clear Settings");
            {
                AddBools(laneclear, "Last Hit [Q]", "qlasthitlane", "Last Hit With Q In Lane Clear");
                AddBools(laneclear, "Last Hit [W]", "wlasthitlane", "Last Hit With W In Lane Clear");
                AddBools(laneclear, "Use [Q]", "qlaneclear", "Use Q Always");
                AddValue(laneclear, "Min Minions To [Q]", "qlaneclearmin", 3, 1, 20);
                AddBools(laneclear, "Use [W]", "wlaneclear", "Use W Always");
                AddValue(laneclear, "Min Minions To [W]", "wlaneclearmin", 3, 1, 20);
            }
            Config.AddSubMenu(laneclear);

            var lasthit = new Menu("Last Hit Settings", "Last Hit Settings");
            {
                AddBools(lasthit, "Last Hit [Q]", "qlasthit", "Last Hit With Q");
                AddBools(lasthit, "Last Hit [W]", "wlasthit", "Last Hit With W");
            }
            Config.AddSubMenu(lasthit);

            var killsteal = new Menu("Kill Steal Settings", "Kill Steal Settings");
            {
                AddBool(killsteal, "Use Kill Steal", "ks");
                AddBools(killsteal, "Use [Q]", "useqks", "Use Q To Ks");
                AddBools(killsteal, "Use [W]", "usewks", "Use W To Ks");
                AddBools(killsteal, "Use [E]", "useeks", "Use E To Ks");
            }
            Config.AddSubMenu(killsteal);

            var drawings = new Menu("Drawing Settings", "Drawing Settings");
            {
                AddBools(drawings, "Draw [Q] Range", "drawq", "Q Range", false);
                AddBools(drawings, "Draw [W] Range", "draww", "W Range", false);
                AddBools(drawings, "Draw [E] Range", "drawe", "E Range", false);
                AddBools(drawings, "Draw [R] Range", "drawr", "R Range", false);
            }
            Config.AddSubMenu(drawings);

            AddKeyBind(Config, "Ward Jump", "wardjump", 'T', KeyBindType.Press);

            Config.AddToMainMenu();

        }
    }
}
