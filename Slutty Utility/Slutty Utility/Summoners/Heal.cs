using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Summoners
{
    class Heal : Helper
    {
        public Heal()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.GetSpellSlot("summonerheal").IsReady() && Player.CountEnemiesInRange(1400) >= 1)
            {
                foreach (var hero in HeroManager.Allies.Where(x => x.Distance(Player) < 500 && !x.IsDead && !x.IsRecalling()))
                {
                    if (HealthCheck("w/e" + hero.ChampionName) && GetBool("healmate" + hero.ChampionName, typeof(bool)))
                    {
                        Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerheal"));
                    }
                }
            }
        }
    }
}
