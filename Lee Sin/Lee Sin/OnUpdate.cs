using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Lee_Sin.ActiveModes;
using Lee_Sin.Drawings;
using Lee_Sin.Misc;
using Prediction = Lee_Sin.Prediction;

namespace Lee_Sin
{
    class OnUpdate : LeeSin
    {



        public static void CastSpell(Spell QWER, Obj_AI_Base target)
        {
            switch (GetStringValue("PredictionMode"))
            {
                case 0:
                {
                    const SkillshotType coreType2 = SkillshotType.SkillshotLine;

                    var predInput2 = new PredictionInput
                    {
                        Collision = QWER.Collision,
                        Speed = QWER.Speed,
                        Delay = QWER.Delay,
                        Range = QWER.Range,
                        From = Player.ServerPosition,
                        Radius = QWER.Width,
                        Unit = target,
                        Type = coreType2
                    };
                    var poutput2 = Prediction.GetPrediction(predInput2);
                    // if (poutput2 == null) return;
                    if (poutput2.Hitchance >= HitChance.High || poutput2.Hitchance == HitChance.Immobile ||
                        poutput2.Hitchance == HitChance.Dashing)
                    {
                        QWER.Cast(poutput2.CastPosition);
                    }
                    break;
                }
                case 1:
                    var pred = Q.GetPrediction(target);
                    if (pred.Hitchance >= LeagueSharp.Common.HitChance.High ||
                        pred.Hitchance == LeagueSharp.Common.HitChance.Immobile)
                    {
                        if (pred.CollisionObjects.Count == 0)
                            Q.Cast(pred.CastPosition);
                    }
                    break;
            }

        }


        public static
            void OnUpdated(EventArgs args)
        {
            ProcessHandler.ProcessHandlers();
         //   Game.PrintChat(Insec.InsecTo.canwardflash.ToString());
            if (Player.IsRecalling() || MenuGUI.IsChatOpen) return;

            if (GetBool("smiteenable", typeof (KeyBind)))
            {
                ActiveModes.Smite.AutoSmite();
            }
            if (GetBool("wardjump", typeof (KeyBind)))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                WardManager.WardJump.WardJumped(Player.Position.Extend(Game.CursorPos, 590), true, true);
            }

            if (GetBool("wardinsec", typeof (KeyBind)))
            {
                Insec.InsecTo.insec();
            }

            if (GetBool("starcombo", typeof (KeyBind)))
            {
                ActiveModes.Star.StarCombo();
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    ActiveModes.ComboMode.Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear.Lane();
                    LaneClear.Lane2();
                    JungleClear.Jungle();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass.Harassed();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LaneClear.LastHit();
                    break;
            }
            AutoUlt.AutoUlti();
            if (GetBool("activatebubba", typeof (KeyBind)))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                BubbaKush.DrawRect();
            }
            var target = TargetSelector.GetTarget(Q.Range + 800, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;

                if (target == null) return;
                LastQ(target);
                CanWardFlash(target);
            }
        }
    }
}
