using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lee_Sin.Drawings 
{
    class OnJungle : LeeSin
    {
        public static DamageToUnitDelegate _damageToMonster;

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        public static DamageToUnitDelegate DamageToMonster
        {
            get { return _damageToMonster; }

            set
            {
                if (_damageToMonster == null)
                {
                    Drawing.OnDraw += OnCamps;
                }
                _damageToMonster = value;

            }
        }

        public static void OnCamps(EventArgs args)
        {
            if (!GetBool("jungledraws", typeof(bool))) return;
            if (!GetBool("ovdrawings", typeof(bool))) return;
            if (GetBool("enabledisablesmite", typeof(bool)))
            {
                var color1 = GetBool("smiteenable", typeof(KeyBind)) ? Color.LimeGreen : Color.Black;
                var text = GetBool("smiteenable", typeof(KeyBind)) ? "Smite Enabled!" : "Smite Disabled!";
                Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X - 20, Drawing.WorldToScreen(Player.Position).Y - 20, color1, text);
            }

            if (GetBool("jungledraw", typeof(bool)))
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.Team == GameObjectTeam.Neutral && minion.IsValidTarget() && minion.IsHPBarRendered)
                    {
                        var smiteDamage = ActiveModes.Smite.SmiteDamages(minion);

                        // Monster bar widths and offsets from ElSmite
                        var barWidth = 0;
                        var xOffset = 0;
                        var yOffset = 0;
                        var yOffset2 = 0;
                        var display = true;
                        string name = "";
                        switch (minion.CharData.BaseSkinName)
                        {
                            case "SRU_Red":
                                barWidth = 145;
                                xOffset = 3;
                                yOffset = 18;
                                yOffset2 = 10;
                                name = "Red Buff";
                                break;

                            case "SRU_Blue":
                                barWidth = 145;
                                xOffset = 3;
                                yOffset = 18;
                                yOffset2 = 10;
                                name = "Blue Buff";
                                break;

                            case "SRU_Dragon":
                                barWidth = 145;
                                xOffset = 3;
                                yOffset = 18;
                                yOffset2 = 10;
                                name = "Dragon";
                                break;

                            case "SRU_Baron":
                                barWidth = 194;
                                xOffset = -22;
                                yOffset = 13;
                                yOffset2 = 16;
                                name = "Baron";
                                break;

                            case "Sru_Crab":
                                barWidth = 61;
                                xOffset = 45;
                                yOffset = 34;
                                yOffset2 = 3;
                                name = "Crab";
                                break;

                            case "SRU_Krug":
                                barWidth = 81;
                                xOffset = 58;
                                yOffset = 18;
                                yOffset2 = 4;
                                name = "Krug";
                                break;

                            case "SRU_Gromp":
                                barWidth = 87;
                                xOffset = 62;
                                yOffset = 18;
                                yOffset2 = 4;
                                name = "Gromp";
                                break;

                            case "SRU_Murkwolf":
                                barWidth = 75;
                                xOffset = 54;
                                yOffset = 19;
                                yOffset2 = 4;
                                name = "Murkwolf";
                                break;

                            case "SRU_Razorbeak":
                                barWidth = 75;
                                xOffset = 54;
                                yOffset = 18;
                                yOffset2 = 4;
                                name = "Razorbeak";
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

                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + xOffset;

                        for (var i = 0; i < differenceInHp; i++)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1, Color.OrangeRed);
                        }

                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1, Color.Red);
                        Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y, Color.Red, name);
                        if (GetBool("killmob", typeof(bool)))
                        {
                            if (smiteDamage >= minion.Health)
                            {
                                Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y, Color.Red, "Killable");
                            }
                        }
                    }
                }
            }
        }
    }
}
