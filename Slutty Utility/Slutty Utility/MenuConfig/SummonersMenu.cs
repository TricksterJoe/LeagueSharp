using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
    class SummonersMenu : Helper
    {
        public static readonly SpellSlot[] Slots =
        {
            SpellSlot.Q,
            SpellSlot.E,
            SpellSlot.W,
            SpellSlot.R
        };

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

        public static void  LoadSummonersMenu()
        {
            var summonersmenu = new Menu("Summoners Settings", "Summoners Settings");
            {
                var ignitemenu = new Menu("Ignite Settings", "Ignite Settings");
                {
                    var championslist = new Menu("Champion Blacklist", "Champion Black List");
                    foreach (var hero in HeroManager.Enemies)
                    {
                        AddBool(championslist, "Ignite " + hero.ChampionName, "useignite" + hero.ChampionName, true);
                    }
                    var spelllist = new Menu("Spell Calculations", "Spell Calculations");
                    foreach (var herospells in Slots)
                    {
                        AddBool(spelllist, "Calculate " + herospells + " Damage", "ignitecalculate" + herospells, true);
                    }
                    ignitemenu.AddSubMenu(championslist);
                    ignitemenu.AddSubMenu(spelllist);
                }
                summonersmenu.AddSubMenu(ignitemenu);

                var healmenu = new Menu("Heal Settings", "Heal Settings");
                {
                    foreach (var hero in HeroManager.AllHeroes.Where(x => x.IsMe || x.IsAlly))
                    {
                        AddBool(healmenu, "Use Heal On" + hero.ChampionName, "useheal" + hero.ChampionName, true);
                        AddValue(healmenu, "Min % HP to Heal", "percenthealth" + hero.ChampionName, 30);
                    }
                }
                summonersmenu.AddSubMenu(healmenu);

                var barriermenu = new Menu("Barrier Settings", "Barrier Settings");
                {
                    AddBool(barriermenu, "Use Barrier", "usebarrier", true);
                    AddValue(barriermenu, "Min. % HP to Barrier", "percenthealth", 30);
                }
                summonersmenu.AddSubMenu(barriermenu);

                var cleansemenu = new Menu("Cleanse Settings", "Cleanse Settings");
                {
                    foreach (var buff in Bufftype)
                    {
                        AddBool(cleansemenu, "Cleanse On " + buff, "cleanse" + buff, true);
                    }
                    AddBool(cleansemenu, "Use Cleanse", "usecleanse", true);
                    AddValue(cleansemenu, "Cleanse Usage Delay", "cleansedelay", 0, 0, 1500);
                }
                summonersmenu.AddSubMenu(cleansemenu);
            }
            Config.AddSubMenu(summonersmenu);
        }
    }
}
