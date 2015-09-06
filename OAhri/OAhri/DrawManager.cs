using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace OAhri
{
    internal class DrawManager : Ahri
    {
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");
        private static readonly Color _color = Color.Red;
        private static readonly Color _fillColor = Color.Blue;

        internal static void Drawing_OnDrawChamp(EventArgs args)
        {
            if (!Config.Item("FillDamage").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                 var barPos = unit.HPBarPosition;
                var damage = GlobalManager.DamageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width*unit.Health/unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int) barPos.X + XOffset;
                    Text.Y = (int) barPos.Y + YOffset - 13;
                    Text.text = "Killable With Combo Rotation " + (unit.Health - damage);
                    Text.OnEndScene();
                }
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

                if (Config.Item("RushDrawWDamageFill").GetValue<bool>())
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107*percentHealthAfterDamage);
                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
                    }
                }
            }
        }

        internal static void Drawing_OnDraw(EventArgs args)
        {

            if (Player.IsDead)
                return;

            var draw = Config.Item("Draw").GetValue<bool>();
            var drawq = Config.Item("qDraw").GetValue<bool>();
            var drawql = Config.Item("qlDraw").GetValue<bool>();
            var draww = Config.Item("wDraw").GetValue<bool>();
            var drawe = Config.Item("eDraw").GetValue<bool>();
            var drawr = Config.Item("rDraw").GetValue<bool>();
            if (!draw)
                return;

            var color = Color.FromArgb(55, 255, 255, 255);

            if (drawq && Q.Level >= 1 && Q.IsReady())
            {
                Drawing.DrawCircle(Player.Position, Q.Range, Color.GreenYellow);
            }

            if (draww && W.Level >= 1 && W.IsReady())
            {
                Drawing.DrawCircle(Player.Position, W.Range, Color.Aqua);
            }

            if (drawe && E.Level >= 1 && E.IsReady())
            {
                Drawing.DrawCircle(Player.Position, E.Range, Color.Brown);
            }
            
            if (drawr)
            {
                Drawing.DrawLine(Drawing.WorldToScreen(Player.ServerPosition),
                    Drawing.WorldToScreen(Player.ServerPosition.Extend(Game.CursorPos, 450)), 3,
                    Color.Blue);

                Drawing.DrawCircle(Player.ServerPosition.Extend(Game.CursorPos, 450), 100, Color.Red);
            }
             

            
            if (!drawql) return;
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            if (target.IsValidTarget(Q.Range) && QMissile != null)
            {
                Drawing.DrawLine(Drawing.WorldToScreen(Player.ServerPosition),
                    Drawing.WorldToScreen(QMissile.Position), Q.Width,
                    color);
            }            
        }
    }
}
