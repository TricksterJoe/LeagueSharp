using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Slutty_Gnar_Reworked
{
    internal class Gnar : MenuConfig
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static int rcast { get; set; }
        public static int lastq { get; set; }

        internal static void OnLoad(EventArgs args)
        {
            #region OnLoad

            if (Player.ChampionName != "Gnar")
                return;
            CreateMenu();
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            Orbwalking.AfterAttack += AfterAttack;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter2.OnInterruptableTarget += GnarInterruptableSpell;
            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;

            #endregion
        }

        private static void GnarInterruptableSpell(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            #region Interruptable

            if (Player.IsMegaGnar())
            {
                if (GnarSpells.WMega.IsReady()
                    && GnarSpells.WMega.IsInRange(sender)
                    && Config.Item("qwi").GetValue<bool>())
                    GnarSpells.WMega.Cast(sender.ServerPosition);
            }

            #endregion
        }

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            #region on gap closer

            if (gapcloser.Sender.IsAlly
                || gapcloser.Sender.IsMe)
                return;
            if (Player.IsMiniGnar() && Config.Item("qgap").GetValue<bool>())
            {
                if (GnarSpells.QMini.IsInRange(gapcloser.Start))
                    GnarSpells.QMini.Cast((gapcloser.Sender.Position));
            }

            #endregion
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            #region Drawings

            if (!Player.Position.IsOnScreen())
                return;

            var qDraw = Config.Item("qDraw").GetValue<bool>();
            var eDraw = Config.Item("eDraw").GetValue<bool>();
            var wDraw = Config.Item("wDraw").GetValue<bool>();
            var draw = Config.Item("Draw").GetValue<bool>();

            if (Player.IsDead || !draw)
                return;
            /*
            if (!draw) Hacks.DisableDrawings = true;
            else Hacks.DisableDrawings = false;
             */
            if (Player.IsMegaGnar())
            {
                if (qDraw && GnarSpells.QMega.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.QMega.Range, Color.Green, 3);
                if (eDraw && GnarSpells.EMega.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.EMega.Range, Color.Gold, 3);
                if (wDraw && GnarSpells.WMega.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.WMega.Range, Color.Black, 3);
            }

            if (Player.IsMiniGnar())
            {
                if (qDraw && GnarSpells.QMini.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.QMini.Range, Color.Green, 3);
                if (eDraw && GnarSpells.EMini.Level > 0)
                    Render.Circle.DrawCircle(Player.Position, GnarSpells.EMini.Range, Color.LightBlue, 3);
            }

            #endregion
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {
            #region After attack

            if (!Player.IsMiniGnar())
                return;

            var targets = TargetSelector.GetTarget(GnarSpells.QMini.Range, TargetSelector.DamageType.Physical);
            if (Player.Distance(target) <= 450)
            {
                if (GnarSpells.QnMini.IsReady())
                    GnarSpells.QnMini.Cast(targets);
            }

            #endregion
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            #region dash

            var useQ = Config.Item("UseQMini").GetValue<bool>();
            var useQm = Config.Item("UseQMega").GetValue<bool>();
            var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId
                && Player.IsMiniGnar())
            {
                if (useQ
                    && GnarSpells.QMini.IsReady()
                    && args.EndPos.Distance(Player) <= GnarSpells.QMini.Range)
                {
                    var delay = (int) (args.EndTick - Game.Time - GnarSpells.QMini.Delay - 0.1f);
                    if (delay > 0)
                    {
                        Utility.DelayAction.Add(delay*1000, () => GnarSpells.QMini.Cast(args.EndPos));
                    }
                    else
                    {
                        GnarSpells.QMini.Cast(args.EndPos);
                    }
                }
                if (sender.NetworkId == target.NetworkId
                    && Player.IsMegaGnar())
                {
                    if (useQm
                        && GnarSpells.QMini.IsReady()
                        && args.EndPos.Distance(Player) <= GnarSpells.QMega.Range)
                    {
                        var delay = (int) (args.EndTick - Game.Time - GnarSpells.QMega.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay*1000, () => GnarSpells.QMega.Cast(args.EndPos));
                        }
                        else
                        {
                            GnarSpells.QMega.Cast(args.EndPos);
                        }
                    }
                }
            }
        }

        #endregion

        private static void Game_OnUpdate(EventArgs args)
        {
            #region On Update

            KillSteal();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }

            if (Config.Item("fleekey").GetValue<KeyBind>().Active)
                Flee();

            #region force target

            var qSpell = Config.Item("focust").GetValue<bool>();
            var target = HeroManager.Enemies.Find(en => en.IsValidTarget(ObjectManager.Player.AttackRange)
                                                        &&
                                                        en.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2));
            if (qSpell && target != null)
            {
                Orbwalker.ForceTarget(target);
                Hud.SelectedUnit = target;
            }

            #endregion

            #region Auto Q

            var autoQ = Config.Item("autoq").GetValue<bool>();
            if (autoQ && target != null)
                GnarSpells.QMini.Cast(target);

            #endregion

            #endregion
        }

        private static void Flee()
        {
            #region flee

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (Player.IsMiniGnar())
            {
               
                var minionCount = MinionManager.GetMinions(Player.Position, GnarSpells.QMini.Range, MinionTypes.All,
                    MinionTeam.All);
                foreach (var minion in minionCount)
                {
                    var minionPrediction = GnarSpells.EMini.GetPrediction(minion);
                   
                    var k =
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                      && x.IsMinion
                                                                      && x.Distance(Player) <= GnarSpells.EMini.Range)
                            .OrderByDescending(x => x.Distance(Player))
                            .First();

                    if (k == null)
                        return;
                    var edm = Player.ServerPosition.Extend(minionPrediction.CastPosition,
                        Player.ServerPosition.Distance(minionPrediction.CastPosition) + GnarSpells.EMini.Range);
                    if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion
                                                                        && !type.IsDead
                                                                        && type.Distance(edm, true) < 775*775))
                    {
                        GnarSpells.EMini.Cast(edm.Extend(Game.CursorPos, Player.ServerPosition.Distance(minionPrediction.CastPosition) + GnarSpells.EMini.Range));
                    }
                }
                /*
                var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, TargetSelector.DamageType.Magical);
                if (target == null)
                    return;
                var prediction = GnarSpells.EMini.GetPrediction(target);
                var ed = Player.ServerPosition.Extend(prediction.CastPosition,
                    Player.ServerPosition.Distance(prediction.CastPosition) + GnarSpells.EMini.Range);

                if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.Team != Player.Team
                                                                    && !type.IsDead
                                                                    && type.Distance(ed, true) < 775 * 775))
                {
                    GnarSpells.EMini.Cast(prediction.CastPosition);
                }
                 */
                 
            }
        }

        #endregion

        private static void KillSteal()
        {
            #region Kill Steal

            var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, TargetSelector.DamageType.Physical);

            if (target == null)
                return;

            var qSpell = Config.Item("qks").GetValue<bool>();
            var rSpell = Config.Item("rks").GetValue<bool>();
            var eqSpell = Config.Item("qeks").GetValue<bool>();

            if (qSpell)
            {
                if (Player.IsMiniGnar())
                {
                    if (GnarSpells.QMini.IsReady()
                        && target.IsValidTarget(GnarSpells.QMini.Range - 30)
                        && target.Health <= GnarSpells.QMini.GetDamage(target))
                        GnarSpells.QMini.Cast(target);
                }
                if (Player.IsMegaGnar())
                {
                    if (GnarSpells.QMega.IsReady()
                        && target.IsValidTarget(GnarSpells.QMega.Range - 30)
                        && target.Health <= GnarSpells.QMega.GetDamage(target))
                        GnarSpells.QMega.Cast(target);
                }
            }

            if (rSpell)
            {
                if (Player.IsMegaGnar()
                    && GnarSpells.RMega.IsReady()
                    && target.Health <= GnarSpells.RMega.GetDamage(target))
                    GnarSpells.RMega.Cast(target);
            }

            if (eqSpell
                && Player.IsMiniGnar()
                && Player.Distance(target) > 1400)
            {
                var prediction = GnarSpells.EMini.GetPrediction(target);
                var ed = Player.ServerPosition.Extend(prediction.CastPosition,
                    Player.ServerPosition.Distance(prediction.CastPosition) + GnarSpells.EMini.Range);

                if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.Team != Player.Team
                                                                    && !type.IsDead
                                                                    && type.Distance(ed, true) < 775*775))
                {
                    GnarSpells.EMini.Cast(prediction.CastPosition);
                    lastq = Environment.TickCount;
                }

                var minionCount = MinionManager.GetMinions(Player.Position, GnarSpells.QMini.Range, MinionTypes.All,
                    MinionTeam.All);
                foreach (var minion in minionCount)
                {
                    var minionPrediction = GnarSpells.EMini.GetPrediction(minion);
                    var k =
                        ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                      && x.IsMinion
                                                                      && x.Distance(Player) <= GnarSpells.EMini.Range)
                            .OrderByDescending(x => x.Distance(Player))
                            .First();

                    var edm = Player.ServerPosition.Extend(minionPrediction.CastPosition,
                        Player.ServerPosition.Distance(minionPrediction.CastPosition) + GnarSpells.EMini.Range);
                    if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion != Player.IsMinion
                                                                        && !type.IsDead
                                                                        && type.Distance(edm, true) < 775*775
                                                                        && k.IsValid))
                    {
                        GnarSpells.EMini.Cast(k);
                        lastq = Environment.TickCount;
                    }
                }
                if (GnarSpells.QMini.IsReady()
                    && Environment.TickCount - lastq > 500)
                {
                    GnarSpells.QMini.Cast(target);
                }
            }

            #endregion
        }

        private static void JungleClear()
        {
            #region Jungle Clear

            var qSpell = Config.Item("UseQj").GetValue<bool>();
            var wSpell = Config.Item("UseWj").GetValue<bool>();

            var jungle = MinionManager.GetMinions(GnarSpells.QMini.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            foreach (var jungleminion in jungle)
            {
                if (!jungleminion.IsValidTarget())
                    return;

                if (Player.IsMiniGnar())
                {
                    if (qSpell && GnarSpells.QMini.IsReady())
                        GnarSpells.QMini.Cast(jungleminion);
                }
                if (Player.IsMegaGnar())
                {
                    if (wSpell && GnarSpells.WMega.IsReady())
                        GnarSpells.WMega.Cast(jungleminion);
                }
            }

            #endregion
        }

        private static void LaneClear()
        {
            #region laneclear

            var qSpell = Config.Item("UseQl").GetValue<bool>();
            var wSpell = Config.Item("UseWl").GetValue<bool>();
            var qlSpell = Config.Item("UseQlslider").GetValue<Slider>().Value;
            var wlSpell = Config.Item("UseWlslider").GetValue<Slider>().Value;

            var minions = MinionManager.GetMinions(GnarSpells.QMini.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minions == null)
                return;

            var QFarmLocation =
                GnarSpells.QMini.GetLineFarmLocation(
                    MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(GnarSpells.QMini.Range),
                        GnarSpells.QMini.Delay, GnarSpells.QMini.Width, GnarSpells.QMini.Speed,
                        Player.Position, GnarSpells.QMini.Range,
                        false, SkillshotType.SkillshotLine), GnarSpells.QMini.Width);

            var WFarmLocation =
                GnarSpells.WMega.GetLineFarmLocation(
                    MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(GnarSpells.WMega.Range),
                        GnarSpells.WMega.Delay, GnarSpells.WMega.Width, GnarSpells.WMega.Speed,
                        Player.Position, GnarSpells.WMega.Range,
                        false, SkillshotType.SkillshotLine), GnarSpells.WMega.Width);

            if (Player.IsMiniGnar())
            {
                if (qSpell && GnarSpells.QMini.IsReady() && QFarmLocation.MinionsHit > qlSpell)
                    GnarSpells.QMini.Cast(QFarmLocation.Position);
            }
            if (Player.IsMegaGnar())
            {
                if (wSpell && GnarSpells.WMega.IsReady() && WFarmLocation.MinionsHit > wlSpell)
                    GnarSpells.WMega.Cast(WFarmLocation.Position);
                if (qSpell && GnarSpells.QMega.IsReady())
                    GnarSpells.QMega.Cast(minions[0]);
            }

            #endregion
        }

        private static void Mixed()
        {
            #region mixed

            var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, TargetSelector.DamageType.Physical);
            var qSpell = Config.Item("qharras").GetValue<bool>();
            var qsSpell = Config.Item("qharras2").GetValue<bool>();
            var wSpell = Config.Item("wharras").GetValue<bool>();

            if (target == null)
                return;

            if (Player.IsMiniGnar())
            {
                if (qSpell && target.IsValidTarget(GnarSpells.QMini.Range)
                    && Player.Distance(target) > 450)
                {
                    if (!qsSpell)
                        GnarSpells.QMini.Cast(target);

                    else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                        GnarSpells.QMini.Cast(target);
                }
            }

            if (Player.IsMegaGnar())
            {
                if (wSpell && Environment.TickCount - rcast >= 600)
                    GnarSpells.WMega.Cast(target);

                if (GnarSpells.QMega.IsReady() && qSpell)
                    GnarSpells.QMega.Cast(target);
            }
        }

        #endregion

        /// <summary>
        /// Combo
        /// </summary>
        private static void Combo()
        {
            
            #region Mini Gnar

            if (Player.IsMiniGnar())
            {
                
                var target = TargetSelector.GetTarget(GnarSpells.QMini.Range, TargetSelector.DamageType.Physical);
                var qSpell = Config.Item("UseQMini").GetValue<bool>();
                var qsSpell = Config.Item("UseQs").GetValue<bool>();
                var eSpell = Config.Item("eGap").GetValue<bool>();
                if (target == null)
                    return;
                var qpred = GnarSpells.QMini.GetPrediction(target);
                var collision = GnarSpells.QMini.GetCollision(Player.Position.To2D(),
                    new List<Vector2> {qpred.CastPosition.To2D()});
                var mincol =
                    collision.Where(
                        obj => obj != null && obj.IsValidTarget() && !obj.IsDead && obj.IsMinion);

                var aiBases = mincol as Obj_AI_Base[] ?? mincol.ToArray();
                var objAiBases = mincol as IList<Obj_AI_Base> ?? aiBases.ToList();
                var count = objAiBases.Count();

                var firstcol = objAiBases.OrderBy(m => m.Distance(Player.ServerPosition, true)).FirstOrDefault();


              //  var firstcolcalc = (int) (GnarSpells.QMini.Range - firstcol.Distance(Player))/(count +0.5);

                if (qSpell && target.IsValidTarget(GnarSpells.QMini.Range)
                    && Player.Distance(target) > 450)
                {
                    if (!aiBases.Any())
                    {
                        if (!qsSpell)
                            GnarSpells.QnMini.Cast(qpred.CastPosition);

                        else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                            GnarSpells.QnMini.Cast(qpred.CastPosition);
                    }
                    else
                    {
                        if (objAiBases.Any(minc => count >= 1 && firstcol.Distance(target) <= 350))
                        {
                            if (!qsSpell)
                            {
                                GnarSpells.QMini.Cast(qpred.CastPosition);
                            }

                            else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                            {
                                GnarSpells.QnMini.Cast(qpred.CastPosition);
                            }
                        }
                    }
                }

                /*
                if (qSpell && target.IsValidTarget(GnarSpells.QMini.Range)
                    && Player.Distance(target) > 450)
                {
                    {
                        if (!qsSpell)
                            GnarSpells.QMini.Cast(target);

                        else if (target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                            GnarSpells.QMini.Cast(target);
                    }

                }
                 */


                if (eSpell
                    && Player.Distance(target) > 500
                    && target.Health <= GnarSpells.QMini.GetDamage(target))
                {
                    var minionCount = MinionManager.GetMinions(Player.Position, GnarSpells.QMini.Range, MinionTypes.All,
                        MinionTeam.All);
                    foreach (var minion in minionCount)
                    {
                        var minionPrediction = GnarSpells.EMini.GetPrediction(minion);
                        var k =
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                          && x.IsMinion
                                                                          &&
                                                                          x.Distance(Player) <= GnarSpells.EMini.Range)
                                .OrderBy(x => x.Distance(target))
                                .FirstOrDefault();

                        var edm = Player.ServerPosition.Extend(minionPrediction.CastPosition,
                            Player.ServerPosition.Distance(minionPrediction.CastPosition) + GnarSpells.EMini.Range);

                        if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion != Player.IsMinion
                                                                            && !type.IsDead
                                                                            && type.Distance(edm, true) < 775*775
                                                                            && k.IsValid)
                            && Player.Distance(target) > 300)
                        {
                            GnarSpells.EMini.Cast(k);
                        }
                    }
                }
            }

            #endregion

            #region Mega Gnar

            if (Player.IsMegaGnar())
            {
                var target = TargetSelector.GetTarget(GnarSpells.QMega.Range, TargetSelector.DamageType.Physical);
                var rSlider = Config.Item("useRSlider").GetValue<Slider>().Value;
                var emSpell = Config.Item("UseEMega").GetValue<bool>();
                var qmSpell = Config.Item("UseQMega").GetValue<bool>();
                var wSpell = Config.Item("UseWMega").GetValue<bool>();
                if (target == null)
                    return;
                if (GnarSpells.RMega.IsReady()
                    && Config.Item("UseRMega").GetValue<bool>())
                {
                    if (Player.CountEnemiesInRange(420) >= rSlider)
                    {
                        var prediction = Prediction.GetPrediction(target, GnarSpells.RMega.Delay);
                        if (GnarSpells.RMega.IsInRange(prediction.UnitPosition))
                        {
                            var direction = (Player.ServerPosition - prediction.UnitPosition).Normalized();
                            var maxAngle = 180f;
                            var step = maxAngle/6f;
                            var currentAngle = 0f;
                            var currentStep = 0f;
                            while (true)
                            {
                                if (currentStep > maxAngle && currentAngle < 0)
                                    break;

                                if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                                {
                                    currentAngle = (currentStep)*(float) Math.PI/180;
                                    currentStep += step;
                                }
                                else if (currentAngle > 0)
                                    currentAngle = -currentAngle;

                                Vector3 checkPoint;
                                if (currentStep == 0)
                                {
                                    currentStep = step;
                                    checkPoint = prediction.UnitPosition + 500*direction;
                                }
                                else
                                    checkPoint = prediction.UnitPosition + 500*direction.Rotated(currentAngle);

                                if (prediction.UnitPosition.GetFirstWallPoint(checkPoint).HasValue
                                    && !target.IsStunned
                                    &&
                                    target.Health >=
                                    (GnarSpells.QMega.GetDamage(target) + Player.GetAutoAttackDamage(target)))
                                {
                                    GnarSpells.RMega.Cast(Player.Position +
                                                          500*(checkPoint - prediction.UnitPosition).Normalized());
                                    rcast = Environment.TickCount;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (GnarSpells.EMega.IsReady()
                    && target.Distance(Player) > 500
                    && emSpell
                    && target.HealthPercent <= Player.HealthPercent)
                    GnarSpells.EMega.Cast(target);

                if (wSpell && Environment.TickCount - rcast >= 600)
                    GnarSpells.WMega.Cast(target);

                if (qmSpell
                    && Environment.TickCount - rcast >= 700)
                {
                    if (target.Distance(Player) <= 130)
                    {
                        if (GnarSpells.QnMega.IsReady())
                            GnarSpells.QnMega.Cast(target);
                    }
                    else if (GnarSpells.QMega.IsReady())
                        GnarSpells.QMega.Cast(target);
                }
            }

            #endregion
        }
    }
}