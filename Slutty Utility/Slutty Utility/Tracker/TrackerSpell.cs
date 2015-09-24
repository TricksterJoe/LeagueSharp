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
    internal class TrackerSpell : Helper
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
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {

            if (!GetBool("spelltracker", typeof(bool))) return;
            foreach (var hero in HeroManager.AllHeroes.Where(x => x.IsValid && x.IsVisible && !x.IsDead))
            {
                for (var i = 0; i < _spellslot.Count(); i++)
                {
                    X = (int)hero.HPBarPosition.X + (i * 30) + 35;
                    
                    Y = (int)hero.HPBarPosition.Y + 40;

                    Drawing.DrawText(X - 1, Y - 13, Color.AliceBlue,
                        _spellslot[i].ToString());
                    foreach (var spell in _spellslot)
                    {
                        var expires = (hero.Spellbook.GetSpell(_spellslot[i]).CooldownExpires);
                        var CD =
                            (int)
                                (expires -
                                 (Game.Time - 1));

                     //   var expiress = expires - (Game.Time + 0.3);
                    //    var CDPercent = expiress * 100 / CD;
                        if (CD > 0)
                        {
                            Drawing.DrawText(X, Y, Color.White, CD.ToString());
                        }
                        else
                        {
                            Drawing.DrawText(X, Y, Color.SlateGray, "0");
                        }
                    }
                }
            }
        }

    }
}