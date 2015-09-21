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
    internal class Spells : Helper
    {
        public static void Spell()
        {
            Drawing.OnDraw += OnDraw;
         //   GameObject.OnCreate += OnCreate;
        }

        public class Polygon
        {
            public static List<Vector2> Points = new List<Vector2>();

            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            public static void DrawLineInWorld(Vector3 start, Vector3 end, float width, Color color)
            {
                var from = Drawing.WorldToScreen(start);
                var to = Drawing.WorldToScreen(end);
                Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
                //Drawing.DrawLine(from.X, from.Y, to.X, to.Y, width, color);
            }

            public static void Draw(Color color, float width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                    DrawLineInWorld(Points[i].To3D(), Points[nextIndex].To3D(), width, color);
                }
            }
        }

        public class Rectangle
        {
            public Vector2 Direction;
            public Vector2 Perpendicular;
            public Vector2 REnd;
            public Vector2 RStart;
            public float Width;

            public Rectangle(Vector2 start, Vector2 end, float width)
            {
                RStart = start;
                REnd = end;
                Width = width;
                Direction = (end - start).Normalized();
                Perpendicular = Direction.Perpendicular();

            }

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();

                result.Add(
                    RStart + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                result.Add(
                    RStart - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular - offset * Direction);
                result.Add(
                    REnd - (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);
                result.Add(
                    REnd + (overrideWidth > 0 ? overrideWidth : Width + offset) * Perpendicular + offset * Direction);

                return result;
            }
 
        }

        static void OnDraw(EventArgs args)
        {

            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            foreach (var spell in Player.Spellbook.Spells)
            {
                var pred = Prediction.GetPrediction(target, spell.SData.CastRange).CastPosition.To2D();
                var myRect = new Rectangle(Player.Position.To2D(), spell.,
     spell.SData.LineWidth);
                myRect.ToPolygon();
                Polygon.Draw(Color.AliceBlue);

            }

        }
    }
}
