using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
    internal class Activator : Helper
    {
        public static void LoadActivator()
        {
            var activator = new Menu("Activator", "Activator");

            #region consumables

            var consumables = new Menu("Consumables", "Consumables");
            {
                var potions = new Menu("Potions", "Potions");
                {
                    AddValue(potions, "Hp Potion", "consumables.potions.hppotion", 30);
                    AddValue(potions, "Mana Potion", "consumables.potions.manapotion", 30);
                    AddValue(potions, "Biscuit", "consumables.potions.biscuit", 30);
                    AddValue(potions, "Flask", "consumables.potions.flask", 30);
                }
                consumables.AddSubMenu(potions);

                var elixers = new Menu("Elixers", "Elixers"); // 
                {
                    AddValue(elixers, "Buy Elixers At Level", "consumables.buy", 13, 1, 18);
                    AddBool(elixers, "Elixir of Iron", "consumables.elixers.iron", true);
                    AddBool(elixers, "Elixir of Ruin", "consumables.elixers.ruin", true);
                    AddBool(elixers, "Elixir of Sorcery", "consumables.elixers.sorcery", true);
                    AddBool(elixers, "Elixir of Wrath", "consumables.elixers.wrath", true);
                }
                consumables.AddSubMenu(elixers);
            }

            #endregion

            #region defensive

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
                            .Where(x => x.IsAlly))
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
                            .Where(x => x.IsAlly))
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
                            .Where(x => x.IsAlly))
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

            #endregion

            #region offensive

            var offensive = new Menu("Offensive", "Offensive");
            {
                var botrk = new Menu("Blade Of The Ruined King/Bilge", "Blade Of The Ruined King/Bilge");
                {
                    AddBool(botrk, "Only Use In Combo Mode", "offensive.botrk.combo", true);
                    AddBool(botrk, "Use Botrk/Bilge", "offensive.botrk", true);
                    AddValue(botrk, "Use When HP <=", "offensive.botrkvalue", 70);
                    AddBool(botrk, "Smart Botrk Usage", "offensive.smartbotrk", true);
                }
                offensive.AddSubMenu(botrk);

                var hydra = new Menu("Hydra/Tiamat", "Hydra/Tiamat");
                {
                    AddBool(hydra, "Use Hydra/Tiamat Minions", "offensive.hydraminions", true);
                    AddValue(hydra, "Use When > Enemies", "offensive.hydraminonss", 3, 1, 10);
                    AddBool(hydra, "Use Hydra/Tiamat Combo", "offensive.hydracombo", true);
                }
                offensive.AddSubMenu(hydra);

                AddBool(offensive, "Use Muramana", "offensive.muramana", true);

                AddBool(offensive, "Use Hextech", "offensive.hextech", true);

                AddBool(offensive, "Use Yoummuu's", "offensive.yom", true);
            }

            #endregion

            activator.AddSubMenu(consumables);
            activator.AddSubMenu(defensive);
            activator.AddSubMenu(offensive);
            Config.AddSubMenu(activator);

        }
    }
}

