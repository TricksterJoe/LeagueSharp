using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Slutty_Kindred
{
    class Kindred : Helper
    {
        public const string ChampName = "Kindred";
        public static Spell Q, W, E, R;
        internal static void OnLoad(EventArgs args)
        {
    //            if (Player.ChampionName != ChampName)
    //                return;

            Q = new Spell(SpellSlot.Q, 800);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E, Player.AttackRange + Player.BoundingRadius);
            R = new Spell(SpellSlot.Q, 1000);

            MenuConfig.OnLoad();
            
            Game.OnUpdate += OnUpdate;
            Orbwalking.AfterAttack += AfterAttack;
            Orbwalking.BeforeAttack += BeforeAttack;
            Obj_AI_Base.OnIssueOrder += OnIssueOrder;
            Drawing.OnDraw += OnDraw;
            CustomEvents.Unit.OnDash += Ondash;
        }

        private static void Ondash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsMe || sender.IsAlly) return;
            var endpos = args.EndPos;
            if (endpos.Distance(Player) < 300)
            {
                Q.Cast(endpos.Extend(Player.ServerPosition.To2D(), Q.Range));
            }
        }


        private static void OnDraw(EventArgs args)
        {
            if (!GetBool("draws", typeof (bool))) return;

            if (Q.Level >= 1 && GetBool("drawq", typeof (bool)))
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Red, 4);
            }

            if (W.Level >= 1 && GetBool("draww", typeof(bool)))
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.BlueViolet, 4);
            }

            if (E.Level >= 1 && GetBool("drawe", typeof(bool)))
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Blue, 4);
            }

            if (R.Level >= 1 && GetBool("drawr", typeof(bool)))
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Black, 4);
            }
        }

        private static void OnIssueOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
//            if (!sender.IsMe || args.Order != GameObjectOrder.AutoAttack) return;
//
//            var siegeminion =
//    MinionManager.GetMinions(Player.Position, 800).FirstOrDefault(x => x.Name.Contains("siege"));
//
//            if (siegeminion != null && siegeminion.HealthPercent < 20)
//            {
//                foreach (
//                    var minion in
//                        MinionManager.GetMinions(Player.Position, 800))
//                {
//                    args.Process = false;
//                }
//            }
//            else if (siegeminion.HealthPercent < 20)
//            {

//            }
        }


        private static void Wallhops()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var behindPosition = ObjectManager.Player.Position.To2D()+ 50;

            var extendedPosition = ObjectManager.Player.ServerPosition.Extend(Game.CursorPos, 300);
            if (behindPosition.IsWall()  && Game.CursorPos.IsWall())
            {
                E.Cast(extendedPosition);
            }
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {

            // soontm
//            foreach (
//                var target in
//                    HeroManager.Enemies.Where(
//                        x =>
//                            x.IsValid && x.Distance(Player) < Player.AttackRange && GetPassiveBuff > 0 &&
//                            x.IsVisible))
//            {
//                if (GetBool("forceetarget", typeof (bool)))
//                    Orbwalker.ForceTarget(target);
//            }
        }

        private static void AfterAttack(AttackableUnit unit, AttackableUnit target)
        {

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None) return;

            var dashPosition = Player.Position.Extend(Game.CursorPos, Q.Range);
            if (!Q.IsReady() || !target.IsValidTarget(840)) return;

            switch (GetStringValue("qmode"))
            {
                case 0:
                    Q.Cast(dashPosition);
                    Utility.DelayAction.Add(100 + Game.Ping, Orbwalking.ResetAutoAttackTimer);
                    break;
                case 1:
                    if (target.Position.Distance(Player.Position) < 500)
                    {
                        Backwardscast(SpellSlot.Q, target);
                        Utility.DelayAction.Add(100 + Game.Ping, Orbwalking.ResetAutoAttackTimer);
                    }
                    break;
            }
        }

        private static void OnUpdate(EventArgs args)
        {

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    Jungleclear();
                    break;
            }

            if (GetBool("wallhops", typeof (KeyBind)))
            {
                Wallhops();
            }
        }

        private static void Jungleclear()
        {
            var minions =
    MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Neutral,
        MinionOrderTypes.MaxHealth);
            var dashPosition = Player.Position.Extend(Game.CursorPos, Q.Range);
            if (minions == null) return;
            foreach (var minion in minions)
            {
                if (W.IsReady() && GetBool("usewjungleclear", typeof(bool)))
                    W.Cast();

                if (Q.IsReady() && GetBool("useqjungleclear", typeof(bool)))
                {
                    Q.Cast(dashPosition);
                }

                if (E.IsReady() && GetBool("useejungleclear", typeof(bool)))
                    E.Cast(minion);
            }
        }

        private static void Laneclear()
        {
            if (GetValue("minmana") > Player.ManaPercent) return;

            var minions =
                MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth);

            if (minions == null) return;

            if (W.IsReady() &&
                GetBool("usewl", typeof(bool)) && minions.Count >= GetValue("wminslider"))
                W.Cast();

            if (Q.IsReady() &&
                GetBool("useql", typeof(bool)) && minions.Count >= GetValue("qminslider"))
                Q.Cast(Game.CursorPos);
        }

        private static void Combo()
        {


            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            var expires = Player.GetSpell(SpellSlot.Q).CooldownExpires; // hi idk how to use Q.IsReady(x);
            var CD = (int)
                    (expires -
                     (Game.Time - 1));



            if (W.IsReady())
            {
                if (Q.IsReady() || CD > 3.5 || target.HealthPercent < 15)
                    W.Cast();
            }


            var dashPosition = Player.Position.Extend(Game.CursorPos, Q.Range);
            switch (GetStringValue("qmode")) 
            {
                case 0:
                {
                    if (Player.Distance(target) > 500 && Player.Distance(target) < Q.Range)
                    {
                        Q.Cast(dashPosition);
                    }
                    break;
                }
                case 1:
                if (Player.Distance(target) > 500 && Player.Distance(target) < Q.Range)
                    {
                        Q.Cast(dashPosition);
                    }
                    break;
            }

            if (E.IsReady() && target.IsValidTarget(E.Range))
                E.Cast(target);
            
            foreach (var hero in HeroManager.Allies)
            {
                if (!R.IsReady() || Player.CountAlliesInRange(R.Range) < GetValue("minallies") ||
                    Player.CountAlliesInRange(R.Range) < GetValue("minenemies")) continue;

                if (hero.HealthPercent < GetValue("minhpr"))
                {
                    R.Cast();
                }
            }
        }
    }
}
