using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Utility.Enviorment
{
    class AntiRengar : Helper
    {
        public AntiRengar()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            GameObject.OnCreate += OnCreateObject;
        }


        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            string[] name =
            {
                "Tristana", "Vayne", "Ahri", "Alistar", "Anivia", "Azir", "Diana", "Draven", "Ezreal", "Fizz",
                "Janna", "Jayce", "Jinx", "Karthus", "Kassadin", "Lee Sin", "Lux", "Morgana", "Nami", "Quinn",
                "Riven", "Shaco", "Soraka", "Thresh", "Trundle", "Vel'Koz", "Xerath", "Zed", "Ziggs"
            };

            if (!sender.IsEnemy && sender.Name != "Rengar")
                return;
            if (sender.Name == "Rengar_LeapSound.troy")
            {
                for (var i = 0; i <= 30; i++)
                {
                    if (Player.ChampionName != name[i])
                        return;
                }

                string[] spelle =
                {
                    "Vayne", "Ahri", "Diana", "Draven", "Ezreal", "Fizz", "Jayce", "Jinx",
                    "Karthus", "Quinn", "Soraka", "Thresh", "Vel'Koz", "Xerath"
                };
                for (var i = 0; i <= 15; i++)
                {
                    if (Player.ChampionName != spelle[i])
                        return;

                    switch (Player.ChampionName)
                    {
                        case "Vayne":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, sender);
                        }
                            break;

                        case "Ahri":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, sender);
                        }
                            break;

                        case "Diana":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange)
                                Player.Spellbook.CastSpell(SpellSlot.E);
                        }
                            break;

                        case "Jinx":
                        {
                            Utility.DelayAction.Add(300, () => Player.Spellbook.CastSpell(SpellSlot.E, Player.Position));
                        }
                            break;

                        case "soraka":
                        {
                            Utility.DelayAction.Add(200, () => Player.Spellbook.CastSpell(SpellSlot.E, Player.Position));
                        }
                            break;

                        case "Xerath":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, sender);
                        }
                            break;

                        case "Quinn":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange)
                                Player.Spellbook.CastSpell(SpellSlot.E, sender);
                        }
                            break;

                        case "Thresh":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange - 20)
                            {
                                Player.Spellbook.CastSpell(SpellSlot.E, sender.Position.Extend(Player.ServerPosition,
                                    Vector3.Distance(sender.Position, Player.Position) + 100));
                            }
                        }
                            break;

                        case "Jayce":
                        {
                            if (Player.IsMelee())
                            {
                                if (sender.Position.To2D().Distance(Player) <=
                                    Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange)
                                {
                                    Player.Spellbook.CastSpell(SpellSlot.E, sender);
                                }
                            }
                        }
                            break;

                        case "Fizz":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                Player.AttackRange)
                            {
                                Player.Spellbook.CastSpell(SpellSlot.E,
                                    sender.Position.Extend(Player.ServerPosition,
                                        -Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange)); //probably wrong i was tired roto
                            }
                        }
                    }
                }
            }
        }
    }
}
