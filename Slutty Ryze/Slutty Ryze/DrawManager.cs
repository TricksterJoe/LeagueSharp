using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Slutty_ryze
{
    class DrawManager
    {
        #region Variable Declaration
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");
        private static readonly Color _color = Color.Lime;
        private static readonly Color _fillColor = Color.Goldenrod;
        #endregion
        #region Private Fuctions
        private static Color GetColor(bool b)
        {
            return b ? Color.DarkGreen : Color.Red;
        }

        private static string BoolToString(bool b)
        {
            return b ? "On" : "off";
        }
        /*
         static Point[] getPoints(Vector2 c, int R)
         {
             int X = (int)c.X;
             int Y = (int)c.Y;

             int R2 = (int)(R / Math.PI);

             Point[] pt = new Point[5];

             pt[0].X = X;
             pt[0].Y = Y;

             pt[1].X = X;
             pt[1].Y = Y + R * 2;

             pt[2].X = X + R * 2;
             pt[2].Y = Y;

             pt[3].X = X - R * 2;
             pt[3].Y = Y;

             pt[4].X = X;
             pt[4].Y = Y - R * 2;

             return pt;
         }

         private static void drawCircleThing(int radius, SharpDX.Vector2 center, Color c)
         {
             var lineWalls = getPoints(center,radius);
             Drawing.DrawLine(lineWalls[0], lineWalls[1], 2, c);
             Drawing.DrawLine(lineWalls[0], lineWalls[2], 2, c);
             Drawing.DrawLine(lineWalls[0], lineWalls[3], 2, c);
             Drawing.DrawLine(lineWalls[0], lineWalls[4], 2, c);
         }
         */
        #endregion
        #region Public Functions
        public static void Drawing_OnDraw(EventArgs args)
        {
            if (GlobalManager.GetHero.IsDead)
                return;
            if (!GlobalManager.Config.Item("Draw").GetValue<bool>())
                return;
            if (!GlobalManager.GetHero.Position.IsOnScreen())
                return;

            // drawCircleThing((int)Champion.Q.Range/2, Drawing.WorldToScreen(GlobalManager.GetHero.Position), Color.Pink);

            if (GlobalManager.Config.Item("qDraw").GetValue<bool>() && Champion.Q.Level > 0)
                Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.Q.Range, Color.Green);
            if (GlobalManager.Config.Item("eDraw").GetValue<bool>() && Champion.E.Level > 0)
                Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.E.Range, Color.Gold);
            if (GlobalManager.Config.Item("wDraw").GetValue<bool>() && Champion.W.Level > 0)
                Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.W.Range, Color.Black);

            var tears = GlobalManager.Config.Item("tearS").GetValue<KeyBind>().Active;
            var passive = GlobalManager.Config.Item("autoPassive").GetValue<KeyBind>().Active;

            var laneclear = !GlobalManager.Config.Item("disablelane").GetValue<KeyBind>().Active;
            //    var laneclear = !GlobalManager.Config.Item("disablelane").GetValue<KeyBind>().Active;
            //var showKeyBind = GlobalManager.Config.Item("keyBindDisplay").GetValue<KeyBind>().Active;

            if (!GlobalManager.Config.Item("notdraw").GetValue<bool>()) return;

            var heroPosition = Drawing.WorldToScreen(GlobalManager.GetHero.Position);
            var textDimension = Drawing.GetTextExtent("Stunnable!");

            Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                GetColor(tears),
                "Tear Stack: " + BoolToString(tears));

            Drawing.DrawText(heroPosition.X - 150, heroPosition.Y - 30, GetColor(passive),
                "Passive Stack: " + BoolToString(passive));

            Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30, GetColor(laneclear),
                "Lane Clear: " + BoolToString(laneclear));

           // if(!showKeyBind) return;

         //   Drawing.DrawText(heroPosition.X + 100, heroPosition.Y - 50, GetColor(showKeyBind),
             //  "Key:","Last Hit: {0}", GlobalManager.Config.Item("LastHit").GetValue<KeyBind>().Key);

        }

        public static void Drawing_OnDrawChamp(EventArgs args)
        {
            if (!GlobalManager.EnableDrawingDamage || GlobalManager.DamageToUnit == null)
            {
                return;
            }

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

                if (GlobalManager.EnableFillDamage)
                {
                    float differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);

                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
                    }
                }
            }
        }
        #endregion
    }
}
