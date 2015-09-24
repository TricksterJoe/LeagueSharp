using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;   
using Color = System.Drawing.Color;

namespace Slutty_Utility.Drawings
{
    public class EnemyRanges
    {
        public static readonly SpellSlot[] Slots =
        {
            SpellSlot.Q,
            SpellSlot.E,
            SpellSlot.W,
            SpellSlot.R
        };

        public static void OnLoad()
        {
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!Helper.GetBool("displayenemyrange", typeof(bool))) return;
            foreach (
                var hero in
                    HeroManager.Enemies.Where(x => !x.IsDead && x.IsVisible && x.IsValid && x.Position.Distance(Helper.Player.Position) < 2000 && x.IsChampion()))
            {
                if (!Helper.GetBool("showdrawings" + hero.ChampionName, typeof (bool)))
                    return;
                if (!hero.IsVisible || hero.Distance(Helper.Player) > 2000) return;
                if (Helper.GetBool("showdrawingsaa" + hero.ChampionName, typeof (bool)))
                {
                    Render.Circle.DrawCircle(hero.Position, hero.AttackRange, Color.DeepPink, 3);
                }

//                foreach (var spell in hero.Spellbook.Spells)
//                {
//                    foreach (var herospell in Slots)
//                    {
//                        if (spell.Slot == herospell && !hero.IsDead &&
//                            Helper.GetBool(
//                                "spellrange.spellrangeenemy.spellrangeenemyname" + herospell + hero.ChampionName,
//                                typeof (bool)))
//                        {
//                            Render.Circle.DrawCircle(hero.Position, spell.SData.CastRange, spell.Slot == SpellSlot.Q
//                                ? Color.Blue
//                                : spell.Slot == SpellSlot.E
//                                    ? Color.Red
//                                    : spell.Slot == SpellSlot.W 
//                                    ? Color.Chocolate : Color.Aqua, 0);
//                        }
//                    }
//                }

            }
        }
    }
}

