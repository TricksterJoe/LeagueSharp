using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    internal class Smite : Helper
    {
        public static SpellSlot SmiteSlot;
        private static float SmiteTick { get; set; }

        private static readonly Dictionary<String, ExternalSpell> NumNumChamps = new Dictionary<String, ExternalSpell>();

        public static void OnLoad()
        {
            GetSmiteSlot(ref SmiteSlot);

            Drawing.OnDraw += OnUpdate;
            JungleDraw.DamageToMonster = SmiteDamage;
            Drawing.OnDraw += JungleDraw.Drawing_OnDrawMonster;
            Drawing.OnDraw += JungleDraw.Drawing_OnDraw;
            GameObject.OnCreate += JungleDraw.OnCreate;

            Drawing.OnDraw += Timer.OnDraw;
            GameObject.OnCreate += Timer.OnCreate;
            GameObject.OnDelete += Timer.OnDelete;
            
        }

        private struct ExternalSpell
        {
            public readonly SpellSlot SpellSlot;
            public readonly float Range;

            public ExternalSpell(SpellSlot spellSlot, float range)
            {
                Range = range;
                SpellSlot = spellSlot;
            }
        }


        private static void OnUpdate(EventArgs args)
        {
            if (SmiteTick > TickCount) return;

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
                if (SmiteDamage(monster) < monster.Health) continue;
                    Player.Spellbook.CastSpell(SmiteSlot, monster);
                
            }

        }

        private static void CheckBuffs()
        {
            foreach (var monster in MinionManager.GetMinions(Player.ServerPosition, 500,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth))
            {
                if (!monster.CharData.BaseSkinName.Equals("SRU_Red") && !monster.CharData.BaseSkinName.Equals("SRU_Blue")) continue;
                if (SmiteDamage(monster) < monster.Health) continue;
                Player.Spellbook.CastSpell(NumNumChamps[Player.ChampionName].SpellSlot, monster);
                Player.Spellbook.CastSpell(SmiteSlot, monster);
                
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
            
            foreach (var champs in NumNumChamps.Keys)
            {
                if (Player.ChampionName != champs) continue;
                if (!Player.Spellbook.GetSpell(NumNumChamps[champs].SpellSlot).IsReady()) continue;
                if(!(Player.Distance(target) < NumNumChamps[champs].Range))continue;
                    damage += (float) (Player.GetSpellDamage(target, NumNumChamps[champs].SpellSlot));
                
                break; 
            }


            if (SmiteSlot.IsReady())
            {
                damage += GetFuckingSmiteDamage();
            }

            Console.WriteLine(@"Damage From Champ + Smite{0}", damage);
            return damage;
        }

        protected virtual void PreformSmite(Obj_AI_Base target)
        {
            foreach (var champs in NumNumChamps.Keys)
            {
                if (Player.ChampionName != champs) continue;
                if (!NumNumChamps[Player.ChampionName].SpellSlot.IsReady()) break;
                if (!target.IsValidTarget(NumNumChamps[Player.ChampionName].Range)) break;
                Player.Spellbook.CastSpell(NumNumChamps[Player.ChampionName].SpellSlot, target);
            }

            if (!SmiteSlot.IsReady()) return;
            if (!target.IsValidTarget(550)) return;
            Player.Spellbook.CastSpell(SmiteSlot, target);
        }

        private static void GetSmiteSlot(ref SpellSlot smiteSlot)
        {
            foreach (var spell in Player.Spellbook.Spells)
            {
                if (!spell.Name.ToLower().Contains("smite")) continue;
                smiteSlot = spell.Slot;
                return;
            }
        }
    }
}
