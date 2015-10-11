using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    internal class LaneOptions
    {
        #region Public Functions
        
        private static bool QSpell  { get { return GlobalManager.Config.Item("useQ").GetValue<bool>(); } }
        private static bool ESpell { get { return GlobalManager.Config.Item("useE").GetValue<bool>(); } }
        private static bool WSpell { get { return GlobalManager.Config.Item("useW").GetValue<bool>(); } }
        private static bool RSpell { get { return GlobalManager.Config.Item("useR").GetValue<bool>(); } }
        private static bool RwwSpell { get { return GlobalManager.Config.Item("useRww").GetValue<bool>(); } }
        private static readonly Random Seeder = new Random();
        private static bool _casted =false;

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

        // ReSharper disable once FunctionComplexityOverflow
        public static void LaneClear()
        {
            if (GlobalManager.GetPassiveBuff == 4
                && !GlobalManager.GetHero.HasBuff("RyzeR")
                && GlobalManager.Config.Item("passiveproc").GetValue<bool>())
                return;

            var qlchSpell = GlobalManager.Config.Item("useQlc").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useElc").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWlc").GetValue<bool>();

            var q2LSpell = GlobalManager.Config.Item("useQ2L").GetValue<bool>();
            var e2LSpell = GlobalManager.Config.Item("useE2L").GetValue<bool>();
            var w2LSpell = GlobalManager.Config.Item("useW2L").GetValue<bool>();

            var rSpell = GlobalManager.Config.Item("useRl").GetValue<bool>();
            var rSlider = GlobalManager.Config.Item("rMin").GetValue<Slider>().Value;
            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            if (GlobalManager.GetHero.ManaPercent <= minMana)
                return;
            
            foreach (var minion in minionCount)
            {
                if (!GlobalManager.CheckMinion(minion)) continue;

                var minionHp = minion.Health;// Reduce Calls and add in randomization buffer.
                if (GlobalManager.Config.Item("doHuman").GetValue<bool>())
                    minionHp = minion.Health * (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage

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

                else if (q2LSpell
                         && Champion.Q.IsReady()
                         && minion.IsValidTarget(Champion.Q.Range)
                         && minionHp >= (GlobalManager.GetHero.GetAutoAttackDamage(minion)*1.3) &&
                         GlobalManager.CheckMinion(minion))
                    Champion.Q.Cast(minion);

                else if (e2LSpell
                         && Champion.E.IsReady()
                         && minion.IsValidTarget(Champion.E.Range)
                         && minionHp >= (GlobalManager.GetHero.GetAutoAttackDamage(minion)*1.3) &&
                         GlobalManager.CheckMinion(minion))
                    Champion.E.CastOnUnit(minion);

                else if (w2LSpell
                         && Champion.W.IsReady()
                         && minion.IsValidTarget(Champion.W.Range)
                         && minionHp >= (GlobalManager.GetHero.GetAutoAttackDamage(minion)*1.3) &&
                         GlobalManager.CheckMinion(minion))
                    Champion.W.CastOnUnit(minion);

                if (rSpell
                    && Champion.R.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minionCount.Count > rSlider && GlobalManager.CheckMinion(minion))
                    Champion.R.Cast();
            }
        }

        //get assembly version
        public static void JungleClear()
        {
            //Convert to use new system later
            var qSpell = GlobalManager.Config.Item("useQj").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("useEj").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("useWj").GetValue<bool>();
            var rSpell = GlobalManager.Config.Item("useRj").GetValue<bool>();
            var mSlider = GlobalManager.Config.Item("useJM").GetValue<Slider>().Value;

            if (GlobalManager.GetHero.ManaPercent < mSlider)
                return;


            var jungle = MinionManager.GetMinions(Champion.Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!jungle.IsValidTarget())
                return;

            if (eSpell
                && jungle.IsValidTarget(Champion.E.Range)
                && Champion.E.IsReady())
                Champion.E.CastOnUnit(jungle);
            if (qSpell
                && jungle.IsValidTarget(Champion.Q.Range)
                && Champion.Q.IsReady())
                Champion.Q.Cast(jungle);

            if (wSpell
                && jungle.IsValidTarget(Champion.W.Range)
                && Champion.W.IsReady())
                Champion.W.CastOnUnit(jungle);

            if (!rSpell || (GlobalManager.GetPassiveBuff != 4 && !GlobalManager.GetHero.HasBuff("RyzePassiveStack")))
                return;

            Champion.R.Cast();
        }


        public static void LastHit()
        {
            #region Old
            //var qlchSpell = GlobalManager.Config.Item("useQl2h").GetValue<bool>();
            //var elchSpell = GlobalManager.Config.Item("useEl2h").GetValue<bool>();
            //var wlchSpell = GlobalManager.Config.Item("useWl2h").GetValue<bool>();

            //var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
            //    MinionTeam.NotAlly);


            //DisplayLaneOption("Last hitting");
            //foreach (var minion in minionCount.Where(GlobalManager.CheckTarget))
            //{
            //    var minionHp = minion.Health;// Reduce Calls and add in randomization buffer.
            //    if (GlobalManager.Config.Item("doHuman").GetValue<bool>())
            //        minionHp = minion.Health * (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage

            //    if (qlchSpell
            //        && Champion.Q.IsReady()
            //        && minion.IsValidTarget(Champion.Q.Range - 20)
            //        && minionHp < Champion.Q.GetDamage(minion))
            //        Champion.Q.Cast(minion);

            //    if (wlchSpell
            //        && Champion.W.IsReady()
            //        && minion.IsValidTarget(Champion.W.Range - 10)
            //        && minionHp < Champion.W.GetDamage(minion))
            //        Champion.W.CastOnUnit(minion);

            //    if (elchSpell
            //        && Champion.E.IsReady()
            //        && minion.IsValidTarget(Champion.E.Range - 10)
            //        && minionHp < Champion.E.GetDamage(minion))
            //        Champion.E.CastOnUnit(minion);
            //}
            #endregion
            #region New
            //New verion
            var bSpells = new bool[3];
            bSpells[0] = GlobalManager.Config.Item("useQl2h").GetValue<bool>();
            bSpells[1] = GlobalManager.Config.Item("useEl2h").GetValue<bool>();
            bSpells[2] = GlobalManager.Config.Item("useWl2h").GetValue<bool>();


            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);

            var minionHpOffset = 1.0f;
            if (GlobalManager.Config.Item("doHuman").GetValue<bool>())
                minionHpOffset = (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage

            foreach (var minion in minionCount)
            {
                StartComboSequence(minion, bSpells, new[] { 'Q', 'W', 'E' }, minionHpOffset);
            }
            #endregion
        }

        public static void Mixed()
        {
            #region Old
           // var qSpell = GlobalManager.Config.Item("UseQM").GetValue<bool>();
           // var qlSpell = GlobalManager.Config.Item("UseQMl").GetValue<bool>();
           // var eSpell = GlobalManager.Config.Item("UseEM").GetValue<bool>();
           // var wSpell = GlobalManager.Config.Item("UseWM").GetValue<bool>();
            //var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;

            //if (GlobalManager.GetHero.ManaPercent < GlobalManager.Config.Item("mMin").GetValue<Slider>().Value)
            //    return;


            //DisplayLaneOption("Mixed Laneing");
            //var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            //if (qSpell
            //    && Champion.Q.IsReady()
            //    && target.IsValidTarget(Champion.Q.Range))
            //    Champion.Q.Cast(target);

            //if (wSpell
            //    && Champion.W.IsReady()
            //    && target.IsValidTarget(Champion.W.Range))
            //    Champion.W.CastOnUnit(target);

            //if (eSpell
            //    && Champion.E.IsReady()
            //    && target.IsValidTarget(Champion.E.Range))
            //    Champion.E.CastOnUnit(target);

            //var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
            //    MinionTeam.NotAlly);
            //{
            //    if (GlobalManager.GetHero.ManaPercent <= minMana)
            //        return;

            //    foreach (
            //        var minion in
            //            minionCount.Where(
            //                minion =>
            //                    qlSpell && Champion.Q.IsReady() && minion.Health < Champion.Q.GetDamage(minion) &&
            //                    GlobalManager.CheckTarget(minion)))
            //    {
            //        Champion.Q.Cast(minion);
            //    }
            //}
#endregion
            //New region
            
            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;
            var bSpells = new bool[3];
            bSpells[0] = GlobalManager.Config.Item("UseQM").GetValue<bool>();
            bSpells[1] = GlobalManager.Config.Item("UseEM").GetValue<bool>();
            bSpells[2] = GlobalManager.Config.Item("UseWM").GetValue<bool>();

            if (GlobalManager.GetHero.ManaPercent < GlobalManager.Config.Item("mMin").GetValue<Slider>().Value)
                return;

            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            StartComboSequence(target, bSpells, new[] {'Q', 'W', 'E'});

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All,
                MinionTeam.NotAlly);
            {
                if (GlobalManager.GetHero.ManaPercent <= minMana)
                    return;


                var minionHpOffset = 1.0f;
                if (GlobalManager.Config.Item("doHuman").GetValue<bool>())
                    minionHpOffset = (1 + (Seeder.Next(GlobalManager.Config.Item("minCreepHPOffset").GetValue<Slider>().Value, GlobalManager.Config.Item("maxCreepHPOffset").GetValue<Slider>().Value) / 100.0f));//Randomioze Minion Hp from min to max hp less than damage

                foreach (var minion in minionCount)
                { 
                    StartComboSequence(minion, new[] {true}, new[] {'Q'}, minionHpOffset);
                }
            }
            

        }

        public static void CastQ(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(Champion.Q.Range)
                && QSpell
                && Champion.Q.IsReady())
                Champion.Q.Cast(target);
        }

        public static void CastQn(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(Champion.Qn.Range)
                && QSpell
                && Champion.Qn.IsReady())
                Champion.Qn.Cast(target);
        }

        public static void CastW(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(Champion.W.Range)
                && WSpell
                && Champion.W.IsReady())
                Champion.W.Cast(target);
        }

        public static void CastE(Obj_AI_Hero target)
        {
            if (target.IsValidTarget(Champion.E.Range)
                && ESpell
                && Champion.E.IsReady())
                Champion.E.Cast(target);
        }

        public static void CastR(Obj_AI_Hero target)
        {
            if (!RSpell || !Champion.R.IsReady())
                return;
            if (target.IsValidTarget(Champion.W.Range)
                && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
            {
                if (RwwSpell && target.HasBuff("RyzeW"))
                    Champion.R.Cast();
                if (!RwwSpell)
                    Champion.R.Cast();
            }
        }

        // ReSharper disable once FunctionComplexityOverflow
        public static void ImprovedCombo()
        {   
            Champion.SetIgniteSlot(GlobalManager.GetHero.GetSpellSlot("summonerdot"));
            var target = TargetSelector.GetTarget(Champion.W.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(Champion.Q.Range) || !GlobalManager.CheckTarget(target)) return;

            if (target.IsValidTarget(Champion.W.Range) &&
                (target.Health < Champion.IgniteDamage(target) + Champion.W.GetDamage(target)))
                GlobalManager.GetHero.Spellbook.CastSpell(Champion.GetIgniteSlot(), target);

            var bSpells = new bool[5];
            switch (GlobalManager.Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                    #region Wombo Combo
                case 0:
                    var qSpell = GlobalManager.Config.Item("useQ").GetValue<bool>();
                    var eSpell = GlobalManager.Config.Item("useE").GetValue<bool>();
                    var wSpell = GlobalManager.Config.Item("useW").GetValue<bool>();
                    var rSpell = GlobalManager.Config.Item("useR").GetValue<bool>();
                    var rwwSpell = GlobalManager.Config.Item("useRww").GetValue<bool>();
                    if (target.IsValidTarget(Champion.Q.Range))
                    {
                        if (GlobalManager.GetPassiveBuff <= 2
                            || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {

                            CastQ(target);
                            CastW(target);
                            CastE(target);
                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }


                        if (GlobalManager.GetPassiveBuff == 3)
                        {
                            if (Champion.Q.IsReady()
                                && target.IsValidTarget(Champion.Q.Range))
                                Champion.Qn.Cast(target);

                            if (Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.Qn.Range)
                                && Champion.Q.IsReady()
                                && qSpell)
                                Champion.Qn.Cast(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && Champion.E.IsReady()
                                && eSpell)
                                Champion.E.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (wSpell
                                && Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (eSpell
                                && Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                    if (!Champion.E.IsReady() && !Champion.Q.IsReady() && !Champion.W.IsReady())
                                        Champion.R.Cast();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (wSpell
                            && Champion.W.IsReady()
                            && target.IsValidTarget(Champion.W.Range))
                            Champion.W.CastOnUnit(target);

                        if (qSpell
                            && Champion.Qn.IsReady()
                            && target.IsValidTarget(Champion.Qn.Range))
                            Champion.Qn.Cast(target);

                        if (eSpell
                            && Champion.E.IsReady()
                            && target.IsValidTarget(Champion.E.Range))
                            Champion.E.CastOnUnit(target);
                    }
                    if (!Champion.R.IsReady() || GlobalManager.GetPassiveBuff != 4 || !rSpell) return;

                    if (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()) return;

                    Champion.R.Cast();
                    break;
                    #endregion        
           }
    }
        private static void StartComboSequence(Obj_AI_Base target, IReadOnlyList<bool> bSpells, IEnumerable<char> seq, float hpOffset = 1)
        {
            foreach (var com in seq)
            {
                var isMinion = target.IsMinion;
                switch (com)
                {
                    case 'Q':
                        if (!bSpells[0]) continue;

                        if (target.IsValidTarget(Champion.Q.Range) && Champion.Q.IsReady() && !target.IsInvulnerable)
                        {
                            if (isMinion &&
                                !(target.Health * hpOffset < Champion.Q.GetDamage(target) &&
                                  GlobalManager.CheckMinion(target))) continue;
                            if (GlobalManager.GetPassiveBuff > 2 ||
                                GlobalManager.GetHero.HasBuff("RyzePassiveStack") && Champion.Q.IsReady())
                                Champion.Qn.Cast(target);
                            else
                                Champion.Q.Cast(target);
                        }

                        continue;

                    case 'W':
                        if (!bSpells[1]) continue;

                        if (target.IsValidTarget(Champion.W.Range) && bSpells[1] && Champion.W.IsReady() && !target.IsInvulnerable)
                            if (isMinion && !(target.Health * hpOffset < Champion.W.GetDamage(target) && GlobalManager.CheckMinion(target))) continue;
                        Champion.W.Cast(target);

                        continue;

                    case 'E':
                        if (!bSpells[2]) continue;

                        if (target.IsValidTarget(Champion.E.Range) && bSpells[2] && Champion.E.IsReady() && !target.IsInvulnerable)
                            if (isMinion && !(target.Health * hpOffset < Champion.E.GetDamage(target) && GlobalManager.CheckMinion(target))) continue;
                        Champion.E.Cast(target);


                        continue;

                    case 'R':
                        if (!bSpells[3]) continue;
                        if (!target.IsValidTarget(Champion.W.Range) || !(target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target))) || target.IsInvulnerable)
                            continue;
                        if (!Champion.R.IsReady()) continue;
                        if (bSpells[4] && target.HasBuff("RyzeW") || !bSpells[4])
                            Champion.R.Cast();
                        continue;
                }

            }

            if (!Champion.R.IsReady() || GlobalManager.GetPassiveBuff != 4 || !bSpells[4]) return;
            if (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()) return;

            Champion.R.Cast();
        }
    }
}
#endregion
