using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;


namespace ODarius
{
    class DrawingManager : Darius
    {
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");
        private static readonly Color _color = Color.Red;
        private static readonly Color _fillColor = Color.Blue;

        internal static void Drawing_OnDraw(EventArgs args)
        {
            var draw = Config.Item("Draw").GetValue<bool>();
            var drawq = Config.Item("qDraw").GetValue<bool>();
            var drawr = Config.Item("rDraw").GetValue<bool>();
            var drawe = Config.Item("eDraw").GetValue<bool>();
            if (!draw)
                return;
            if (!Player.Position.IsOnScreen())
                return;

            if (drawq && Q.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, Q.Range, Color.Blue);
            }

            if (drawe && E.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, E.Range, Color.Brown);
            }

            if (drawr && R.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, R.Range, Color.Red);
            }
        }

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
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable With Combo Rotation " + (unit.Health - damage);
                    Text.OnEndScene();
                }

                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

                if (Config.Item("RushDrawWDamageFill").GetValue<bool>())
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);
                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
                    }
                }
            }
        }
    }
}
