using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using SharpDX;

namespace Slutty_ryze
{
    public static class FastDraw
    {
        static FastDraw()
        {
            Quality = 1.5f;
        }

        public static float Quality { get; set; }
        private static double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        private static IEnumerable<Vector2> DrawCircle2(double x, double y, double radius, int chordLength)
        {
            var quality2 = Math.Max(8, Math.Floor(180 / RadianToDegree((Math.Asin((chordLength / (2 * radius)))))));

            quality2 = Quality * 2 * Math.PI / quality2;
            radius = radius * .92;

            for (double theta = 0; theta < 2 * Math.PI + quality2; theta += quality2)
            {
                yield return (new Vector2((float)(x + radius * Math.Cos(theta)), (float)(y - radius * Math.Sin(theta))));
            }
        }

        private static IEnumerable<Vector2> DrawCircleNextLvl(float x, float y, float radius)
        {
            var k = new Vector2(x, y);

            return (from pt in DrawCircle2(k.X, k.Y, radius, 75)
                    select Drawing.WorldToScreen(new Vector3(pt.X, pt.Y, 0)));
        }

        public static void DrawCircle(float x, float y, float radius, float thickness, System.Drawing.Color color)
        {
            var pts = DrawCircleNextLvl(x, y, radius).GetEnumerator();

            pts.MoveNext();

            Vector2 huehue = pts.Current;

            while (pts.MoveNext())
            {
                Drawing.DrawLine(huehue, pts.Current, thickness, color);
                huehue = pts.Current;
            }
        }
    }
}
