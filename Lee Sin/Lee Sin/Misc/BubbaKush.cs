using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Lee_Sin.WardManager;
using SharpDX;
using Color = System.Drawing.Color;

namespace Lee_Sin.Misc
{
    /// <summary>
    /// The static mathematics class for lee sin x on n enemies hit.
    /// </summary>
    class BubbaKush : LeeSin
    {
        public static int lastthingy;


        public static void DrawRect()
        {


            if (GetBool("activatebubba", typeof (KeyBind)))
            {
                var getresults = BubbaKush.GetPositions(Player, 600, (byte) GetValue("enemiescount"),
                    HeroManager.Enemies.Where(x => x.Distance(Player) < 1200).ToList());
                if (getresults.Count > 1)
                {

                    var ward = Items.GetWardSlot();
                    var getposition = BubbaKush.SelectBest(getresults, Player);
                    Render.Circle.DrawCircle(getposition, 100, Color.Red, 3, true);
                    var heros =
                        HeroManager.Enemies.Where(x => x.Distance(Player) < 400).OrderBy(x => x.Distance(Player));
                    if (LeeSin.HasFlash() && R.IsReady())
                    {
                        //if (Player.Distance(getposition) < 400 && Player.Distance(getposition) > R.Range)
                        //{
                        //    //  WardJump.WardJumped(getposition, false);
                        //    var poss = getposition;

                        //    Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"), poss, true);
                        //    lastthingy = Environment.TickCount;
                        //}
                        if (Player.Distance(getposition) < 400 && Player.Distance(getposition) < R.Range &&
                            heros.FirstOrDefault() != null)
                        {
                            R.Cast(heros.FirstOrDefault());
                            lastthingy = Environment.TickCount;
                        }
                    }
                    if (Player.Distance(getposition) < 30 && heros.FirstOrDefault() != null && R.IsReady())
                    {
                        R.Cast(heros.FirstOrDefault());
                    }
                }
            }

        }

        //public static int TravelDistance()
        //{
        //    var travel = 0;
        //    if (HasFlash())
        //    {
        //        travel += 400;
        //    }
        //    var
        //    if ()
        //}

        /// <summary>
        /// Gets the best possible destination where the max amount of enemies will be hit.
        /// </summary>
        /// <param name="maxTravelDistance">
        /// The max travel distance of lee sin.
        /// </param>
        /// <param name="player">
        /// The player obj.
        /// </param>
        /// <param name="minHitRequirement">
        /// The min hit requirement, min amount of enemies to be hit.
        /// </param>
        /// <param name="enemies">
        /// The enemies.
        /// </param>
        /// <returns>
        /// The <see cref="Vector3"/>, position that is where lee should be to cast r, eg the destination.
        /// </returns>
        public static
            Vector3 GetWardFlashPositions(float maxTravelDistance, Obj_AI_Hero player, byte minHitRequirement, List<Obj_AI_Hero> enemies)
        {
            var destination = SelectBest(GetPositions(player, maxTravelDistance, minHitRequirement, enemies), player);
            return destination;
        }

        // Lazy kappa
        public static Vector3 SelectBest(List<Vector3> getPositionsResults, Obj_AI_Hero player)
        {
            if (getPositionsResults.Count <= 0)
            {
                return new Vector3(null);
            }

            return getPositionsResults[0];
        }

        public static List<Vector3> GetPositions(Obj_AI_Hero player, float maxTravelDistance, byte minHitRequirement, List<Obj_AI_Hero> enemies)
        {
            if (enemies.Count <= 0) return new List<Vector3>(0); 
            var polygons = GeneratePolygons(enemies);
            var removedDuplicates = RemoveDuplicates(polygons);
            var minHitFiltered = MinHitFilter(removedDuplicates, enemies, minHitRequirement);
            var positions = GeneratePositions(minHitFiltered, player);
            var travelRangeFilter = TravelRangeFilter(positions, maxTravelDistance, player);
            return travelRangeFilter;
        }
            
