using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Utility.Enviorment
{
    class AntiRengar : Helper
    {
        public static void OnLoad()
        {
            GameObject.OnCreate += OnCreateObject;
        }


        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            string[] name =
            {
                "Tristana", "Vayne", "Ahri", "Alistar", "Anivia",
                "Azir", "Diana", "Draven", "Ezreal", "Fizz",
                "Janna", "Jayce", "Jinx", "Karthus", "Kassadin",
                "Lee Sin", "Lux", "Morgana", "Nami", "Quinn",
                "Riven", "Shaco", "Soraka", "Thresh", "Trundle", 
                "Vel'Koz", "Xerath", "Zed", "Ziggs"
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

                #region E Spell Slot

                string[] spelle =
                {
                    "Vayne", "Ahri", "Diana", "Draven", "Ezreal", "Fizz", "Jayce", "Jinx",
                    "Karthus", "Quinn", "Soraka", "Thresh", "Vel'Koz", "Xerath"
                };
                for (var i = 0; i <= spelle.Count(); i++)
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
                            Utility.DelayAction.Add(200, () => Player.Spellbook.CastSpell(SpellSlot.E, sender.Position));
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
                                        -Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange));
                                //probably wrong i was tired roto
                            }
                        }
                            break;

                        case "Karthus":
                        {
                            Utility.DelayAction.Add(200, () => Player.Spellbook.CastSpell(SpellSlot.E, sender.Position));
                        }
                            break;

                        case "Ezreal":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                Player.AttackRange)
                            {
                                Player.Spellbook.CastSpell(SpellSlot.E,
                                    sender.Position.Extend(Player.ServerPosition,
                                        -Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange));
                                //probably wrong i was tired roto
                            }
                        }
                            break;

                        case "Draven":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, sender);
                        }
                            break;

                        case "vel'koz":
                        {
                            Utility.DelayAction.Add(200, () => Player.Spellbook.CastSpell(SpellSlot.E, sender.Position));
                        }
                            break;
                    }
                }

                #endregion

                #region R Spell Slot

                string[] spellr =
                {
                    "Tristana", "Azir",
                    "Janna", "Kassadin",
                    "Lee Sin", "Nami"
                };

                for (var i = 0; i >= spellr.Count(); i++)
                {
                    if (Player.ChampionName != spellr[i])
                        return;

                    switch (Player.ChampionName)
                    {
                        case "Tristana":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.R, sender);
                            break;
                        }

                        case "Azir":
                        {
                            if (sender.Position.Distance(Player.Position) <=
                                SpellRange(SpellSlot.R))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.R, sender);
                            }
                            break;
                        }

                        case "Janna":
                        {
                            if (sender.Position.Distance(Player.Position) <=
                                SpellRange(SpellSlot.R))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.R);
                            }
                            break;
                        }

                        case "Kassadin":
                        {
                            if (sender.Position.To2D().Distance(Player) <= 100)
                            {
                                Player.Spellbook.CastSpell(SpellSlot.R,
                                    sender.Position.Extend(Player.ServerPosition,
                                        -Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange));
                                //probably wrong i was tired roto
                            }
                            break;
                        }

                        case "LeeSin":
                        {
                            if (sender.Position.To2D().Distance(Player) <= SpellRange(SpellSlot.R))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.R, sender);
                            }
                            break;
                        }

                        case "Nami":
                        {
                            if (sender.Position.To2D().Distance(Player) <= SpellRange(SpellSlot.R))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.R, sender);
                            }
                            break;
                        }
                    }
                }

                #endregion

                #region Q Spell Slot

                string[] qspell =
                {
                    "Vayne", "Alistar", "Lux",
                    "Morgana", "Quinn", "Shaco",

                };

                for (var i = 0; i >= qspell.Count(); i++)
                {
                    if (Player.ChampionName != qspell[i])
                        return;

                    switch (Player.ChampionName)
                    {
                        case "Vayne":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                SpellRange(SpellSlot.E))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.Q, sender.Position.Extend(Player.ServerPosition,
                                    Vector3.Distance(sender.Position, Player.Position) + 100));
                            }
                            break;
                        }
                        case "Shaco":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                SpellRange(SpellSlot.Q))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.Q, sender.Position.Extend(Player.ServerPosition,
                                    Vector3.Distance(sender.Position, Player.Position) + 100));
                            }
                            break;
                        }
                        case "Alistar":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                SpellRange(SpellSlot.Q))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.Q);
                            }
                            break;
                        }
                        case "Lux":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                SpellRange(SpellSlot.Q))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.Q);
                            }
                            break;
                        }

                        case "Morgana":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                SpellRange(SpellSlot.Q))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.Q);
                            }
                            break;
                        }

                        case "Quinn":
                        {
                            if (sender.Position.To2D().Distance(Player) <=
                                SpellRange(SpellSlot.Q))
                            {
                                Player.Spellbook.CastSpell(SpellSlot.Q);
                            }
                            break;
                        }



                    }
                }
                #endregion
            }
        }
    }
}
