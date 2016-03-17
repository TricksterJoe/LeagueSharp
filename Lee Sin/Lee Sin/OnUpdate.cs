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
using Lee_Sin.WardManager;
using Prediction = Lee_Sin.Prediction;

namespace Lee_Sin
{
    class OnUpdate : LeeSin
    {
        private static float _g;


        public static void CastSpell(Spell qwer, Obj_AI_Base target)
        {
            switch (GetStringValue("PredictionMode"))
            {
                case 0:
                {
                    const SkillshotType coreType2 = SkillshotType.SkillshotLine;

                    var predInput2 = new PredictionInput
                    {
                        Collision = qwer.Collision,
                        Speed = qwer.Speed,
                        Delay = qwer.Delay,
                        Range = qwer.Range,
                        From = Player.ServerPosition,
                        Radius = qwer.Width,
                        Unit = target,
                        Type = coreType2
                    };
                    var poutput2 = Prediction.GetPrediction(predInput2);
                    // if (poutput2 == null) return;
                    if (poutput2.Hitchance >= HitChance.High || poutput2.Hitchance == HitChance.Immobile ||
                        poutput2.Hitchance == HitChance.Dashing)
                    {
                        qwer.Cast(poutput2.CastPosition);
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
            //if (Player.HasBuff("blindmonkqtwodash"))
            //{
            //    Game.PrintChat("dash");
            //}
          //  Console.WriteLine(Environment.TickCount - LeeSin.lasttotarget);
           // ProcessHandler.ProcessHandlers();
           // Misc.BubbaKush.DrawRect();
          //  BubbaKushPos.ResolveBubbaPosition.GetPosition();
            //WardSorter.HasPoachers();
           // WardSorter.Wards();
          //  Game.PrintChat(WardSorter.HasPoachers().ToString());
          //  Game.PrintChat(ItemReady(3711).ToString());
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
                Insec.InsecTo.Insec();
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


            var target = TargetSelector.GetTarget(Q.Range + 800, TargetSelector.DamageType.Physical);
            if (target == null) return;
            target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;

            if (target == null) return;

            //Console.WriteLine(target.Buffs.Where(x => x.Name.ToLower().Contains("blindmonkqone")).Any());
            LastQ(target);
            CanWardFlash(target);
        }
    }
}
