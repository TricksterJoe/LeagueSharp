using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Summoners
{
    class Ignite : Helper
    {
        public static SpellSlot Ignite1; // fucking ignite class name
        public static readonly SpellSlot[] Slots =
        {
            SpellSlot.Q,
            SpellSlot.E,
            SpellSlot.W,
            SpellSlot.R
        };


        public static void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite1 == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite1) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void OnUpdate(EventArgs args)
        {
            foreach (var spell in Slots)
            {
                Player.Spellbook.GetSpell(spell).IsReady();
            }
            foreach (var hero in HeroManager.Enemies.Where(x => x.IsValid && x.Distance(Player) <= 600 && !x.IsDead))
            {
                if (GetBool("useignite" + hero.ChampionName, typeof (bool)))
                {
                    if (Player.GetSpellDamage(hero, SpellSlot.W) <= 0)
                    if (Ignite1.IsReady() &&
                            (hero.Health <=
                            Player.GetSpellDamage(hero, SpellSlot.Q)
                            + Player.GetSpellDamage(hero, SpellSlot.W)
                            + Player.GetSpellDamage(hero, SpellSlot.E)
                            + Player.GetAutoAttackDamage(hero) + IgniteDamage(hero)))
//                        && (Player.Spellbook.GetSpell(SpellSlot.W).IsReady() &&
//                            Player.Spellbook.GetSpell(SpellSlot.E).IsReady() &&
//                            Player.Spellbook.GetSpell(SpellSlot.Q).IsReady()))
                    {
                        Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerdot"), hero);
                    }
                }
            }
        }
    }
}
