using System;
using System.Collections.Generic;
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
        public static int MaxTravelDistance()
        {
            var slot = Items.GetWardSlot();
            var flash = Player.GetSpellSlot("summonerflash");
            if (slot != null && flash.IsReady() && W.IsReady())
            {
                return 1125;
            }

            if (slot != null && !W.IsReady())
            {
                return 185;
            }

            if (slot != null && W.IsReady())
            {
                return 600;
            }

            if (slot == null)
            {
                return 185;
            }

            return 185;
        }


        public static void AutoWardUlt()
        {
            var distance = MaxTravelDistance();
            var enemiescount = GetValue("enemiescount");
            var enemies = Player.GetEnemiesInRange(2800);
            var wardflashpos = Mathematics.GetWardFlashPositions(distance, Player, (byte)enemiescount, enemies);
            var wardJumpPos = Mathematics.MoveVector(Player.Position, wardflashpos);
            var enemies1 = HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player) < 1125).ToList();
            var getresults = Mathematics.GetPositions(Player, 1125, (byte)enemiescount, enemies1);
            var items = Items.GetWardSlot();
            if (getresults.Count > 1)
            {
                var getposition = Mathematics.SelectBest(getresults, Player);
                if (Player.Distance(getposition) < 600 && W.IsReady() && items != null)
                {
                    var pos = getposition;
                    foreach (var wards in ObjectManager.Get<Obj_AI_Base>())
                    {
                        if (!_processW2 && W.IsReady() && Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne" &&
                            Player.Spellbook.GetSpell(SpellSlot.Q).Name != "blindmonkwtwo"
                            && ((wards.Name.ToLower().Contains("ward") && wards.IsAlly)))
                        {
                            W.Cast(wards);
                            _lastcasted = Environment.TickCount;
                        }
                    }

                    var ward = Items.GetWardSlot();
                    if (W.IsReady() && ward != null && ward.IsValidSlot() &&
                        Environment.TickCount - _lastward > 400 &&
                        Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne"
                        )
                    {
                        Player.Spellbook.CastSpell(ward.SpellSlot, pos);
                        _lastward = Environment.TickCount;
                    }
                }
            }
            if (enemies1.FirstOrDefault() == null) return;
            if (Environment.TickCount - _lastcasted < 1000)
            {
                R.Cast(enemies1.FirstOrDefault());
            }

        }
    }
}
