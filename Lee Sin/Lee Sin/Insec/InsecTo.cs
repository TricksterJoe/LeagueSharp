using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Lee_Sin.WardManager;
using SharpDX;
using Color = System.Drawing.Color;
using Prediction = Lee_Sin.Prediction;

namespace Lee_Sin.Insec
{
    class InsecTo : LeeSin
    {
        public static bool Setbool;
        private static int _starttimer;

        public static bool Checkno1(Obj_AI_Base target)
        {
           if (target.Buffs.Any(x => x.Name.ToLower().Contains("blindmonkqone")))
            {
                Setbool = true;
            }

           if (Setbool && !target.Buffs.Any(x => x.Name.ToLower().Contains("blindmonkqone")))
            {
                _starttimer = Environment.TickCount;
                Setbool = false;
            }

            return Environment.TickCount - _starttimer > 2500;

        }
        public static bool CanQ2()
        {
            return Environment.TickCount - Lasttotarget > 3000;
        }
        public static void Insec()
        {
            #region Target, Slots, Prediction

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(Q.Range + 800, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
            }

            if (target == null) return;
            var qpred = Q.GetPrediction(target);

            var col = qpred.CollisionObjects;

            var slot = WardSorter.Wards();
            #endregion

            if (Player.Distance(target) > 500)
            {
                if (Q2() && Q.IsReady() && (R.IsReady() || Environment.TickCount - Lastr < 4000))
                {
                    if (CanQ2())
                    Utility.DelayAction.Add(400, () => Q.Cast());
                }
            }

            if (Q1() && Player.Distance(target) <= Q.Range)
            {
                OnUpdate.CastSpell(Q, target);
            }


            var poss = InsecPos.WardJumpInsecPosition.InsecPos(target, GetValue("fixedwardrange"), true);
            if (!GetBool("laggy", typeof(bool)))
            {
                var min =
                SebbyLib.Cache.GetMinions(Player.Position, Q.Range)
                    .Where(
                        x => x.Health > Q.GetDamage(x) + 30 && !x.IsDead);

                foreach (var mins in min.Where(mins => mins.Distance(target) < 800 ||
                                                       mins.Distance(poss.To3D()) < 530 ||
                                                       (CanWardFlash(target) &&
                                                        mins.Distance(target) < 600)).Where(mins => col.Count != 0))
                {
                    if (Q1() && Q.IsReady())
                    {
                        Q.Cast(mins);
                    }
                    if (Q1() && Q.IsReady())
                    {
                        Q.Cast(mins);
                    }

                    if (Q2() && mins.HasBuff("blindmonkqtwo"))
                    {
                        Q.Cast();
                    }
                }
            }


            if ((Steps == LeeSin.steps.WardJump || Environment.TickCount - Lastwardjump < 1500) && slot != null && W.IsReady() && R.IsReady())
            {

                if (GetValue("fixedwardrange") + Player.ServerPosition.Distance(target.ServerPosition) < 700
                    && Environment.TickCount - LastTeleported > 500)
                {
                    WardManager.WardJump.WardJumped(poss.To3D(), false, false);
                    LeeSin.Lastwardjumpd = Environment.TickCount;
                    Canwardflash = false;
                }
                else if (CanWardFlash(target))
                {
                    Canwardflash = true;
                }
            }

            if (Environment.TickCount - Lastprocessw < 1500 || (Steps == LeeSin.steps.Flash && HasFlash()) ||
                Environment.TickCount - Lastwcasted < 1500 || Player.Distance(poss) < 50)
            {
                if (R.IsReady())
                    R.Cast(target);
            }

            if ((!W.IsReady() || W2()) && !GetBool("prioflash", typeof(bool)) &&
                Environment.TickCount - Lastwcasted > 1500 && LastQ(target))
            {
                Lastflashoverprio = Environment.TickCount;
                R.Cast(target);
            }
            #region Q Smite

            var prediction = Prediction.GetPrediction(target, Q.Delay);

            var collision = Q.GetCollision(Player.Position.To2D(),
                new List<Vector2> { prediction.UnitPosition.To2D() });

            foreach (var collisions in collision)
            {
                if (collision.Count == 1)
                {
                    if (collision[0].IsMinion && collision[0].IsEnemy)
                    {
                        if (GetBool("UseSmite", typeof (bool)))
                        {
                            if (Q.IsReady())
                            {
                                if (collision[0].Distance(Player) < 500)
                                {
                                    if (collision[0].Health <= ActiveModes.Smite.GetFuckingSmiteDamage() &&
                                        Smite.IsReady())
                                    {
                                        Q.Cast(prediction.CastPosition);
                                        Player.Spellbook.CastSpell(Smite, collision[0]);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            #endregion
            #region Determine if we want to flash or ward jump

            if (R.IsReady())
            {
                if (slot != null && (W.IsReady() || Environment.TickCount - Lastprocessw < 1500))
                {
                    if (GetBool("prioflash", typeof(bool)) && Player.GetSpellSlot("summonerflash").IsReady())
                    {
                        Steps = LeeSin.steps.Flash;
                        if (Environment.TickCount - Lastqcasted < 600)
                        {
                            Canwardflash = false;
                        }
                        else if (CanWardFlash(target))
                        {
                            Canwardflash = true;
                        }       
                    }
                    else if (GetValue("fixedwardrange") + Player.ServerPosition.Distance(target.ServerPosition) < 900)
                    {
                        if (CanWardFlash(target))
                        {
                            Canwardflash = true;
                        }
                        Lastwardjump = Environment.TickCount;
                    }
                }
                else if (GetBool("useflash", typeof(bool)) && target.Distance(Player) < 400 &&
                         Player.GetSpellSlot("SummonerFlash").IsReady() && (slot == null || !W.IsReady() || W2()) &&
                         (Environment.TickCount - Lastwcasted > 2000 || Environment.TickCount - Lastprocessw > 2000))
                {
                    Steps = LeeSin.steps.Flash;
                }
            }

            var wardtotargetpos = Player.Position.Extend(target.Position, Player.Distance(target) - 150);

            if (!Canwardflash) return;
            if (Player.HasBuff("blindmonkqtwodash")) return;

            if (Player.Distance(target) < 350 || target.Distance(Player) > 900 ||
                Environment.TickCount - Lastq1Casted < 500
                || !CanWardFlash(target) || Environment.TickCount - LeeSin.Lsatcanjump1 < 3000 ||
                target.Buffs.Any(x => x.Name.ToLower().Contains("blindmonkqone")))
                return;



        if (!Checkno1(target)) return;
          // if ( Q2()) return;
         //  if (!HasFlash()) return;
            if (LastQ(target)
                && Environment.TickCount - LastTeleported > 500)
            {
                WardManager.WardJump.WardJumped(wardtotargetpos, true, false);
                Wardjumpedto = Environment.TickCount;
                Wardjumpedtotarget = true;
                Lastflashward = Environment.TickCount;
            }

            #endregion
        }
    }
}