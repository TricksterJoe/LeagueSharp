using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    class JungleDisplay
    {
        public static void Drawing_OnDrawMonster(EventArgs args)
        {
            try
            {
                if (!Properties.MainMenu.Item("cDrawOnMonsters").GetValue<Circle>().Active ||
                    Properties.Drawing.DamageToMonster == null)
                    return;

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.Team != GameObjectTeam.Neutral || !minion.IsValidTarget() || !minion.IsHPBarRendered)
                        continue;

                    var rendDamage = Smite.smiteSpell.GetDamage(minion);

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
                    var percentHealthAfterDamage = Math.Max(0, minion.Health - rendDamage) / minion.MaxHealth;
                    var yPos = barPos.Y + yOffset;
                    var xPosDamage = barPos.X + xOffset + barWidth * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + xOffset + barWidth * minion.Health / minion.MaxHealth;

                    if (Properties.MainMenu.Item("cFillMonster").GetValue<Circle>().Active)
                    {
                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + xOffset;

                        for (var i = 0; i < differenceInHp; i++)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1,
                                Properties.MainMenu.Item("cFillMonster").GetValue<Circle>().Color);
                        }
                    }
                    else
                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1,
                            Properties.MainMenu.Item("cDrawOnMonsters").GetValue<Circle>().Color);

                    if (!(rendDamage > minion.Health)) continue;
                    if (!Properties.MainMenu.Item("cKillableText").GetValue<Circle>().Active) return;

                    Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y,
                        Properties.MainMenu.Item("cKillableText").GetValue<Circle>().Color, "Killable");
                }
            }
            catch
            {
                //Dumb color picker
            }
        }
    }
}
