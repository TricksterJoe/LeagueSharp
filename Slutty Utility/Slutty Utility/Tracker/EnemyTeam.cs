using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Tracker
{
    internal class EnemyTeam : Helper
    {
        private static readonly SpellSlot[] _spellslot =
        {
            SpellSlot.Q,
            SpellSlot.W,
            SpellSlot.E,
            SpellSlot.R
        };

        private static int Y;
        private static int X;

        public static void OnLoad()
        {
            Drawing.OnDraw += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            foreach (var buffs in _spellslot)
            {
                for (var i = 0; i <= _spellslot.Count(); i++)
                {
                    X = (int) Player.HPBarPosition.X  + (i*23) + 35;

                    Y = (int) Player.HPBarPosition.Y - 5;
                    var CD = (int) (Player.GetSpell(buffs).CooldownExpires - Game.Time);
                    if (CD > 0)
                    {
                        Drawing.DrawText(X, Y, Color.AliceBlue,
                            CD.ToString());
                    }
                    if (CD == 0)
                    {
                        //here goes the sprite
                    }
                }
            }
        }
    }
}
