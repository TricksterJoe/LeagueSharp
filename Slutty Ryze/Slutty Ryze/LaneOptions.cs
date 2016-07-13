using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_ryze
{
    internal class LaneOptions
    {
        #region Public Functions

        private static bool QSpell
        {
            get { return GlobalManager.Config.Item("useQ").GetValue<bool>(); }
        }

        private static bool ESpell
        {
            get { return GlobalManager.Config.Item("useE").GetValue<bool>(); }
        }

        private static bool WSpell
        {
            get { return GlobalManager.Config.Item("useW").GetValue<bool>(); }
        }

        private static bool RSpell
        {
            get { return GlobalManager.Config.Item("useR").GetValue<bool>(); }
        }

        private static bool RwwSpell
        {
            get { return GlobalManager.Config.Item("useRww").GetValue<bool>(); }
        }

        private static readonly Random Seeder = new Random();
        private static bool _casted = false;

        //struct MinionHealthPerSecond
        //{
        //    public float LastHp;
        //    public float DamagePerSecond;
        //}

        //private MinionHealthPerSecond[] calcMinionHealth(Obj_AI_Base[] minionsBase)
        //{
        //    MinionHealthPerSecond[] minionsStruct = new MinionHealthPerSecond[minionsBase.Length];
        //    const int checkDelay = 2;
        //    for (int i = 0; checkDelay > i; i++)
        //    {
        //        var startTime = Utils.TickCount;
        //        var endTime = startTime + 1;
        //        if (Utils.TickCount < endTime)

        //        for (int index = 0; index < minionsBase.Length; index++)
        //        {
        //            if (minionsBase[index].IsDead)
        //                    continue;

        //             var cMinionHP = minionsBase[index].Health;

        //             if (Math.Abs(minionsStruct[index].LastHp) > 1)
        //                minionsStruct[index].DamagePerSecond = (minionsStruct[index].LastHp - minionsBase[index].Health/checkDelay);

        //            minionsStruct[index].LastHp = minionsBase[index].Health;
        //        }
        //    }

        //    return minionsStruct;
        //}

        public static void LaneClear()
        {

            var qlchSpell = GlobalManager.Config.Item("useQlc").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useElc").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWlc").GetValue<bool>();


            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            if (GlobalManager.GetHero.ManaPercent <= minMana)
                return;

            foreach (var minion in minionCount)
            {
                if (!GlobalManager.CheckMinion(minion)) continue;

                var minionHp = minion.Health; // Reduce Calls and add in randomization buffer.
                //if (GlobalManager.Config.Item("doHuman").GetValue<bool>())
                //    minionHp = minion.Health * (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage
                if (minion.IsDead) return;

                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minionHp <= Champion.Q.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.Q.Cast(minion);

                else if (wlchSpell
                         && Champion.W.IsReady()
                         && minion.IsValidTarget(Champion.W.Range)
                         && minionHp <= Champion.W.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.W.CastOnUnit(minion);

                else if (elchSpell
                         && Champion.E.IsReady()
                         && minion.IsValidTarget(Champion.E.Range)
                         && minionHp <= Champion.E.GetDamage(minion) && GlobalManager.CheckMinion(minion))
                    Champion.E.CastOnUnit(minion);


            }
        }

        //get assembly version
        public static void JungleClear()
        {

            var qlchSpell = GlobalManager.Config.Item("useQj").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useEj").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWj").GetValue<bool>();
            //Convert to use new system later
            var mSlider = GlobalManager.Config.Item("useJM").GetValue<Slider>().Value;

            if (GlobalManager.GetHero.ManaPercent < mSlider)
                return;


            var jungle = MinionManager.GetMinions(Champion.Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);
            foreach (var jung in jungle)
            {
                if (qlchSpell && Champion.Q.IsReady())
                {
                    Champion.Q.Cast(jung);
                }

                if (elchSpell && Champion.E.IsReady())
                {
                    Champion.E.Cast(jung);
                }

                if (wlchSpell && Champion.W.IsReady())
                {
                    Champion.W.Cast(jung);
                }
            }

        }


        public static void LastHit()
        {
            #region Old

            var qlchSpell = GlobalManager.Config.Item("useQl2h").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useEl2h").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWl2h").GetValue<bool>();

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            foreach (var minion in minionCount)
            {
                var minionHp = minion.Health; // Reduce Calls and add in randomization buffer.

                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range - 20)
                    && minionHp < Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range - 10)
                    && minionHp < Champion.W.GetDamage(minion))
                    Champion.W.Cast(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range - 10)
                    && minionHp < Champion.E.GetDamage(minion))
                    Champion.E.Cast(minion);
            }

            #endregion
        }

        public static void Mixed()
        {
            #region Old

            var qSpell = GlobalManager.Config.Item("UseQM").GetValue<bool>();
            var qlSpell = GlobalManager.Config.Item("UseQMl").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("UseEM").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("UseWM").GetValue<bool>();
            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;

            if (GlobalManager.GetHero.ManaPercent < GlobalManager.Config.Item("mMin").GetValue<Slider>().Value)
                return;


            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (qSpell
                && Champion.Q.IsReady()
                && target.IsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.IsReady()
                && target.IsValidTarget(Champion.W.Range))
                Champion.W.Cast(target);

            if (eSpell
                && Champion.E.IsReady()
                && target.IsValidTarget(Champion.E.Range))
                Champion.E.Cast(target);

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            {
                if (GlobalManager.GetHero.ManaPercent <= minMana)
                    return;

                foreach (
                    var minion in
                        minionCount.Where(
                            minion =>
                                qlSpell && Champion.Q.IsReady() && minion.Health < Champion.Q.GetDamage(minion) &&
                                GlobalManager.CheckTarget(minion)))
                {
                    Champion.Q.Cast(minion);
                }
            }

            #endregion


        }

        public static void CastQ(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.IsValidTarget(Champion.Q.Range)
                && Champion.Q.IsReady())
                Champion.Q.Cast(target);
        }

        public static void CastQn(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !QSpell) return;
            if (target.IsValidTarget(Champion.Qn.Range)
                && Champion.Qn.IsReady())
                Champion.Qn.Cast(target);
        }

        public static void CastW(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !WSpell) return;
            if (target.IsValidTarget(Champion.W.Range)
                && Champion.W.IsReady())
                Champion.W.Cast(target);
        }

        public static void CastE(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !ESpell) return;
            if (target.IsValidTarget(Champion.E.Range)
                && Champion.E.IsReady())
                Champion.E.Cast(target);
        }

        public static void CastR(Obj_AI_Base target, bool menu = true)
        {
            if (menu && !RSpell) return;
            if (!Champion.R.IsReady())
                return;
            if (target.IsValidTarget(Champion.W.Range)
                && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
            {
                if (target.HasBuff("RyzeW"))
                    Champion.R.Cast();
            }
        }

        public static void Combo()
        {
            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical, false);

            if (!target.IsValidTarget())
                return;

            if ((!target.HasBuff("RyzeE") && Champion.E.IsReady()) ||
                (Champion.E.IsReady() && target.IsValidTarget(Champion.E.Range)) ||
                (!Champion.Q.IsReady() && !Champion.W.IsReady()))
            {
                if (target.IsValidTarget(Champion.E.Range) && Champion.E.IsReady())
                {
                    Champion.E.Cast(target);
                }
            }
            else
            {
                if (target.IsValidTarget(Champion.W.Range) && Champion.W.IsReady())
                {
                        Champion.W.Cast(target);
                }
                else if (target.IsValidTarget(Champion.Q.Range) && Champion.Q.IsReady())
                {
                    if (Champion.Q.MinHitChance == HitChance.High)
                    {
                        Champion.Q.Cast(target);
                    }
                }
            }
        }

    }
}



#endregion
