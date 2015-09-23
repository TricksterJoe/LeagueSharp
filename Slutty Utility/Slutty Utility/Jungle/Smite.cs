

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
//            JungleDraw.DamageToMonster = SmiteDamage;
//            Drawing.OnDraw += JungleDraw.Drawing_OnDrawMonster;
//            Drawing.OnDraw += JungleDraw.Drawing_OnDraw;
            
//
//            Drawing.OnDraw += Timer.OnDraw;
//            GameObject.OnCreate += Timer.OnCreate;
//            GameObject.OnDelete += Timer.OnDelete;

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


                if (!NumNumChamps.ContainsKey("Nunu"))
                    LoadNumNum();

                if (!GetBool("jungle.options.autoSmite", typeof(KeyBind))) return;
             //   Game.PrintChat("Jungle Auto Smite ON");

                if (GetBool("jungle.options.smiteBuffs", typeof(bool)))
                {
                   // Game.PrintChat("Check Buffs ON");
                    CheckBuffs();
                }

                if (GetBool("jungle.options.smiteEpic", typeof(bool)))
                    CheckEpics();

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
                PreformSmite(monster);

            }

        }

        private static void CheckBuffs()
        {
            foreach (var monster in MinionManager.GetMinions(Player.ServerPosition, 500,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth))
            {
                if (!monster.CharData.BaseSkinName.Equals("SRU_Red") 
                    && !monster.CharData.BaseSkinName.Equals("SRU_Blue")) continue;

                if (SmiteDamage(monster) < monster.Health) continue;
                PreformSmite(monster);

            }
        }

        public static void SmiteCheck()
        {
            SmiteTick = TickCount + 300;
        }

        private static float GetFuckingSmiteDamage()
        {
            var level = Player.Level;
            var index = Player.Level / 5;
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
                //Game.PrintChat("{0}:{1}",champs,Player.ChampionName);
                if (!String.Equals(Player.ChampionName, champs, StringComparison.CurrentCultureIgnoreCase)) continue;
                if (!Player.Spellbook.GetSpell(NumNumChamps[champs].SpellSlot).IsReady()) continue;
                if (!(Player.Distance(target) < NumNumChamps[champs].Range)) continue;
                // Game.PrintChat(@"Check 3", damage);
                damage += (float)(Player.GetSpellDamage(target, NumNumChamps[champs].SpellSlot));

                break;
            }


            if (SmiteSlot.IsReady())
            {
                damage += GetFuckingSmiteDamage();
            }

            return damage;
        }

        protected static void PreformSmite(Obj_AI_Base target)
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
                if (!spell.Name.ToLower().Contains("smite"))
                    return;
                smiteSlot = spell.Slot;
                return;
            }
        }
    }
}

