using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Slutty_Gragas
{
    static class  Gragas
    {
        public static Spell R, Q, Q2,E;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Geometry.Polygon.Circle CirclePoly;
        private static Vector3 targetposition;
        private static Vector3 secondtargetpositon;
        private static int timeinside;
        private static int rcastedtime;
        private static List<Vector2> newPoly;
        private static Vector2 cricleendpos;
        public static GameObject Barrel;
        internal static void OnLoad(EventArgs args)
        {
            MenuConfig.OnLoad();
            R = new Spell(SpellSlot.R, 1000);
            Q = new Spell(SpellSlot.Q, 500);
            E = new Spell(SpellSlot.E, 950);
            Q2 = new Spell(SpellSlot.Q, float.MaxValue);
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnCreate += OnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            Drawing.OnDraw += OnDraw;
            R.SetSkillshot(0.25f, 375, 1800f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0f, 200, 1200f, true, SkillshotType.SkillshotLine);
        }



        private static void OnCreate(GameObject sender, EventArgs args)
        {
           // if (args == EventArgs.Empty) return;
          //  Game.PrintChat(sender.Name);

            if (sender.Name.ToLower().Contains("gragas_base_q"))
            {
                if (sender.Name.ToLower().Contains("ally"))
                {
                    Barrel = sender;
                    if (Barrel != null)
                        CirclePoly = new Geometry.Polygon.Circle(Barrel.Position, 300);
                }

                if (!sender.Name.ToLower().Contains("end")) return;
                Barrel = null;
                CirclePoly = null;
            }
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.R)
                {
                    targetposition = target.Position;
                    rcastedtime = Environment.TickCount;
                    if (Barrel != null)
                    {
                        if (Barrel.Position.Distance(Player.ServerPosition) < E.Range)
                        {
                            var dashspeed = target.GetDashInfo().Speed/1000;
                            var dashstart = target.GetDashInfo().StartPos;
                            var dashend = target.GetDashInfo().EndPos;
                            cricleendpos = dashend;
                            var ddistance = dashstart.Distance(dashend);
                            var calculatedspeed = (float) dashspeed;
                            var delay = ddistance/calculatedspeed;
                            Utility.DelayAction.Add((int)delay, () => E.Cast(dashend));
                        }
                    }
                }
            }
        }


//        public static Geometry.Polygon RandomPoints
//        {
//            get
//            {
//                var polygon = new Geometry.Polygon();
//                {
//                    polygon.Add(new Vector2());
//                    polygon.Add(new Vector2());
//                    polygon.Add(new Vector2());
//                }
//                return polygon;
//            }
//        }
        private static void OnUpdate(EventArgs args)
        {
          //  Game.PrintChat((Barrel != null).ToString());
       var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
           if (target == null) return;
            RCasting();
            //   Gam.PrintChat(TargetSpeed(target).ToString());
        }

        public static float TargetSpeed(Obj_AI_Hero target)
        {
          // if (CirclePoly == null || Barrel == null) return 0;
            var pos = targetposition;
            var travel = (float) Barrel.Position.Distance(pos);
            if (target.Distance(Barrel.Position) < 300)
            {
                timeinside = Environment.TickCount;
            }

            var time = (float) timeinside - rcastedtime;
            var speed = travel/time;
            return speed;

        }
        private static void RCasting()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            var barr = Barrel;
            var pos = RCastPosition(target, barr);
            if (pos.IsValid() && R.IsReady())
            {
                R.Cast(pos);
            }
            if (CirclePoly == null) return;
            if (Q2.IsReady() && CirclePoly.IsInside(target))
            {
                Q2.Cast();
            }
        }
        private static void OnDraw(EventArgs args)
        {
            if (cricleendpos != null)
            {
                Drawing.DrawCircle(cricleendpos.To3D(), 100, Color.Blue);
            }
            if (Barrel != null)
            {
               Render.Circle.DrawCircle(Barrel.Position, 270, Color.Blue, 5);
            }

            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            var pos = target.RCastPosition(Barrel);
            if (pos.IsValid())
            {
                Render.Circle.DrawCircle(pos, 200, Color.BlueViolet);
            }


        }
        public static Vector3 RCastPosition(this Obj_AI_Hero unit, GameObject barrel)
        {
           if (barrel == null) return new Vector3();
            var barellposition = barrel.Position;
            var playerposition = Player.ServerPosition;
            var distancepredcition = playerposition.Distance(unit.ServerPosition);
            var delay = distancepredcition/(R.Speed);
            const int knockdistnace = 1200;
            var predict = Prediction.GetPrediction(unit, delay, unit.BoundingRadius);
            var unitposition = predict.CastPosition;
            var checkdistance = barellposition.Distance(unitposition);
            float playerdistancecheck = playerposition.Distance(unitposition);
            var valueextend = (R.Width/2);
            var castposition = barellposition.Extend(unitposition, valueextend + checkdistance);
            if (checkdistance < knockdistnace && playerdistancecheck < R.Range && predict.Hitchance >= HitChance.VeryHigh)
                return castposition;
            return new Vector3();
        }

    }
}
