using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    class Smite : Helper
    {
        private static SpellSlot _smiteSlot;
        private static float SmiteTick { get; set; }

        private static readonly Dictionary<String, ExternalSpell> NumNumChamps = new Dictionary<String, ExternalSpell>();

        public static void OnLoad()
        {
            Drawing.OnDraw += OnUpdate;
            JungleDraw.DamageToMonster = SmiteDamage;
            Drawing.OnDraw += JungleDraw.Drawing_OnDrawMonster;
            Drawing.OnDraw += JungleDraw.Drawing_OnDraw;
            GameObject.OnCreate += JungleDraw.OnCreat;
        }

        struct ExternalSpell
        {
            public readonly SpellSlot _SpellSlot;
            public readonly float Range;

            public ExternalSpell(SpellSlot spellSlot,float range)
            {
                Range = range;
                _SpellSlot = spellSlot;
            }
        }

      
        private static void OnUpdate(EventArgs args)
        {
            try
            {
                if (!NumNumChamps.ContainsKey("Nunu"))
                    LoadNumNum();                     
                if (!GetBool("jungle.options.autoSmite", typeof(bool))) return;
                if (SmiteTick > TickCount) return;
                if (!GetSmiteSlot(ref _smiteSlot)) return;

                if (GetBool("jungle.options.smiteEpics", typeof (bool)))
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
            NumNumChamps.Add("Nunu", new ExternalSpell(SpellSlot.Q, 125));
            NumNumChamps.Add("Cho'Gath", new ExternalSpell(SpellSlot.R, 175));
        }
        private static bool CheckEpics()
        {

            foreach (var monster in MinionManager.GetMinions(Player.ServerPosition,SpellRange(_smiteSlot), MinionTypes.All,MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
            {
                if (!monster.Name.Contains("Baron") && !monster.Name.Contains("Dragon")) continue;
                if (!(SmiteDamage(monster) > monster.Health)) return false;
                JustDoIt(monster);
                return true;
            }

            return false;
        }
        private static bool CheckBuffs()
        {
            foreach (var monster in MinionManager.GetMinions(Player.ServerPosition,
                SpellRange(_smiteSlot),
                MinionTypes.All,
                MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth))
            {
                if (!monster.CharData.BaseSkinName.Equals("SRU_Red") &&
                    !monster.CharData.BaseSkinName.Equals("SRU_Blue"))
                    continue;
                if (!(SmiteDamage(monster) > monster.Health)) return false;
                JustDoIt(monster);
                return true;
            }
            return false;
        }
        public static void SmiteCheck()
        {
            SmiteTick = TickCount + 300;
        }

        private static void JustDoIt(Obj_AI_Base target)
        {
            if (NumNumChamps.ContainsKey(Player.ChampionName) && NumNumChamps[Player.ChampionName]._SpellSlot.IsReady() &&
                target.IsValidTarget(NumNumChamps[Player.ChampionName].Range))
                Player.Spellbook.CastSpell(NumNumChamps[Player.ChampionName]._SpellSlot,target);

            if (_smiteSlot.IsReady() && target.IsValidTarget(550))
            Player.Spellbook.CastSpell(NumNumChamps[Player.ChampionName]._SpellSlot,target);
        }

        private static float GetFuckingSmiteDamage()
        {
            var level = Player.Level;
            var index = Player.Level / 5;
            float[] dmgs = 
                {   370 + 20 * level,
                     330 + 30 * level,
                      240 + 40 * level,
                        100 + 50 * level };
            return dmgs[index];
        }

        public static float SmiteDamage(Obj_AI_Base target)
        {
            float damage = 0;

            if (_smiteSlot.IsReady())
                damage += GetFuckingSmiteDamage();

            if (NumNumChamps.ContainsKey(Player.ChampionName))
                if (NumNumChamps[Player.ChampionName]._SpellSlot.IsReady())
                    if (target.IsValidTarget(NumNumChamps[Player.ChampionName].Range))
                        damage += (float) (Player.GetSpellDamage(target, NumNumChamps[Player.ChampionName]._SpellSlot));

        Console.WriteLine("Damage From Champ + Smite{0}",damage);
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
