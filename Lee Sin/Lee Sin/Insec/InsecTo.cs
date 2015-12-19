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
        public static void insec()
        {
            #region Target, Slots, Prediction

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(Q.Range + 800, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
            }
            // if (!R.IsReady() && Environment.TickCount - lastr > 2000) return;

            if (target == null) return;
            
            var qpred = Q.GetPrediction(target);

            var col = qpred.CollisionObjects;

            var slot = WardSorter.Wards();
            #endregion

            if (Player.Distance(target) > 500)
            {
                if (Q2() && Q.IsReady() && (R.IsReady() || Environment.TickCount - lastr < 4000))
                {
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
                MinionManager.GetMinions(poss.To3D(), 800, MinionTypes.All, MinionTeam.NotAlly)
                    .Where(
                        x => !x.Name.ToLower().Contains("turret") && !x.Name.ToLower().Contains("tower")
                             && x.Health > Q.GetDamage(x) + 50 && !x.IsDead &&
                             Q.GetPrediction(x).CollisionObjects.Count == 0 && x.Distance(Player) < Q.Range);
                    foreach (var mins in min.Where(mins => mins.Distance(target) < 500 ||
                                                           mins.Distance(poss.To3D()) < 530 ||
                                                           (CanWardFlash(target) &&
                                                            mins.Distance(target) < 600)))
                        
                    {
                       // Render.Circle.DrawCircle(mins.Position, 100, Color.AliceBlue, 3);
                        if (col.Count <= 0 && !(target.Distance(Player) > Q.Range)) continue;
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

            if ((Steps == LeeSin.steps.WardJump || Environment.TickCount - _lastwardjump < 1500) && slot != null && W.IsReady() && R.IsReady())
            {

                if (GetValue("fixedwardrange") + Player.ServerPosition.Distance(target.ServerPosition) < 700)
                {
                    WardManager.WardJump.WardJumped(poss.To3D(), false, false);
                    LeeSin.lastwardjumpd = Environment.TickCount;
                    canwardflash = false;
                }
                else if (CanWardFlash(target))
                {
                    canwardflash = true;
                }
            }

            if (Environment.TickCount - _lastprocessw < 1500 || (Steps == LeeSin.steps.Flash && HasFlash()) ||
                Environment.TickCount - _lastwcasted < 1500 || Player.Distance(poss) < 50)
            {
                if (R.IsReady())
                    R.Cast(target);
            }

            if ((!W.IsReady() || W2()) && !GetBool("prioflash", typeof(bool)) &&
                Environment.TickCount - _lastwcasted > 1500 && LastQ(target))
            {
                lastflashoverprio = Environment.TickCount;
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
                if (slot != null && (W.IsReady() || Environment.TickCount - _lastprocessw < 1500))
                {
                    if (GetBool("prioflash", typeof(bool)) && Player.GetSpellSlot("summonerflash").IsReady())
                    {
                        Steps = LeeSin.steps.Flash;
                        if (Environment.TickCount - _lastqcasted < 600)
                        {
                            canwardflash = false;
                        }
                        else if (CanWardFlash(target))
                        {
                            canwardflash = true;
                        }       
                    }
                    else if (GetValue("fixedwardrange") + Player.ServerPosition.Distance(target.ServerPosition) < 700)
                    {
                        Steps = LeeSin.steps.WardJump;
                        if (Environment.TickCount - _lastqcasted < 600)
                        {
                            canwardflash = false;
                        }
                        else if (CanWardFlash(target))
                        {
                            canwardflash = true;
                        }
                        _lastwardjump = Environment.TickCount;
                    }
                }
                else if (GetBool("useflash", typeof(bool)) && target.Distance(Player) < 400 &&
                         Player.GetSpellSlot("SummonerFlash").IsReady() && (slot == null || !W.IsReady() || W2()) &&
                         (Environment.TickCount - _lastwcasted > 2000 || Environment.TickCount - _lastprocessw > 2000))
                {
                    Steps = LeeSin.steps.Flash;
                }
            }

            var wardtotargetpos = Player.Position.Extend(target.Position, Player.Distance(target) - 150);

           if (!canwardflash) return;
            
            if (Player.ServerPosition.Distance(target.ServerPosition) < 500  || target.Distance(Player) > 800 ||
                Environment.TickCount - _lastq1casted < 500 || (Q.IsReady() && col.Count == 0)
                || !CanWardFlash(target) || Environment.TickCount - LeeSin.lsatcanjump1 < 3000 || target.Buffs.Any(x => x.Name.ToLower().Contains("blindmonkqone"))) 
                return;
            if ( Q2()) return;
            if (!HasFlash()) return;
            if (LastQ(target))
            {
                WardManager.WardJump.WardJumped(wardtotargetpos, true, false);
                _wardjumpedto = Environment.TickCount;
                _wardjumpedtotarget = true;
                _lastflashward = Environment.TickCount;
            }

            #endregion
        }
    }
}