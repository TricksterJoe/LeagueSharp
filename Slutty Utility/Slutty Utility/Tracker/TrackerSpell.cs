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
            foreach (var hero in HeroManager.AllHeroes.Where(x => x.Distance(Player) < 3000))
            {
                for (var i = 0; i <= _spellslot.Count(); i++)
                {
                    X = (int) hero.HPBarPosition.X + (i*30) + 35;

                    Y = (int) hero.HPBarPosition.Y - 10;
                    Drawing.DrawText(X - 1, Y - 13, Color.AliceBlue,
                        _spellslot[i].ToString());
                    foreach (var spell in _spellslot)
                    {
                        var expires = (hero.Spellbook.GetSpell(_spellslot[i]).CooldownExpires);
                        var CD =
                            (int)
                                (expires -
                                 (Game.Time - 1));

                        var expiress = expires - (Game.Time + 0.3);
                        var CDPercent = expiress*100/CD;
                        Game.PrintChat(CDPercent.ToString());
                        if (CD > 0)
                        {
                            Drawing.DrawText(X, Y,
                                CDPercent >= 80
                                    ? Color.Blue
                                    : CDPercent >= 50 && CDPercent < 80
                                        ? Color.BlueViolet
                                        : CDPercent < 50 && CDPercent >= 30
                                            ? Color.LightSeaGreen
                                            : Color.LawnGreen, CD.ToString());
                        }
                        else
                        {
                            Drawing.DrawText(X, Y, Color.Black, "0", 100);
                        }
                    }
                }
            }
        }

    }
}

