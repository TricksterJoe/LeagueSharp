

using System;
using System.Collections.Generic;
using System.Linq;
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

                if (GetBool("jungle.options.smiteEpic", typeof(bool)))
                    if (CheckEpics()) return;

                if (GetBool("jungle.options.smiteBuffs", typeof(bool)))
                    if (CheckBuffs()) return;

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

        private static bool CheckEpics()
        {
            foreach (var mob in MinionManager.GetMinions(Player.ServerPosition, 1000, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
            {
                if (!mob.Name.Contains("Baron") && !mob.Name.Contains("Dragon")) continue;
                if (!(SmiteDamage(mob) > mob.Health)) continue;
                PreformSmite(mob);
                return true;
            }

            return false;
        }

        private static bool CheckBuffs()
        {
            foreach (var monster in MinionManager.GetMinions(Player.ServerPosition, 1000,
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth))
            {
                if (!monster.CharData.BaseSkinName.Equals("SRU_Red") &&
                    !monster.CharData.BaseSkinName.Equals("SRU_Blue"))
                    continue;

                if (!(SmiteDamage(monster) > monster.Health)) continue;
                PreformSmite(monster);
                return true;
            }
            return false;
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
                if (target.IsValidTarget(NumNumChamps[champs].Range))
                    damage += (float)(Player.GetSpellDamage(target, NumNumChamps[champs].SpellSlot));

                break;
            }


            if (SmiteSlot.IsReady())
            {
                if(target.IsValidTarget(500))
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
            if (!target.IsValidTarget(500)) return;
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

