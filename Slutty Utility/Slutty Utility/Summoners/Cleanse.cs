using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Summoners
{
    class Cleanse : Helper
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

        public static void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            foreach (var buff in Bufftype)
            {
                if (Player.HasBuffOfType(buff) && GetBool("cleanse" + buff, typeof (bool)))
                {
                    Utility.DelayAction.Add(GetValue("cleansedelay"),
                        () => Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerboost")));
                }
            }
        }
    }
}
