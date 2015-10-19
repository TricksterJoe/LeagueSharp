//
// This file was created by HyunMi on 10/19/2015
//

using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Lee_Sin
{
    public static class Mathematics
    {
        // Ward position, flash position, position to be in when you have to cast r, if a parameter is not applicable eg flash pos without having flash than this param will be set to new Vector3(nullptr)
        // Func will return null when no position is found
        public static Tuple<Vector3, Vector3, Vector3> GetWardFlashPositions(bool canUseWard, bool canUseFlash, Obj_AI_Hero player, byte minHitRequirement, List<Obj_AI_Hero> enemies)
        {
            float maxTravelDistance = GetMaxTravelDistance(canUseWard, canUseFlash);
            Vector3 destination = SelectBest(GetPositions(player, maxTravelDistance, minHitRequirement, enemies),player);

            if (destination == new Vector3(null))
            {
                return null;
            }

            bool useWard, useFlash;

            if (maxTravelDistance == 187.5f)
            {
                useWard = false;
                useFlash = false;
            }
            else
            {
                if (player.Distance(destination) <= 600f)
                {
                    useWard = true;
                    useFlash = false;
                }
                else
                {
                    useWard = true;
                    useFlash = true;
                }
            }

            Vector3 wardPos = new Vector3(null), flashPos = new Vector3(null);

            if (useWard && useFlash)
            {
                wardPos = MoveVector(player.ServerPosition, destination, 600f);
                flashPos = destination;
            }

            if (useWard && !useFlash)
            {
                wardPos = destination;
            }

            return new Tuple<Vector3, Vector3, Vector3>(wardPos, flashPos, destination);
        }

        private static float GetMaxTravelDistance(bool canUseWard, bool canUseFlash)
        {
            if (canUseWard && canUseFlash)
            {
                return 1125f;
            }

            if (canUseWard)
            {
                return 600f;
            }

            if (canUseFlash)
            {
                return 425f;
            }

            return 187.5f;
        }

        //Lazy kappa
        private static Vector3 SelectBest(List<Vector3> getPositionsResults, Obj_AI_Hero player)
        {
            if (getPositionsResults.Count == 0)
            {
                return new Vector3(null);
            }

            if (player.ServerPosition.Distance(getPositionsResults[0]) < player.Distance(getPositionsResults[1]))
            {
                return getPositionsResults[0];
            }
            return getPositionsResults[1];
        }

        private static List<Vector3> GetPositions(Obj_AI_Hero player, float maxTravelDistance, byte minHitRequirement, List<Obj_AI_Hero> enemies)
        {
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
            Vector3 leePos = player.ServerPosition;
            List<Vector3> results = new List<Vector3>();
            foreach (Tuple<Geometry.Polygon.Rectangle, Vector3, Vector3> tuple in generatePositionsResult)
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
            Vector3 leePos = player.ServerPosition;
            foreach (Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>> tuple in minHitFilterResults)
            {
                tuple.Item3.OrderBy(e => e.Distance(leePos));
            }

            var results = new List<Tuple<Geometry.Polygon.Rectangle, Vector3, Vector3>>();
            foreach (Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>> tuple in minHitFilterResults)
            {
                Tuple<Vector3, Vector3> sres = SGeneratePosition(tuple.Item1, tuple.Item3.Last().ServerPosition, tuple.Item3.First().ServerPosition);//polygon, farthest eg last, closest eg first since we ordered them
                results.Add(new Tuple<Geometry.Polygon.Rectangle, Vector3, Vector3>(tuple.Item1, sres.Item1, sres.Item2));
            }
            return results;
        }

        private static Tuple<Vector3, Vector3> SGeneratePosition(Geometry.Polygon.Rectangle polygon, Vector3 fatherst, Vector3 closest)
        {
            Vector3 v0 = MoveVector(fatherst, closest, -187.5F);
            Vector3 v1 = MoveVector(closest, fatherst, -187.5F);
            return new Tuple<Vector3, Vector3>(v0, v1);
        }

        private static List<Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>> MinHitFilter(List<Geometry.Polygon.Rectangle> polygons, List<Obj_AI_Hero> enemies, byte minHit)
        {
            List<Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>> results = new List<Tuple<Geometry.Polygon.Rectangle, byte, List<Obj_AI_Hero>>>();
            foreach (Geometry.Polygon.Rectangle polygon in polygons)
            {
                byte count = 0x0;
                List<Obj_AI_Hero> inPoly = new List<Obj_AI_Hero>();
                foreach (Obj_AI_Hero tar in enemies)
                {
                    if (polygon.IsInside(tar))
                    {
                        count++;
                        inPoly.Add(tar);
                    }
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
            List<Geometry.Polygon.Rectangle> results = new List<Geometry.Polygon.Rectangle>();
            foreach (Geometry.Polygon.Rectangle rectangle in input)
            {
                if (!results.Contains(rectangle))
                {
                    results.Add(rectangle);
                }
            }
            return results;
        }

        private static List<Geometry.Polygon.Rectangle> GeneratePolygons(List<Obj_AI_Hero> enemies)
        {
            List<Geometry.Polygon.Rectangle> results = new List<Geometry.Polygon.Rectangle>();
            foreach (Obj_AI_Hero enemy in enemies)
            {
                Obj_AI_Hero tar = enemy;
                foreach (Obj_AI_Hero end in enemies.Where(e => e != tar))
                {
                    results.Add(
                        SGeneratePolygon(
                            tar.ServerPosition,
                            end.ServerPosition,
                            (tar.BoundingRadius + enemy.BoundingRadius) / 2));
                }
            }
            return results;
        }

        private static Geometry.Polygon.Rectangle SGeneratePolygon(Vector3 start, Vector3 end, float boundingBWidth)
        {
            Vector3 nstart = MoveVector(start, end);
            Vector3 nend = MoveVector(end, start);
            return new Geometry.Polygon.Rectangle(nstart, nend, boundingBWidth);
        }

        // Distance from start to end is t = 1
        private static Vector3 MoveVector(Vector3 start, Vector3 end, float distance = 2250F)
        {
            float t = distance / (start.Distance(end));
            Vector3 direction = new Vector3(end.X - start.X, end.Y - start.Y, end.Z - start.Z);
            float x = start.X + (direction.X * t);
            float y = start.Y + (direction.Y * t);
            float z = start.Z + (direction.Z * t);
            return new Vector3(x, y, z);
        }
    }
}