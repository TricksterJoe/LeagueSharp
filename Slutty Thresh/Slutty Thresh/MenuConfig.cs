using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Thresh
{
    internal class MenuConfig
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public const string Menuname = "Slutty Thresh";

        public static void CreateMenuu()
        {
            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Draw", "Display Drawings").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Draw [Q]").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("wDraw", "Draw [W]").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("eDraw", "Draw [E]").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("qfDraw", "Draw Flash-[Q] Range").SetValue(true));

            var comboMenu = new Menu("Combo Settings" , "combospells");
            {
                var qsettings = new Menu("Q (Death Sentence) Settings", "settings");
                {
                    
                    qsettings.AddItem(new MenuItem("useQ", "Use [Q] (Death Sentence)").SetValue(true));
                    qsettings.AddItem(new MenuItem("smartq", "Smart [Q]").SetValue(true));
                    qsettings.AddItem(new MenuItem("useQ1", "Use 2nd [Q] (Death Leap)").SetValue(true));
                    qsettings.AddItem(
                        new MenuItem("useQ2", "Set 2nd-[Q] Delay (Death Leap)").SetValue(new Slider(1000, 0, 1500)));
                    qsettings.AddItem(
                        new MenuItem("qrange", "Use [Q] Only if Target Range >=").SetValue(new Slider(500, 0, 1040)));
                    comboMenu.AddSubMenu(qsettings);

                }
                comboMenu.AddItem(new MenuItem("useE", "Use [E] (Flay)").SetValue(true));
                comboMenu.AddItem(new MenuItem("combooptions", "Set [E] Mode").SetValue(new StringList(new[] {"Push", "Pull"}, 1)));
                comboMenu.AddItem(new MenuItem("useR", "Use [R] (The Box)").SetValue(true));
                comboMenu.AddItem(
    new MenuItem("rslider", "Use [R] Only if X Target(s) in Range").SetValue(new Slider(3, 1, 5)));

            }
            Config.AddSubMenu(comboMenu);

            var lantMenu = new Menu("Lantern Settings", "lantern");
            {
                foreach (var hero in 
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsAlly
                        && !x.IsMe))
                {
                    {
                        lantMenu.AddItem(new MenuItem("healop" + hero.ChampionName, hero.ChampionName))
                            .SetValue(new StringList(new[] {"Lantern", "No Lantern"}));

                        lantMenu.AddItem(
                            new MenuItem("hpsettings" + hero.ChampionName, "Lantern When % HP <").SetValue(
                                new Slider(20)));
                    }

                }
                lantMenu.AddItem(new MenuItem("manalant", "Set % Mana for Lantern").SetValue(new Slider(50)));
                lantMenu.AddItem(new MenuItem("autolantern", "Auto-Lantern Ally if [Q] hits").SetValue(false));
            }

            var laneMenu = new Menu("Lane Clear", "laneclear");
            {
                laneMenu.AddItem(new MenuItem("useelch", "Use [E]").SetValue(true));
                // laneMenu.AddItem(new MenuItem("elchslider", "Minimum Minions For E").SetValue(new Slider(0, 1, 10)));
            }
            Config.AddSubMenu(laneMenu);

            Config.AddSubMenu(lantMenu);
            var flashMenu = new Menu("Flash-Hook Settings", "flashf");
            {
                flashMenu.AddItem(new MenuItem("flashmodes", "Flash Modes")
                    .SetValue(new StringList(new[] {"Flash->E->Q", "Flash->Q"})));
                flashMenu.AddItem(new MenuItem("qflash", "Flash-Hook").SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            Config.AddSubMenu(flashMenu);

            var miscMenu = new Menu("Miscellaneous", "miscsettings");

            var eventMenu = new Menu("Events", "eventssettings");
            {
                eventMenu.AddItem(new MenuItem("useW2I", "Interrupt with [W]").SetValue(true));
                eventMenu.AddItem(new MenuItem("useQW2D", "Use W/Q on Dashing").SetValue(true));
            }

//            var itemMenu = new Menu("Item Usage", "items");
//            var shieldMenu = new Menu("Shield Usage", "shield");
//            {
//                var mountainmenu = new Menu("Face Of The Mountain", "faceof");
//                {
//                    foreach (var hero in
//                        ObjectManager.Get<Obj_AI_Hero>()
//                            .Where(x => x.IsAlly
//                                        && !x.IsMe))
//                    {
//                        {
//                            mountainmenu.AddItem(new MenuItem("faceop" + hero.ChampionName, hero.ChampionName))
//                                .SetValue(new StringList(new[] {"Use", "Don't Use"}));
//
//                            mountainmenu.AddItem(
//                                new MenuItem("facehp" + hero.ChampionName, "Use When % HP <").SetValue(new Slider(20)));
//                        }
//                    }
//                }
//
//                var locketmenu = new Menu("Locket Of Solari", "locksol");
//                {
//                    foreach (var hero in
//                        ObjectManager.Get<Obj_AI_Hero>()
//                            .Where(x => x.IsAlly
//                                        && !x.IsMe))
//                    {
//                        {
//                            locketmenu.AddItem(new MenuItem("locketop" + hero.ChampionName, hero.ChampionName))
//                                .SetValue(new StringList(new[] {"Use", "Don't Use"}));
//
//                            locketmenu.AddItem(
//                                new MenuItem("lockethp" + hero.ChampionName, "Use When % HP <").SetValue(
//                                    new Slider(20)));
//                        }
//                    }
//
//                }
//                shieldMenu.AddSubMenu(locketmenu);
//                shieldMenu.AddSubMenu(mountainmenu);
//            }
//
//            var healMenu = new Menu("Healing Items", "heals");
//            {
//                var mikaelss = new Menu("Mikael's Crucibile", "mikaels");
//                mikaelss.AddItem(new MenuItem("charm", "Charm").SetValue(true));
//                mikaelss.AddItem(new MenuItem("snare", "Snare").SetValue(true));
//                mikaelss.AddItem(new MenuItem("taunt", "Taunt").SetValue(true));
//                mikaelss.AddItem(new MenuItem("suppression", "Suppression").SetValue(true));
//                mikaelss.AddItem(new MenuItem("stun", "Stun").SetValue(true));
//                //  mikaelss.AddItem(new MenuItem("mikaelshp", "Use On %HP", true).SetValue(new Slider(20, 40)));
//                var allies = new Menu("Ally Config", "AllysConfig");
//                foreach (var hero in
//                    ObjectManager.Get<Obj_AI_Hero>()
//                        .Where(x => x.IsAlly
//                                    && !x.IsMe))
//                {
//                    {
//                        allies.AddItem(new MenuItem("healmikaels" + hero.ChampionName, hero.ChampionName))
//                            .SetValue(new StringList(new[] {"Use Mikaels On", "Don't Use Mikaels On"}));
//                    }
//                }
//                mikaelss.AddSubMenu(allies);
//                healMenu.AddSubMenu(mikaelss);
//            }
//            itemMenu.AddSubMenu(shieldMenu);
//            itemMenu.AddSubMenu(healMenu);
          //  Config.AddSubMenu(itemMenu);

            miscMenu.AddSubMenu(eventMenu);
            Config.AddSubMenu(miscMenu);

            Config.AddToMainMenu();
        }
    }
}
   
