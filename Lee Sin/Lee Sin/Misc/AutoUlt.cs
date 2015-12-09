using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lee_Sin.Misc
{
    class AutoUlt : LeeSin
    {
        public static void AutoUlti()
        {
            // Hoes code below
            if (GetBool("wardinsec", typeof(KeyBind))) return;

            var target =
                HeroManager.Enemies.Where(x => x.Distance(Player) < R.Range && !x.IsDead && x.IsValidTarget(R.Range))
                    .OrderBy(x => x.Distance(Player)).FirstOrDefault();
            if (target == null || Player.IsDead)
            {
                UltPoly = null;
                _ultPolyExpectedPos = null;
                return;
            }

            UltPoly = new Geometry.Polygon.Rectangle(Player.ServerPosition,
                Player.ServerPosition.Extend(target.Position, 1100),
                target.BoundingRadius + 10);

            var counts =
                HeroManager.Enemies.Where(x => x.Distance(Player) < 1100 && x.IsValidTarget(1100))
                    .Count(h => h.NetworkId != target.NetworkId && UltPoly.IsInside(h.ServerPosition));

            if (counts >= GetValue("autoron") && R.IsReady())
            {
                R.Cast(target);
            }
        }
    }
}
