using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Auto_Level_Manager
{
   class AutoLevel : Helper
    {
        public static void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
        }

        private struct Abilitys 
        {
            public const int Q = 1;
            public const int W = 2;
            public const int E = 3;
            public const int R = 4;
        }


        public static Spell Q, W, E, R;
        private static int[]_abilitySequence;
        private static void OnUpdate(EventArgs args)
        {
            if (!GetBool("useautolevel", typeof (bool))) return;

            switch (GetStringValue("autolevelmode"))
            {
                case 0:
                    _abilitySequence = new[]
                    {
                        Abilitys.Q, Abilitys.W, Abilitys.E, Abilitys.Q, Abilitys.Q, Abilitys.R, Abilitys.Q, Abilitys.W,
                        Abilitys.Q,
                        Abilitys.W, Abilitys.R, Abilitys.E, Abilitys.W, Abilitys.W, Abilitys.E, Abilitys.R, Abilitys.E,
                        Abilitys.E
                    };
                    break;
                case 1:
                    _abilitySequence = new[]
                    {
                        Abilitys.W, Abilitys.Q, Abilitys.E, Abilitys.W, Abilitys.W, Abilitys.R, Abilitys.W, Abilitys.Q,
                        Abilitys.W,
                        Abilitys.Q, Abilitys.R, Abilitys.E, Abilitys.Q, Abilitys.Q, Abilitys.E, Abilitys.R, Abilitys.E,
                        Abilitys.E
                    };
                    break;
                case 2:
                    _abilitySequence = new[]
                    {
                        Abilitys.E, Abilitys.Q, Abilitys.W, Abilitys.E, Abilitys.E, Abilitys.R, Abilitys.E, Abilitys.Q,
                        Abilitys.E,
                        Abilitys.Q, Abilitys.R, Abilitys.W, Abilitys.Q, Abilitys.Q, Abilitys.W, Abilitys.R, Abilitys.W,
                        Abilitys.W
                    };
                    break;
            }


            var qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            var wL = Player.Spellbook.GetSpell(SpellSlot.W).Level;
            var eL = Player.Spellbook.GetSpell(SpellSlot.E).Level;
            var rL = Player.Spellbook.GetSpell(SpellSlot.R).Level;

            if (qL + wL + eL + rL >= Player.Level) return;


            int[] level = { 0, 0, 0, 0 };

            for (var i = 0; i < Player.Level; i++)
            {
                level[_abilitySequence[i] - 1] = level[_abilitySequence[i] - 1] + 1;
            }

            if (qL < level[0]) Player.Spellbook.LevelSpell(SpellSlot.Q);
            if (wL < level[1]) Player.Spellbook.LevelSpell(SpellSlot.W);
            if (eL < level[2]) Player.Spellbook.LevelSpell(SpellSlot.E);
            if (rL < level[3]) Player.Spellbook.LevelSpell(SpellSlot.R);

        }
    }
}
