using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    class Smite : Helper
    {
        public static Spell smiteSpell { get; set; }

        static double SmiteDamage(Obj_AI_Base target)
        {
            return smiteSpell.GetDamage(target);
            //Work???
        }

    }
}
