using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace OAnnie
{
    internal class DrawManager : Annie
    {
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, Color.Red, "monospace");
        private static readonly System.Drawing.Color _color = System.Drawing.Color.Red;

        private static readonly System.Drawing.Color _fillColor = System.Drawing.Color.Blue;
        internal static void Drawing_OnDrawChamp(EventArgs args)
        {
            
            if (!Config.Item("FillDamage").GetValue<bool>())
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

        internal static void OnDraw(EventArgs args)
        {
            var draw = Config.Item("Draw").GetValue<bool>();
            var useq = Config.Item("qDraw").GetValue<bool>();
            var usew = Config.Item("wDraw").GetValue<bool>();
            var user = Config.Item("rDraw").GetValue<bool>();
            var userf = Config.Item("rfDraw").GetValue<bool>();

            if (!draw)
                return;

            if (useq && Q.IsReady() && Q.Level > 0)
            {
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.CadetBlue);
            }

            if (usew && W.IsReady() && W.Level > 0)
            {
                Drawing.DrawCircle(Player.Position, W.Range, System.Drawing.Color.DarkGoldenrod);
            }

            if (user && R.IsReady() && R.Level > 0)
            {
                Drawing.DrawCircle(Player.Position, R.Range, System.Drawing.Color.Red);
            }

            if (userf && R.IsReady() && R.Level > 0 && FlashSlot.IsReady())
            {
                var heroPosition = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(heroPosition.X, heroPosition.Y,
                    System.Drawing.Color.Blue, "Can Flash Q!");
                Drawing.DrawCircle(Player.Position, R.Range + FlashRange, System.Drawing.Color.Blue);
            }
        }
    }
}