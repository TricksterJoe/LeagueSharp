using System;
using LeagueSharp;
using LeagueSharp.Common;
//using Color = System.Drawing.Color;
namespace Slutty_Utility.Jungle
{
    class JungleDisplay : Helper
    {
        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);
        private static DamageToUnitDelegate  _damageToMonster;

        public static DamageToUnitDelegate DamageToMonster
        {
            get { return _damageToMonster; }

            set
            {
                if (_damageToMonster == null)
                {
                    LeagueSharp.Drawing.OnDraw += Drawing_OnDrawMonster;
                }
                _damageToMonster = value;
            }
        }

        public static void Drawing_OnDrawMonster(EventArgs args)
        {
            try
            {
                if (!Config.Item("cDrawOnMonsters").GetValue<Circle>().Active ||
                   DamageToMonster == null)
                    return;

                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.Team != GameObjectTeam.Neutral || !minion.IsValidTarget() || !minion.IsHPBarRendered)
                        continue;

                    var smiteDamage = Smite.SmiteDamage(minion);

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
                    var percentHealthAfterDamage = Math.Max(0, minion.Health - smiteDamage) / minion.MaxHealth;
                    var yPos = barPos.Y + yOffset;
                    var xPosDamage = barPos.X + xOffset + barWidth * percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + xOffset + barWidth * minion.Health / minion.MaxHealth;

                    if (Config.Item("cFillMonster").GetValue<Circle>().Active)
                    {
                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + xOffset;

                        for (var i = 0; i < differenceInHp; i++)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1,
                                Config.Item("cFillMonster").GetValue<Circle>().Color);
                        }
                    }
                    else
                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1,
                            Config.Item("cDrawOnMonsters").GetValue<Circle>().Color);

                    if (!(smiteDamage > minion.Health)) continue;
                    if (!Config.Item("cKillableText").GetValue<Circle>().Active) return;

                    Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y,
                        Config.Item("cKillableText").GetValue<Circle>().Color, "Killable");
                }
            }
            catch
            {
                //Dumb color picker
            }
        }
    }
}
