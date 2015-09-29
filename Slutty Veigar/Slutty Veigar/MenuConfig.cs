using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Veigar
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
                AddBool(combomenu, "Use [Q]", "useqcombo");
                combomenu.AddItem(new MenuItem("usewmode", "[W] Mode"))
                    .SetValue(new StringList(new[] {"Regular", "On Immobilized", "Don't Use"}));

                AddBool(combomenu, "Use [E]", "useecombo");
                var rsettings = new Menu("[R] Settings", "[R] [S]ettings");
                foreach (var hero in HeroManager.Enemies)
                {
                    rsettings.AddItem(new MenuItem("user" + hero.ChampionName, "Use [R] " + hero.ChampionName))
                        .SetValue(new StringList(new[] {"Use", "Don't Use"}));
                }
                AddBool(rsettings, "Use [R]", "users");
                AddBool(combomenu, "Block AA in Combo", "aablock", false);
                combomenu.AddSubMenu(rsettings);
            }
            Config.AddSubMenu(combomenu);

            var harassmenu = new Menu("Harass Settings", "Harass Settings");
            {
                AddBool(harassmenu, "Use [Q]", "useqharass");
                AddBool(harassmenu, "Smart [Q] Enable", "smartq"); // so much kappa
                harassmenu.AddItem(new MenuItem("usewmodeharass", "[W] Mode"))
                    .SetValue(new StringList(new[] { "Regular", "On Immobilized", "Don't Use" }));
                AddBool(harassmenu, "Use [E]", "useeharass");
            }
            Config.AddSubMenu(harassmenu);

            var killsteal = new Menu("Killsteal", "Kill Steal");
            {
                AddBool(killsteal, "Use [Q]", "useqks");
                AddBool(killsteal, "Use [W]", "usewks");
                AddBool(killsteal, "Use [R]", "userks");
            }
            Config.AddSubMenu(killsteal);

            var lanemenu = new Menu("Lane Clear Settings", "Lane Clear Settings");
            {
                AddValue(lanemenu, "Min. % Mana To Lane Clear", "minmana", 40);
                AddBool(lanemenu, "Use [Q]", "useqlaneclear");
                lanemenu.AddItem(new MenuItem("qmode", "Q Modes"))
                    .SetValue(new StringList(new[] {"2 Minions Kills", "1 Minion Kill", "None"}));
                AddBool(lanemenu, "Use [W]", "usewlaneclear");
                AddValue(lanemenu, "Use W if X Minions", "wminionjigolo", 3, 1, 20);           
            }
            Config.AddSubMenu(lanemenu);

            var lasthit = new Menu("Last Hit", "[L]ast Hit");
            {
                AddValue(lasthit, "Min. % Mana To Lane Clear", "minmanalast", 40);
                AddBool(lasthit, "Use [Q]", "useqlaneclearlast");
                lasthit.AddItem(new MenuItem("qmodelast", "Q Modes"))
                    .SetValue(new StringList(new[] { "2 Minions Killed", "1 Minion Killed", "None" }));
            }
            Config.AddSubMenu(lasthit);

            var junglemenu = new Menu("Jungle Clear", "Jungle Clear");
            {
                AddBool(junglemenu, "Use [Q]", "useqjungle");
                AddBool(junglemenu, "Use [W]", "usewjungle");
                AddValue(junglemenu, "Min. % Mana", "minmanajungle", 40);
            }
            Config.AddSubMenu(junglemenu);

            var enviorment = new Menu("Miscellaneous", "eviorment");
            {
                AddBool(enviorment, "Auto-Stack Tear at Fountain", "tearoptions");
                AddKeyBind(enviorment, "Flee Mode", "fleemode", 'Z', KeyBindType.Press);
                AddBool(enviorment, "Use [E] In Flee Mode", "efleemode");
                AddBool(enviorment, "Auto-Use [E]", "autoe");
                AddValue(enviorment, "Auto [E] on X Target(s)", "AutoE", 2, 2, 5);
            }
            Config.AddSubMenu(enviorment);

            var drawingmenu = new Menu("Drawing Menu", "[D]rawing Menu");
            {
                AddBool(drawingmenu, "Display Drawings", "displayrange");
                AddBool(drawingmenu, "Display Q Range", "displayQrange");
                AddBool(drawingmenu, "Display W Range", "displayWrange");
                AddBool(drawingmenu, "Display E Range", "displayErange");
                AddBool(drawingmenu, "Display R Range", "displayRrange");
                AddBool(drawingmenu, "Display Player Damage on Target", "FillDamage");
                AddBool(drawingmenu, "Fill Damage", "RushDrawWDamageFill");
            }
            Config.AddSubMenu(drawingmenu);

            AddKeyBind(Config, "Auto-[Q] Toggle", "autoqtoggle", 'G', KeyBindType.Toggle);
            Config.AddItem(new MenuItem("autoq", "Auto-[Q] Farm"))
                .SetValue(new StringList(new[] {"2 Minions Killed", "1 Minion Killed", "None"}));

            


        }
    }
}