        // Best position order is maintained
        private static List<Vector3> TravelRangeFilter(List<Tuple<Geometry.Polygon.Rectangle, Vector3, Vector3>> generatePositionsResult, float maxTravelDistance, Obj_AI_Hero player)
        {
            var leePos = player.ServerPosition;
            var results = new List<Vector3>();
            foreach (var tuple in generatePositionsResult)
            {
                if (leePos.Distance(tuple.Item2) <= maxTravelDistance)
                {
                    results.Add(tuple.Item2);
                }

                if (leePos.Distance(tuple.Item3) <= maxTravelDistance)
                {
                    results.Add(tuple.Item3);
                }
            }
            return results;
        }

        private static List<Tuple<Geometry.Polygon.Rectangle, Vector3, Vector3>> GeneratePositions(List<Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>> minHitFilterResults, Obj_AI_Hero player)
        {
            var leePos = player.ServerPosition;
            foreach (var tuple in minHitFilterResults)
            {
                tuple.Item3.OrderBy(e => e.Distance(leePos));
            }

            return (from tuple in minHitFilterResults
                    let sres =
                        SGeneratePosition(tuple.Item1, tuple.Item3.Last().ServerPosition, tuple.Item3.First().ServerPosition)
                    select new Tuple<Geometry.Polygon.Rectangle, Vector3, Vector3>(tuple.Item1, sres.Item1, sres.Item2))
                .ToList();
        }

        private static Tuple<Vector3, Vector3> SGeneratePosition(Geometry.Polygon.Rectangle polygon, Vector3 fatherst, Vector3 closest)
        {
            var v0 = MoveVector(fatherst, closest, -187.5F);
            var v1 = MoveVector(closest, fatherst, -187.5F);
            return new Tuple<Vector3, Vector3>(v0, v1);
        }

        private static List<Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>> MinHitFilter(List<Geometry.Polygon.Rectangle> polygons, List<Obj_AI_Hero> enemies, byte minHit)
        {
            var results = new List<Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>>();
            foreach (var polygon in polygons)
            {
                byte count = 0x0;
                var inPoly = new List<Obj_AI_Hero>();
                foreach (var tar in enemies.Where(tar => polygon.IsInside(tar)))
                {
                    count++;
                    inPoly.Add(tar);
                }
                if (count >= minHit)
                {
                    results.Add(new Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>(polygon, count, inPoly));
                }
            }
            return results.OrderBy(i => i.Item2).ToList();
        }

        private static List<Geometry.Polygon.Rectangle> RemoveDuplicates(List<Geometry.Polygon.Rectangle> input)
        {
            var results = new List<Geometry.Polygon.Rectangle>();
            foreach (var rectangle in input.Where(rectangle => !results.Contains(rectangle)))
            {
                results.Add(rectangle);
            }
            return results;
        }

        private static List<Geometry.Polygon.Rectangle> GeneratePolygons(List<Obj_AI_Hero> enemies)
        {
            List<Geometry.Polygon.Rectangle> list = new List<Geometry.Polygon.Rectangle>();
            foreach (var enemy in enemies)
            {
                foreach (var end in enemies.Where(e => e != enemy && !e.IsDead))
                    list.Add(SGeneratePolygon(enemy.ServerPosition, end.ServerPosition, (enemy.BoundingRadius + end.BoundingRadius)/2));
            }
            return list;
        }

        private static Geometry.Polygon.Rectangle SGeneratePolygon(Vector3 start, Vector3 end, float boundingBWidth)
        {
            var nstart = MoveVector(start, end);
            var nend = MoveVector(end, start);
            return new Geometry.Polygon.Rectangle(nstart, nend, boundingBWidth);
        }

        // Distance from start to end is t = 1
        public static Vector3 MoveVector(Vector3 start, Vector3 end, float distance = 2250F)
        {
            var t = distance / (start.Distance(end));
            var direction = new Vector3(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
            var x = start.X + (direction.X * t);
            var y = start.Y + (direction.Y * t);
            var z = start.Z + (direction.Z * t);
            return new Vector3(x, y, z);
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
    }
}