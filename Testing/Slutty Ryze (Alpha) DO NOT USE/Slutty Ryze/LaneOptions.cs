using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    class LaneOptions
    {
        #region Public Functions

        private const int RandomThreshold = 15; // 15%
        readonly static Random Seeder = new Random();
        public static void DisplayLaneOption(String line)
        {
            // not working o-o?
            // var displayAlert = new Alerter(250, 450, line, 7, new ColorBGRA(255f, 0f, 255f, 255f), "Calibri", 5F);
            // displayAlert.Remove();
        }

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
                float randSeed = Seeder.Next(1,RandomThreshold);
                var minionHp = minion.Health * (1 + (randSeed / 100.0f)) ; // Reduce Calls and add in randomization buffer.

                if (!GlobalManager.CheckMinion(minion)) continue;

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
                    && minionHp >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3) && GlobalManager.CheckMinion(minion))
                    Champion.Q.Cast(minion);

                else if (e2LSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range)
                    && minionHp >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3) && GlobalManager.CheckMinion(minion))
                    Champion.E.CastOnUnit(minion);

                else if (w2LSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range)
                    && minionHp >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3) && GlobalManager.CheckMinion(minion))
                    Champion.W.CastOnUnit(minion);

                if (rSpell
                    && Champion.R.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minionCount.Count > rSlider && GlobalManager.CheckMinion(minion))
                    Champion.R.Cast();
            }
        }


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
            var bSpells = new bool[3];
            bSpells[0] = GlobalManager.Config.Item("useQl2h").GetValue<bool>();
            bSpells[1] = GlobalManager.Config.Item("useEl2h").GetValue<bool>();
            bSpells[2] = GlobalManager.Config.Item("useWl2h").GetValue<bool>();

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);


            DisplayLaneOption("Last hitting");
            float randSeed = Seeder.Next(1, RandomThreshold);
            var minionHpOffset = (1 + (randSeed / 100.0f)); // Reduce Calls and add in randomization buffer.

            foreach (var minion in minionCount)
            {
                StartComboSequence(minion, bSpells, new[] { 'Q', 'W', 'E'} , minionHpOffset);
            }

        }

        public static void Mixed()
        {
            var bSpells = new bool[3];
            bSpells[0] = GlobalManager.Config.Item("UseQM").GetValue<bool>();
            bSpells[1] = GlobalManager.Config.Item("UseEM").GetValue<bool>();
            bSpells[2] = GlobalManager.Config.Item("UseWM").GetValue<bool>();

            if (GlobalManager.GetHero.ManaPercent < GlobalManager.Config.Item("mMin").GetValue<Slider>().Value)
            {
                DisplayLaneOption("Mixed Laneing");
                var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
                StartComboSequence(target, bSpells, new[] {'Q', 'W', 'E'});
            }
            // No target to harass
            LastHit();
        }
        public static void ImprovedCombo()
        {

            Champion.SetIgniteSlot(GlobalManager.GetHero.GetSpellSlot("summonerdot"));
            var bSpells = new bool[5];
            bSpells[0] = GlobalManager.Config.Item("useQ").GetValue<bool>();
            bSpells[1] = GlobalManager.Config.Item("useE").GetValue<bool>();
            bSpells[2] = GlobalManager.Config.Item("useW").GetValue<bool>();
            bSpells[3] = GlobalManager.Config.Item("useR").GetValue<bool>();
            bSpells[4] = GlobalManager.Config.Item("useRww").GetValue<bool>();

            var target = TargetSelector.GetTarget(Champion.W.Range, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget(Champion.W.Range) && (target.Health < Champion.IgniteDamage(target) + Champion.W.GetDamage(target)))
                GlobalManager.GetHero.Spellbook.CastSpell(Champion.GetIgniteSlot(), target);

            if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
            {
                Console.WriteLine("Using Ryze Combo Sequence 4");
                StartComboSequence(target, bSpells, new[] {'Q', 'W', 'Q', 'E', 'Q', 'R'});
            }
            else

            {
                switch (GlobalManager.GetPassiveBuff)
                {
                    case 1:
                    case 2:
                        Console.WriteLine("Using Ryze Combo Sequence 1");
                        StartComboSequence(target, bSpells, new[] {'Q', 'W', 'E', 'R'});
                        break;
                    case 3:
                        Console.WriteLine("Using Ryze Combo Sequence 2");
                        StartComboSequence(target, bSpells, new[] {'Q', 'E', 'W', 'R'});
                        break;
                    case 4:
                        Console.WriteLine("Using Ryze Combo Sequence 3");
                        StartComboSequence(target, bSpells, new[] {'W', 'Q', 'E', 'R'});
                        break;
                    default: // 0
                        Console.WriteLine("Using Ryze Combo Sequence default");
                        StartComboSequence(target, bSpells, new[] { 'W', 'Q', 'E' });
                        break;
                }
            }
        }

        private static void StartComboSequence(Obj_AI_Base target, IReadOnlyList<bool> bSpells, IEnumerable<char> seq,float hpOffset = 1)
        {
            //var autoAttack = !GlobalManager.Config.Item("AAblock").GetValue<bool>();
            var isMinion = target.IsMinion;
            foreach (var com in seq)
            {
                
                switch (com)
                {
                    case 'Q':
                        Console.WriteLine("Use Q Start");
                        if (!bSpells[0]) continue;
                        if (target.IsInvulnerable) continue;
                        //Is Hero
                        if (!isMinion)
                        {
                            if (GlobalManager.GetPassiveBuff >= 2)
                            {
                                if (target.IsValidTarget(Champion.Qn.Range) && Champion.Qn.IsReady())
                                {
                                    Champion.Qn.Cast(target);
                                }
                            }

                            else if (target.IsValidTarget(Champion.Q.Range) && Champion.Q.IsReady())
                                        Champion.Q.Cast(target);
                            
                        }
                        // is Minion
                        else if(!GlobalManager.CheckMinion(target))
                        {
                            if(!target.IsValidTarget(Champion.Q.Range - 20)) continue;

                            if (GlobalManager.GetPassiveBuff >= 2)
                            {
                                if (target.Health*hpOffset < Champion.Qn.GetDamage(target))
                                    Champion.Qn.Cast(target);
                            }
                            else
                            {
                                if (target.Health*hpOffset < Champion.Q.GetDamage(target))
                                    Champion.Q.Cast(target);
                            }
                        }

                        continue;

                    case 'W':
                        Console.WriteLine("Use W Start");
                        if (!bSpells[1]) continue;
                        if (target.IsInvulnerable) continue;

                        if (target.IsValidTarget(Champion.W.Range) && bSpells[1] && Champion.W.IsReady())
                        {
                            if (!isMinion)
                                Champion.W.CastOnUnit(target);
                            else if (target.Health * hpOffset < Champion.W.GetDamage(target) && GlobalManager.CheckMinion(target))
                                Champion.W.CastOnUnit(target);
                        }

                        continue;

                    case 'E':
                        Console.WriteLine("Use E Start");
                        if (!bSpells[2]) continue;
                        if (target.IsInvulnerable) continue;

                        if (target.IsValidTarget(Champion.E.Range) && Champion.E.IsReady())
                        {
                            if (!isMinion)
                                Champion.E.CastOnUnit(target);
                            else if (target.Health * hpOffset < Champion.E.GetDamage(target) && GlobalManager.CheckMinion(target))
                                Champion.E.CastOnUnit(target);
                        }

                        continue;

                    case 'R':

                        Console.WriteLine("Use R Start");
                        if (!bSpells[3]) continue;
                        if (target.IsInvulnerable) continue;
                        if (!target.IsValidTarget(Champion.W.Range))continue;
                        if(!(target.Health > Champion.W.GetDamage(target) + Champion.E.GetDamage(target))) continue;
                        if (!Champion.R.IsReady()) continue;
                        if (!bSpells[4]) continue;
                        if (target.HasBuff("RyzeW"))
                            Champion.R.Cast();

                        continue;
                }
                //if (!autoAttack) continue;
                //// Stop orbwalk AA to use are own
                //Champion.AABlock(false);
                //Champion.PreformAutoAttack(target);
                //Champion.AABlock(true);
            }

            if (!Champion.R.IsReady() || GlobalManager.GetPassiveBuff != 4 || !bSpells[4]) return;
            if (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()) return;

            Champion.R.Cast();
        }
    }
    #endregion
}
