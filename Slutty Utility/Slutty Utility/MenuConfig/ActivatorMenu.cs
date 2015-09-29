using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
     class ActivatorMenu : Helper
    {
         public static readonly BuffType[] Bufftype =
        {
            BuffType.Snare, 
            BuffType.Blind, 
            BuffType.Charm, 
            BuffType.Stun,
            BuffType.Fear, 
            BuffType.Slow,
            BuffType.Taunt, 
            BuffType.Suppression
        };

        public static void LoadActivator()
        {
            var activator = new Menu("Activator", "Activator");

            #region consumables

            var consumables = new Menu("Consumables", "Consumables");
            {
                var potions = new Menu("Potions", "Potions");
                {
                    AddValue(potions, "Health Potion", "consumables.potions.hppotion", 30);
                    AddValue(potions, "Mana Potion", "consumables.potions.manapotion", 30);
                    AddValue(potions, "Biscuit", "consumables.potions.biscuit", 30);
                    AddValue(potions, "Flask", "consumables.potions.flask", 30);
                }
                consumables.AddSubMenu(potions);

                var elixers = new Menu("Elixers", "Elixers"); // 
                {
                    AddValue(elixers, "Buy Elixirs at Level:", "consumables.buy", 13, 1, 18);
                    AddBool(elixers, "Elixir of Iron", "consumables.elixers.iron", false);
                    AddBool(elixers, "Elixir of Ruin", "consumables.elixers.ruin", false);
                    AddBool(elixers, "Elixir of Sorcery", "consumables.elixers.sorcery", false);
                    AddBool(elixers, "Elixir of Wrath", "consumables.elixers.wrath", false);
                }
                consumables.AddSubMenu(elixers);
            }

            #endregion

            #region defensive

            var defensive = new Menu("Defensive", "Defensive");
            {
                AddBool(defensive, "Zhonya", "defensive.zhonya", true);

                var omen = new Menu("Randuin's Omen", "Randuins Omen");
                {
                    AddBool(omen, "Use Randuin's Omen", "defensive.omen", true);
                    AddValue(omen, "Only When >= Target(s) in Range", "defensive.omencount", 2, 1, 5);
                }
                defensive.AddSubMenu(omen);

                var mikaels = new Menu("Mikael's", "Mikaels");
                {
                    AddBool(mikaels, "Use Mikael's", "defensive.mikaels", true);
                    AddValue(mikaels, "Mikael's Usage Delay", "mikaelsdelay", 0, 0, 1000);
                    foreach (var hero in HeroManager.Allies)
                    {
                        var heros = new Menu(hero.ChampionName, hero.ChampionName);
                        foreach (var buff in Bufftype)
                        {
                            AddBool(heros, "Use On" + buff, "mikalesuse" + buff, true);
                        }
                        AddBool(heros, "Use Mikael's", "usemikaels" + hero.ChampionName, true);
                        mikaels.AddSubMenu(heros);
                    }
                }
                defensive.AddSubMenu(mikaels);


            var qss = new Menu("QSS/Mercurial", "QSS/Mercurial");
                {
                    foreach (var buff in Bufftype)
                    {
                        AddBool(qss, "Use QSS/Mercurial On " + buff , "defensive.qss" + buff, true);
                    }
                    AddValue(qss, "QSS/Mercurial Usage Delay", "qssdelay", 0, 0, 1000);
                }
                defensive.AddSubMenu(qss);

                var mountainmenu = new Menu("Face of the Mountain", "Face Of The Mountain");
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

                var locketmenu = new Menu("Locket of Solari", "Locket Of Solari");
                {
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly))
                    {
                        {
                            locketmenu.AddItem(new MenuItem("locketop" + hero.ChampionName, hero.ChampionName))
                                .SetValue(new StringList(new[] {"Use", "Don't Use"}));

                            locketmenu.AddItem(
                                new MenuItem("lockethp" + hero.ChampionName, "Use When % HP <=").SetValue(
                                    new Slider(20)));
                        }
                    }
                }
                defensive.AddSubMenu(locketmenu);

                var seraphmenu = new Menu("Seraph's Embrace", "Seraph's Embrace");
                {
                    AddBool(seraphmenu, "Use Seraph's Embrace", "defensive.seraphmenu", true);
                    AddValue(seraphmenu, "Use When % HP <=", "defensive.value", 45);
                }
                defensive.AddSubMenu(seraphmenu);
            }

            #endregion

            #region offensive

            var offensive = new Menu("Offensive", "Offensive");
            {
                var botrk = new Menu("Blade of the Ruined King/Cutlass", "Blade Of The Ruined King/Bilge");
                {
                    AddBool(botrk, "Only Use in Combo", "offensive.botrk.combo", true);
                    AddBool(botrk, "Use BotRK/Cutlass", "offensive.botrk", true);
                    AddValue(botrk, "Use When % HP <=", "offensive.botrkvalue", 70);
                    AddBool(botrk, "Smart BotRK Usage", "offensive.smartbotrk", true);
                }
                offensive.AddSubMenu(botrk);

                var hydra = new Menu("Hydra/Tiamat", "Hydra/Tiamat");
                {
                    AddBool(hydra, "Use Hydra/Tiamat on Minions", "offensive.hydraminions", true);
                    AddValue(hydra, "Use When > Minion(s)", "offensive.hydraminonss", 3, 1, 10);
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

