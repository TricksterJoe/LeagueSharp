using System;
using System.Drawing;
using System.Runtime.Remoting.Channels;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    internal class JungleDraw : Helper
    {

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (!Player.Position.IsOnScreen())
                return;
            try
            {
                //If User does not want drawing
                if (GetBool("jungle.options.drawing.range", typeof (bool)))
                    Render.Circle.DrawCircle(Player.Position, 550, Color.Red, 5);

                if (GetBool("jungle.options.drawing.damage", typeof (bool)))
                {


                }
            }
            catch
            {
                // DUmb color picker
            }
        }

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        private static DamageToUnitDelegate _damageToMonster;

        public static DamageToUnitDelegate DamageToMonster
        {
            get { return _damageToMonster; }

            set
            {
                if (_damageToMonster == null)
                {
                    Drawing.OnDraw += Drawing_OnDrawMonster;
                }
                _damageToMonster = value;
            }
        }

        public static void Drawing_OnDrawMonster(EventArgs args)
        {
            try
            {
                if (!GetBool("jungle.options.drawing.damage", typeof (bool)) ||
                    DamageToMonster == null)
                    return;

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.Team != GameObjectTeam.Neutral || !minion.IsValidTarget() || !minion.IsHPBarRendered)
                        continue;

                    var damage = Smite.SmiteDamage(minion);

                    // Monster bar widths and offsets from ElSmite
                    var barWidth = 0;
                    var xOffset = 0;
                    var yOffset = 0;
                    var yOffset2 = 0;
                    var display = true;
                    switch (minion.CharData.BaseSkinName)
                    {
                        case "SRU_Red":
                        case "SRU_Blue":
                        case "SRU_Dragon":
                            barWidth = 145;
                            xOffset = 3;
                            yOffset = 18;
                            yOffset2 = 10;
                            break;

                        case "SRU_Baron":
                            barWidth = 194;
                            xOffset = -22;
                            yOffset = 13;
                            yOffset2 = 16;
                            break;

                        case "Sru_Crab":
                            barWidth = 61;
                            xOffset = 45;
                            yOffset = 34;
                            yOffset2 = 3;
                            break;

                        case "SRU_Krug":
                            barWidth = 81;
                            xOffset = 58;
                            yOffset = 18;
                            yOffset2 = 4;
                            break;

                        case "SRU_Gromp":
                            barWidth = 87;
                            xOffset = 62;
                            yOffset = 18;
                            yOffset2 = 4;
                            break;

                        case "SRU_Murkwolf":
                            barWidth = 75;
                            xOffset = 54;
                            yOffset = 19;
                            yOffset2 = 4;
                            break;

                        case "SRU_Razorbeak":
                            barWidth = 75;
                            xOffset = 54;
                            yOffset = 18;
                            yOffset2 = 4;
                            break;

                        default:
                            display = false;
                            break;
                    }
                    if (!display) continue;
                    var barPos = minion.HPBarPosition;
                    var percentHealthAfterDamage = Math.Max(0, minion.Health - damage)/minion.MaxHealth;
                    var yPos = barPos.Y + yOffset;
                    var xPosDamage = barPos.X + xOffset + barWidth*percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + xOffset + barWidth*minion.Health/minion.MaxHealth;

                    if (GetBool("jungle.options.drawing.damage.fill", typeof (bool)))
                    {
                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + xOffset;

                        for (var i = 0; i < differenceInHp; i++)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1, Color.White);
                        }
                    }
                    else
                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1, Color.White);

                    if (!(damage > minion.Health)) continue;
                    if (!GetBool("jungle.options.drawing.killable.text", typeof (bool))) return;
                    Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y, Color.Red, "Killable");
                }
            }
            catch
            {
                //Dumb color picker
            }
        }


    }
}

