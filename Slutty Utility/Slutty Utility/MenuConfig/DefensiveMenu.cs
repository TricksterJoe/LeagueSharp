using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility;

namespace Slutty_Utility
{
    class DefensiveMenu : Helper
    {
        public static void LoadDefensiveMenu()
        {
            var defensive = new Menu("Defensive", "Defensive");
            {
                AddBool(defensive, "Zhonya", "defensive.zhonya", true);

                var omen = new Menu("Randuins Omen", "Randuins Omen");
                {
                    AddBool(omen, "Use Randuins Omen", "defensive.omen", true);
                    AddValue(omen, "Only When >= Enemies In range", "defensive.omencount", 2, 1, 5);
                }
                defensive.AddSubMenu(omen);

                var mikaels = new Menu("Mikaels", "Mikaels");
                {
                    AddBool(mikaels, "Use Mikaels", "defensive.mikaels", true);
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly
                                        && !x.IsMe))
                    {
                        {
                            mikaels.AddItem(new MenuItem("mikaels" + hero.ChampionName, hero.ChampionName))
                                .SetValue(new StringList(new[] {"Use", "Don't Use"}));
                        }
                    }
                }
                defensive.AddSubMenu(mikaels);

                var qss = new Menu("QSS/Mercurial", "QSS/Mercurial");
                {
                    AddBool(qss, "Use QSS/Mercurial", "defensive.qss", true);
                }
                defensive.AddSubMenu(qss);

                var mountainmenu = new Menu("Face Of The Mountain", "Face Of The Mountain");
                {
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly
                                        && !x.IsMe))
                    {
                        {
                            mountainmenu.AddItem(new MenuItem("Mountain" + hero.ChampionName, hero.ChampionName))
                                .SetValue(new StringList(new[] {"Use", "Don't Use"}));

                            mountainmenu.AddItem(
                                new MenuItem("facehp" + hero.ChampionName, "Use When % HP <").SetValue(new Slider(20)));
                        }
                    }
                }
                defensive.AddSubMenu(mountainmenu);

                var locketmenu = new Menu("Locket Of Solari", "Locket Of Solari");
                {
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly
                                        && !x.IsMe))
                    {
                        {
                            locketmenu.AddItem(new MenuItem("locketop" + hero.ChampionName, hero.ChampionName))
                                .SetValue(new StringList(new[] {"Use", "Don't Use"}));

                            locketmenu.AddItem(
                                new MenuItem("lockethp" + hero.ChampionName, "Use When % HP <").SetValue(
                                    new Slider(20)));
                        }
                    }
                }
                defensive.AddSubMenu(locketmenu);

                var seraphmenu = new Menu("Seraph's Embrace", "Seraph's Embrace");
                {
                    AddBool(seraphmenu, "Use Seraph's Embrace", "defensive.seraphmenu", true);
                    AddValue(seraphmenu, "Use When HP <=", "defensive.value", 45);
                }
                defensive.AddSubMenu(seraphmenu);
            }
            Config.AddSubMenu(defensive);
        }
    }
}
