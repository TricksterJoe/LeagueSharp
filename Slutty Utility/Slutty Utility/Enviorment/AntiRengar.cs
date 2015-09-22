using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Utility.Enviorment
{
    //todo rewrite this roto
    //struct

    internal class AntiRengar : Helper
    {
        private static Obj_AI_Hero _rengo;
        private static bool ecasted;
        private static bool qcasted;
        private static bool casted;
        private static bool qscasted;

        public static void OnLoad()
        {
            GameObject.OnCreate += OnCreateObject;

        }

        public static void backwardscast(SpellSlot name, GameObject sender)
        {
            Player.Spellbook.CastSpell(name,
                sender.Position.Extend(Player.ServerPosition,
                    Player.Spellbook.GetSpell(name).SData.CastRange));
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
            if (!sender.IsAlly)
                return;
            if (name.Any(hero => Player.ChampionName != hero))
            {
                return;
            }

            if (sender.Name == "Rengar_LeapSound.troy")
            {

                foreach (var enemy in
                    HeroManager.Enemies.Where(hero => hero.IsValidTarget(1500) && hero.ChampionName == "Rengar"))
                {
                    _rengo = enemy;
                }

                    #region E Spell Slot

                    /*
                string[] spelle =
                {
                    "Vayne", "Ahri", "Diana", "Draven", "Ezreal", "Fizz", "Jayce", "Jinx",
                    "Karthus", "Quinn", "Soraka", "Thresh", "Vel'Koz", "Xerath"
                };
                 */

                    switch (Player.ChampionName)
                    {
                        case "Vayne":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, _rengo);
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
                            if (sender.Position.To2D().Distance(Player) <=
                                Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange)
                                Player.Spellbook.CastSpell(SpellSlot.E);
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
                            Player.Spellbook.CastSpell(SpellSlot.R, sender.Position);

                            Player.Spellbook.CastSpell(SpellSlot.E,
                                sender.Position.Extend(Player.ServerPosition,
                                    Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange));

                        }
                            break;

                        case "Karthus":
                        {
                            Utility.DelayAction.Add(200, () => Player.Spellbook.CastSpell(SpellSlot.E, sender.Position));
                        }
                            break;

                        case "Ezreal":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E,
                                Player.Position.Extend(sender.Position,
                                    Player.Spellbook.GetSpell(SpellSlot.E).SData.CastRange));
                        }
                            break;

                        case "Draven":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, sender);
                        }
                            break;

                        case "vel'koz":
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, sender.Position);
                        }
                            break;
                    }
                }

                #endregion

                #region R Spell Slot


            switch (Player.ChampionName)
            {
                case "Tristana":
                {
                    /*
                    Player.Spellbook.CastSpell(SpellSlot.W,
                        sender.Position.Extend(Player.ServerPosition,
                            Player.Spellbook.GetSpell(SpellSlot.W).SData.CastRange));
                     */ // figure out proper trist logic
                    Player.Spellbook.CastSpell(SpellSlot.R, _rengo);
                    break;
                }

                case "Azir":
                {
                    /*
                    if (sender.Position.Distance(Player.Position) <= SpellRange(SpellSlot.R))
                        Player.Spellbook.CastSpell(SpellSlot.R, sender.Position);
                     */ // wtf broken gg
                    break;
                }

                case "Janna":
                {
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).IsReady())
                    {
                        Player.Spellbook.CastSpell(SpellSlot.Q, sender.Position);
                        {
                            lastq = Environment.TickCount;
                        }
                    }

                    if (sender.Position.Distance(Player.Position) <=
                        SpellRange(SpellSlot.R)
                        && Environment.TickCount - lastq >= 1000
                        && !Player.Spellbook.GetSpell(SpellSlot.Q).IsReady())
                    {
                        Player.Spellbook.CastSpell(SpellSlot.R);
                    }
                    break;
                }

                case "Kassadin":
                {
                    Player.Spellbook.CastSpell(SpellSlot.R,
                        sender.Position.Extend(Player.ServerPosition,
                            Player.Spellbook.GetSpell(SpellSlot.R).SData.CastRange));
                    break;

                }

                case "LeeSin":
                {
                    Player.Spellbook.CastSpell(SpellSlot.R, _rengo);
                    break;
                }

                case "Nami":
                {
                    if (sender.Position.To2D().Distance(Player) <= SpellRange(SpellSlot.R))
                    {
                        Player.Spellbook.CastSpell(SpellSlot.R, sender.Position);
                    }
                    break;
                }
            }



            #endregion

            #region Q Spell Slot



            switch (Player.ChampionName)
            {
                case "Vayne":
                {
                    if (GetBool("userantirengar", typeof(bool)))
                    {
                        Player.Spellbook.CastSpell(SpellSlot.R);
                    }
                    if (_rengo.Distance(Player) < 300)
                    {
                        qscasted = true;
                        Utility.DelayAction.Add(150 + Game.Ping, () => backwardscast(SpellSlot.Q, sender)); // yay
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

            #endregion
        }

        public static int lastq { get; set; }
    }
}
