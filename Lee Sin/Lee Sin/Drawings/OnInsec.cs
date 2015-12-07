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
        public static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (!GetBool("spellsdraw", typeof(bool))) return;
            if (!GetBool("targetexpos", typeof(bool))) return;
            if (!GetBool("ovdrawings", typeof(bool))) return;

            if (SelectedAllyAiMinion != null)
            {
                Render.Circle.DrawCircle(SelectedAllyAiMinion.Position, 200, Color.Blue, 2, true);
            }



            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
            }


            if (target == null || target.IsDead || !target.IsVisible) return;


            if (!GetBool("linebetween", typeof(bool))) return;
            var objAiHero = InsecPos.WardJumpInsecPosition.GetAllyHeroes(target, 1200).FirstOrDefault();
            if (SelectedAllyAiMinion == null)
            {
                if (objAiHero != null && GetBool("useobjectsallies", typeof(bool)))
                {
                    var pos11 = Drawing.WorldToScreen(target.Position);
                    var distance = target.Distance(objAiHero);
                    var pos22 = Drawing.WorldToScreen(target.Position.Extend(objAiHero.Position, distance));
                    Drawing.DrawLine(pos11, pos22, 1, Color.Red);
                    Render.Circle.DrawCircle(objAiHero.Position, 100, Color.Blue, 2, true);
                    Drawing.DrawText(pos22.X, pos22.Y, Color.Black, "X");
                }
                else
                {
                    var pos11 = Drawing.WorldToScreen(target.Position);
                    var distance = target.Distance(Player);
                    var pos22 = Drawing.WorldToScreen(target.Position.Extend(Player.Position, distance));
                    Drawing.DrawLine(pos11, pos22, 1, Color.Red);
                    Render.Circle.DrawCircle(Player.Position, 100, Color.Blue, 2, true);
                    Drawing.DrawText(pos22.X, pos22.Y, Color.Black, "X");
                }
            }

            if (SelectedAllyAiMinion != null)
            {
                var pos3 = Drawing.WorldToScreen(target.Position);
                var distance = target.Distance(SelectedAllyAiMinion);
                var pos4 = Drawing.WorldToScreen(target.Position.Extend(SelectedAllyAiMinion.Position, distance));
                Drawing.DrawLine(pos3, pos4, 3, Color.Red);
                Drawing.DrawText(pos4.X, pos4.Y, Color.Black, "X");
            }
        }
    }
}
