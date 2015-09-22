using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    internal class Smite : Helper
    {
        public static Spell smite;
        public static SpellSlot _smiteSlot;
        private static float SmiteTick { get; set; }

        private static readonly Dictionary<String, ExternalSpell> NumNumChamps = new Dictionary<String, ExternalSpell>();
        private static SpellSlot smiteSlot;

        public static void OnLoad()
        {
            GetSmiteSlot(ref _smiteSlot);

            Drawing.OnDraw += OnUpdate;
            JungleDraw.DamageToMonster = SmiteDamage;
            Drawing.OnDraw += JungleDraw.Drawing_OnDrawMonster;
            Drawing.OnDraw += JungleDraw.Drawing_OnDraw;
            GameObject.OnCreate += JungleDraw.OnCreate;
            GameObject.OnCreate += Timer.OnCreate;
            GameObject.OnDelete += Timer.OnDelete;
            Game.OnUpdate += Timer.OnUpdate;
        }

        private struct ExternalSpell
        {
            public readonly SpellSlot _SpellSlot;
            private static float Range;

            public ExternalSpell(SpellSlot spellSlot, float range)
            {
                Range = range;
                _SpellSlot = spellSlot;
            }
        }


        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (!GetBool("jungle.options.autoSmite", typeof(bool))) return;


                if (GetBool("jungle.options.smiteBuffs", typeof(bool)))
                CheckBuffs();


                if (GetBool("jungle.options.smiteEpic", typeof(bool)))
                    CheckEpics();
                
                
                if (!NumNumChamps.ContainsKey("Nunu"))
                    LoadNumNum();
                    
                

            }
            catch
            {
                // Just used so we dont have to use goto LOL
            }
                //After try block
            finally
            {
                SmiteCheck();
            }
        }

        private static void LoadNumNum()
        {
            NumNumChamps.Add("Nunu", new ExternalSpell(SpellSlot.Q, 300));
            NumNumChamps.Add("Cho'Gath", new ExternalSpell(SpellSlot.R, 175));
        }

        private static void CheckEpics()
        {
            foreach (
                var monster in
                    MinionManager.GetMinions(Player.ServerPosition, 500, MinionTypes.All,
                        MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
            {
                if (monster == null) return;
                if (!monster.Name.Contains("Baron") && !monster.Name.Contains("Dragon")) continue;
                if (!(SmiteDamage(monster) > monster.Health))
                {
                    if (SmiteDamage(monster) > monster.Health)
                    {
                        Player.Spellbook.CastSpell(_smiteSlot, monster);
                   }

                }
            }

        }

        private static void CheckBuffs()
        {
            foreach (var monster in MinionManager.GetMinions(Player.ServerPosition, 500,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth))
            {
                if (!monster.CharData.BaseSkinName.Equals("SRU_Red") &&
                     !monster.CharData.BaseSkinName.Equals("SRU_Blue"))
                    continue;
                if (SmiteDamage(monster) > monster.Health)
                {
                    if (GetBool("usenunuq", typeof (bool)))
                    {
                        Player.Spellbook.CastSpell(NumNumChamps[Player.ChampionName]._SpellSlot, monster);
                    }
                    Player.Spellbook.CastSpell(_smiteSlot, monster);
                }
            }
        }

        public static void SmiteCheck()
        {
            SmiteTick = TickCount + 300;
        }

        private static float GetFuckingSmiteDamage()
        {
            var level = Player.Level;
            var index = Player.Level/5;
            float[] dmgs =
            {
                370 + 20*level,
                330 + 30*level,
                240 + 40*level,
                100 + 50*level
            };
            return dmgs[index];
        }

        public static float SmiteDamage(Obj_AI_Base target)
        {
            float damage = 0;
            
            string[] champlist =
            {
                "Cho'Gath", "Nunu"
            };
            foreach (var champs in champlist)
            {
                if (Player.ChampionName == champs)
                {
                    switch (champs)
                    {
                        case "Nunu":
                        {
                            if (Player.Spellbook.GetSpell(SpellSlot.Q).IsReady() && Player.Distance(target) < 500
                                && GetBool("usenunuq", typeof(bool)))
                            {
                                damage += (float)(Player.GetSpellDamage(target, SpellSlot.Q));
                            }
                            break;
                        }
                    }
                }
            }
             


            if (_smiteSlot.IsReady())
            {
                damage += GetFuckingSmiteDamage();
            }

            Console.WriteLine("Damage From Champ + Smite{0}", damage);
            return damage;
        }


        private static bool GetSmiteSlot(ref SpellSlot smiteSlot)
        {
            foreach (var spell in Player.Spellbook.Spells)
            {
                if (!spell.Name.ToLower().Contains("smite")) continue;
                smiteSlot = spell.Slot;
                return true;
            }
            return false;
        }

    }
}
