using LeagueSharp.Common;

namespace Lee_Sin
{
   internal class MenuConfig : Helper
    {
       public static readonly string[] Names =
        {
            "Krug",
            "Razorbeak",
            "Murkwolf",
            "Gromp",
            "Crab",
            "Blue",
            "Red",
            "Dragon",
            "Baron"
        };
        public static void OnLoad()
        {
            Config = new Menu(Menuname, Menuname, true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var combos = new Menu("Key Binds", "Key Binds");
            {
                AddKeyBind(combos, "StarCombo", "starcombo", 'Z', KeyBindType.Press);
                AddKeyBind(combos, "Ward Jump", "wardjump", 'A', KeyBindType.Press);
                AddKeyBind(combos, "Use Insec", "wardinsec", 'G', KeyBindType.Press);
                AddValue(combos, "Fixed Ward range", "fixedwardrange", 270, 0, 600);
                AddBool(combos, "Use Flash In Insec", "useflash");
                AddBool(combos, "Use Objects In Insec", "useobjects");
                AddBool(combos, "Click To Insec", "clickto");
                AddBool(combos, "Insec Towards Allies/Turrets", "useobjectsallies");
                AddBool(combos, "Prioritize Flash Over Ward", "prioflash");
                AddBool(combos, "Ward -> Flash Insec", "expwardflash", false);
                AddBool(combos, "Use Smite In Insec", "UseSmite");
               // AddValue(combos, "R Flash Delay", "rflashdelay", 80, 0, 500);
            }

            var combo = new Menu("Combo Settings", "Combo Settings");
            {
                AddBool(combo, "Use [Q]", "useq");
                AddBool(combo, "Use Second [Q]", "useq2");
                AddValue(combo, "Use Second [Q] Delay", "secondqdelay", 500, 0, 2500);
                AddBool(combo, "Use [E]", "usee");
                AddBool(combo, "Use [R]", "user");
                AddValue(combo, "Auto [R] On X targets", "autoron", 1, 1, 5);
                //var rmenu = new Menu("Auto R on X Enemies", "autorxenemies");
                //{
                //    AddBool(rmenu, "Use ward", "xeward");
                //    AddBool(rmenu, "Use flash", "xeflash");
                //    AddValue(rmenu, "Min enemies hit", "xeminhit", 3, 2, 5);
                //    combo.AddSubMenu(rmenu);
                //}
                AddBool(combo, "Use [W]", "wardjumpcombo");
                AddBool(combo, "Use [W]ard Jump", "wardjumpcombo1");
                AddBool(combo, "Use [Smite]", "usessmite");
                var items = new Menu("Use Items", "Use Items");
                {
                    items.AddItem(
                        new MenuItem("hydrati", "Hydra/Tiamat Mode").SetValue(
                            new StringList(new[] {"Combo", "Lane Clear", "Both", "None"})));
                    AddBool(items, "Youmuu's Ghostblade", "youm");
                    AddBool(items, "Randuin's Omen", "omen");
                    AddValue(items, "Minimum targets to Randuin", "minrand", 3, 1, 5);
                }
                combo.AddSubMenu(items);
            }

            var harass = new Menu("Harass Settings", "Harass Settings");
            {
                AddBool(harass, "Use [Q]", "useqh");
                AddBool(harass, "Use Second [Q]", "useq2h");
                AddValue(harass, "Use Second [Q] Delay", "secondqdelayh", 500, 0, 2500);
                AddBool(harass, "Use [E]", "useeh");
                AddValue(harass, "Min Energy", "minenergy", 100, 0, 200);
            }
            var laneclear = new Menu("Lane Clear Settings", "Lane Clear Settings");
            {
                AddBool(laneclear, "Use [Q]", "useql");
                AddBool(laneclear, "Use [E]", "useel");
                AddValue(laneclear, "[E] On X Minions", "useelv", 3, 1, 10);
                AddValue(laneclear, "Min Energy", "minenergyl", 100, 0, 200);
            }

            var lasthit = new Menu("Last Hit Settings", "Last Hit Settings");
            {
                AddBool(lasthit, "Use [Q] Last Hit", "useqlh");
                AddValue(lasthit, "Min Energy", "minenergylh", 100, 0, 200);
            }
            var jungleClear = new Menu("Jungle Clear Settings", "Jungle Clear Settings");
            {
                AddBool(jungleClear, "Use [Q]", "useqjl");
                AddBool(jungleClear, "Use [W]", "usewjl");
                AddBool(jungleClear, "Use [E]", "useejl");
                AddBool(jungleClear, "Smart Spell Usage", "usesjl");
            }

            var Smite = new Menu("Smite Options", "Smite Options");
            {
                var smtiekill = new Menu("Smite Killable", "Smite Killable");
                {
                    foreach (var name in Names)
                    {
                        AddBool(smtiekill, "Use Smite On " + name, "usesmiteon" + name);
                    }
                    AddBool(smtiekill, "Use Smite On Killable Jungle Mob", "smiteonkillable");
                }
                AddBool(Smite, "Calculate Q Damage On Smite", "qcalcsmite");
                AddBool(Smite, "Smite COllision Enemies", "collsmite");
                AddKeyBind(Smite, "Enable Smite", "smiteenable", 'M', KeyBindType.Toggle);
                Smite.AddSubMenu(smtiekill);
            }
            
            var drawings = new Menu("Drawings Menu", "Drawings Menu");
            {
                var jungle = new Menu("Jungle Drawings", "Jungle Drawings");
                {
                    AddBool(jungle, "Enable Drawings", "jungledraws");
                    AddBool(jungle, "Camp HP Bar", "jungledraw");
                    AddBool(jungle, "Killable Mob", "killmob");
                    AddBool(jungle, "Smite Enabled/Disabled", "enabledisablesmite");
                }
                drawings.AddSubMenu(jungle);
                var spells = new Menu("Spell Ranges", "Spell Ranges");
                {
                    AddBool(spells, "Show Drawings", "spellsdraw");
                    AddBool(spells, "Show Expected Target Position After Insec", "targetexpos");
                    AddBool(spells, "[Q] Range", "qrange");
                    AddBool(spells, "[W] Range", "wrange");
                    AddBool(spells, "[E] Range", "erange");
                    AddBool(spells, "[R] Range", "rrange");
                }
                var misc = new Menu("Misc Drawings", "Misc Drawings");
                {
                    AddBool(misc, "Show Expected Target Position After Insec", "targetexpos");
                    AddBool(misc, "Show Ward Position", "wardpositionshow");
                    AddBool(misc, "Display [R] Polygon", "rpolygon");
                    AddBool(misc, "Show target count hit by R", "counthitr");
                    AddBool(misc, "Show Line Between Objects", "linebetween");
                }
                drawings.AddSubMenu(misc);
                drawings.AddSubMenu(spells);
                AddBool(drawings, "Enable Drawings", "ovdrawings");
            }

            //todo Anti Dash/Gap Closer (Ward jump/R)

            Config.AddSubMenu(combos);
            Config.AddSubMenu(combo);
            Config.AddSubMenu(harass);
            Config.AddSubMenu(laneclear);
            Config.AddSubMenu(jungleClear);
            Config.AddSubMenu(lasthit);
            Config.AddSubMenu(Smite);
            Config.AddSubMenu(drawings);
            Config.AddToMainMenu();
        }
    }
}
