using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Lee_Sin.Drawings
{
    class OnInsec : LeeSin
    {
        public static Vector2 RotateByX(Vector2 point1, Vector2 point2, float Angle)
        {
            var angle = Angle * Math.PI/180;
            var line = Vector2.Subtract(point2, point1);

            var newline = new Vector2
            {
                X = (float) (line.X * Math.Cos(angle) - line.Y*Math.Sin(angle)),
                Y = (float) (line.X*Math.Sin(angle) + line.Y*Math.Cos(angle))
            };

            return Vector2.Add(newline, point1);
        }

        public static void DrawArrow(Vector3 starPosition, Vector3 endPosition, int angle, int linelength, int arrowsize, Color arrowColor)
        {
            var playerPositionExtend = starPosition.Extend(endPosition, linelength);
            var extendPlayerPos = Drawing.WorldToScreen(playerPositionExtend);
            var afterStartPosition = playerPositionExtend.Extend(starPosition,
                playerPositionExtend.Distance(starPosition) - 110);
            var starPos = Drawing.WorldToScreen(afterStartPosition);
            Drawing.DrawLine(starPos, extendPlayerPos, 1, arrowColor);
            var playerposextend = playerPositionExtend.Extend(starPosition, -130);
            var firstLineRotate = RotateByX(playerposextend.To2D(), starPosition.To2D(), angle);
            var secondLineRotate = RotateByX(playerposextend.To2D(), starPosition.To2D(), -angle);
            var extend1 = playerposextend.Extend(firstLineRotate.To3D(), arrowsize);
            var extend2 = playerposextend.Extend(secondLineRotate.To3D(), arrowsize);
            var extendpoint = Drawing.WorldToScreen(playerposextend);
            var firstLineRotatePos = Drawing.WorldToScreen(extend1);
            var secondLineRotatePos = Drawing.WorldToScreen(extend2);
           Drawing.DrawLine(extendpoint, firstLineRotatePos, 1, arrowColor);
            Drawing.DrawLine(extendpoint, secondLineRotatePos, 1, arrowColor);
        }

        public static void OnDraw(EventArgs args)
        {
           

            if (Player.IsDead) return;
            if (!GetBool("spellsdraw", typeof(bool))) return;
            if (!GetBool("targetexpos", typeof(bool))) return;
            if (!GetBool("ovdrawings", typeof(bool))) return;
            if (Player.Level < 6) return;
            if (!R.IsReady()) return;

            if (SelectedAllyAiMinion != null)
            {
                Render.Circle.DrawCircle(SelectedAllyAiMinion.Position, 140, Color.LightBlue, 1, true);
            }
            
            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
            }


            if (target == null || target.IsDead || !target.IsVisible) return;
            
            
            var objAiHero = InsecPos.WardJumpInsecPosition.GetAllyHeroes(target, 1200).FirstOrDefault();
            if (SelectedAllyAiMinion == null)
            {
                if (objAiHero != null && GetBool("useobjectsallies", typeof(bool)))
                {
                    var pos11 = Drawing.WorldToScreen(target.Position);
                    var distance = target.Distance(objAiHero);
                   // var pos22 = Drawing.WorldToScreen(target.Position.Extend(objAiHero.Position, distance));
                    var pos22 = Drawing.WorldToScreen(objAiHero.Position);
                   // Drawing.DrawLine(pos11, pos22, 1, Color.Red);
                    Render.Circle.DrawCircle(objAiHero.Position, 140, Color.LightBlue, 2, true);
                    Drawing.DrawText(pos22.X - 25, pos22.Y + 25, Color.LightBlue, "Position");
                    DrawArrow(target.Position, objAiHero.Position, 30, 500, 200, Color.LightBlue);

                }
                else
                {
                    var pos11 = Drawing.WorldToScreen(target.Position);
                    var distance = target.Distance(Player);
                   // var pos22 = Drawing.WorldToScreen(target.Position.Extend(Player.Position, distance));
                    var pos22 = Drawing.WorldToScreen(Player.Position);
                    Render.Circle.DrawCircle(Player.Position, 140, Color.LightBlue, 2, true);
                    Drawing.DrawText(pos22.X - 25, pos22.Y + 25, Color.LightBlue, "Position");
                    DrawArrow(target.Position, Player.Position, 30, 500, 200, Color.LightBlue);
                }
            }

            if (SelectedAllyAiMinion != null)
            {
                var distance = target.Distance(SelectedAllyAiMinion);
              //  var pos4 = Drawing.WorldToScreen(target.Position.Extend(SelectedAllyAiMinion.ServerPosition, distance));
                var pos4 = Drawing.WorldToScreen(SelectedAllyAiMinion.Position);
                Drawing.DrawText(pos4.X - 25    , pos4.Y + 25, Color.LightBlue, "Position");
                DrawArrow(target.Position, SelectedAllyAiMinion.Position, 30, 500, 200, Color.LightBlue);
            }
        }
    }
}
