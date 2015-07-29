using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_ryze
{
    class LaneOptions
    {
        #region Public Functions

        public static void DisplayLaneOption(String line)
        {
            // not working o-o?
            // var displayAlert = new Alerter(250, 450, line, 7, new ColorBGRA(255f, 0f, 255f, 255f), "Calibri", 5F);
            // displayAlert.Remove();
        }

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
            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (GlobalManager.GetHero.ManaPercent <= minMana)
                return;

            DisplayLaneOption("Clearing Lane");
            foreach (var minion in minionCount)
            {
                if(!GlobalManager.CheckTarget(minion)) continue; 
                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minion.Health <= Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range)
                    && minion.Health <= Champion.W.GetDamage(minion))
                    Champion.W.CastOnUnit(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range)
                    && minion.Health <= Champion.E.GetDamage(minion))
                    Champion.E.CastOnUnit(minion);

                if (q2LSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minion.Health >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3))
                    Champion.Q.Cast(minion);

                if (e2LSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range)
                    && minion.Health >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3))
                    Champion.E.CastOnUnit(minion);

                if (w2LSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range)
                    && minion.Health >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3))
                    Champion.W.CastOnUnit(minion);

                if (rSpell
                    && Champion.R.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minionCount.Count > rSlider)
                    Champion.R.Cast();
            }
        }


        public static void JungleClear()
        {
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
            if (!GlobalManager.CheckTarget(jungle))
                return;

            DisplayLaneOption("Clearing Jungle");
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

            if (!rSpell || (GlobalManager.GetPassiveBuff != 4 && !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))) return;

            Champion.R.Cast();
        }

        public static void LastHit()
        {
            var qlchSpell = GlobalManager.Config.Item("useQl2h").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useEl2h").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWl2h").GetValue<bool>();

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);


            DisplayLaneOption("Last hitting");
            foreach (var minion in minionCount)
            {
                if (!GlobalManager.CheckTarget(minion)) continue;

                    if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range - 20)
                    && minion.Health < Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range - 10)
                    && minion.Health < Champion.W.GetDamage(minion))
                    Champion.W.CastOnUnit(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range - 10)
                    && minion.Health < Champion.E.GetDamage(minion))
                    Champion.E.CastOnUnit(minion);
            }
        }

        public static void Mixed()
        {
            var qSpell = GlobalManager.Config.Item("UseQM").GetValue<bool>();
            var qlSpell = GlobalManager.Config.Item("UseQMl").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("UseEM").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("UseWM").GetValue<bool>();
            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;

            if (GlobalManager.GetHero.ManaPercent < GlobalManager.Config.Item("mMin").GetValue<Slider>().Value)
                return;


            DisplayLaneOption("Mixed Laneing");
            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (qSpell
                && Champion.Q.IsReady()
                && target.IsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.IsReady()
                && target.IsValidTarget(Champion.W.Range))
                Champion.W.CastOnUnit(target);

            if (eSpell
                && Champion.E.IsReady()
                && target.IsValidTarget(Champion.E.Range))
                Champion.E.CastOnUnit(target);

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if (GlobalManager.GetHero.ManaPercent <= minMana)
                    return;

                foreach (var minion in minionCount.Where(minion => qlSpell && Champion.Q.IsReady() && minion.Health < Champion.Q.GetDamage(minion) && GlobalManager.CheckTarget(minion)))
                {
                    Champion.Q.Cast(minion);
                }
            }
        }

        public static void Combo()
        {
            Champion.SetIgniteSlot(GlobalManager.GetHero.GetSpellSlot("summonerdot"));
            var qSpell = GlobalManager.Config.Item("useQ").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("useE").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("useW").GetValue<bool>();
            var rSpell = GlobalManager.Config.Item("useR").GetValue<bool>();
            var rwwSpell = GlobalManager.Config.Item("useRww").GetValue<bool>();
            var target = TargetSelector.GetTarget(Champion.W.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(Champion.Q.Range) || !GlobalManager.CheckTarget(target)) return;

            if (target.IsValidTarget(Champion.W.Range) && (target.Health < Champion.IgniteDamage(target) + Champion.W.GetDamage(target)))
                GlobalManager.GetHero.Spellbook.CastSpell(Champion.GetIgniteSlot(), target);


            switch (GlobalManager.Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                case 1:
                    if (Champion.R.IsReady())
                    {
                        if (GlobalManager.GetPassiveBuff == 1 || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (rSpell)
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

                        if (GlobalManager.GetPassiveBuff == 2)
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                    if (target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
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
                                    if (rwwSpell && target.HasBuff("RyzeW")
                                        && (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()))
                                        Champion.R.Cast();

                                    if (!rwwSpell
                                        && (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()))
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

                                    if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

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

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

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

                    if (!Champion.R.IsReady())
                    {
                        if (GlobalManager.GetPassiveBuff == 1
                            || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);
                        }

                        if (GlobalManager.GetPassiveBuff == 2)
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (rSpell)
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
                        }

                        if (GlobalManager.GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(Champion.E.Range)
                                && Champion.E.IsReady()
                                && eSpell)
                                Champion.E.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.Qn.Range)
                                && Champion.Q.IsReady()
                                && qSpell)
                                Champion.Qn.Cast(target);
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

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

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);
                        }
                    }
                    break;


                case 0:

                    if (target.IsValidTarget(Champion.Q.Range))
                    {
                        if (GlobalManager.GetPassiveBuff <= 2
                            || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
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
                    break;
            }

            if (!Champion.R.IsReady() || GlobalManager.GetPassiveBuff != 4 || !rSpell) return;

            if (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()) return;

            Champion.R.Cast();
        }
    }
#endregion
}
