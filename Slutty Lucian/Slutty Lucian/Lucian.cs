using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace Slutty_Lucian
{
    internal class Lucian : Helper
    {
        public static Spell Q, W, E, R;
        private static bool passive;
        private static bool casted;
        private static bool castq;

        internal static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Lucian") return;
            MenuConfig.OnLoad();
            Q = new Spell(SpellSlot.Q, 1100);
            R = new Spell(SpellSlot.R, 1400);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E);
            Q.SetTargetted(0.2f, float.MaxValue);
            W.SetSkillshot(0.4f, 150f, 1600, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.2f, 110f, 2500, true, SkillshotType.SkillshotLine);
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            CustomEvents.Unit.OnDash += Ondash;
        }

        private static void Ondash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (sender.IsMe)
            {
                passive = true;
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
        //      Game.PrintChat(args.SData.Name);
            if (args.SData.Name == "LucianW" || args.SData.Name == "LucianE" || args.SData.Name == "LucianQ")
            {
                passive = true;
               Orbwalking.ResetAutoAttackTimer();
            }

        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W || args.Slot == SpellSlot.E)
            {
                passive = true;
                Utility.DelayAction.Add(150, () => passive = false);
            }
           
        }


        private static void OnDraw(EventArgs args)
        {
            if (GetBool("drawq", typeof (bool)) && Q.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, Q.Range, Color.LightSeaGreen);
            }

            if (GetBool("draww", typeof(bool)) && W.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, W.Range, Color.DarkOrchid);
            }

            if (GetBool("drawe", typeof(bool)) && E.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, E.Range, Color.DarkRed);
            }

            if (GetBool("drawr", typeof(bool)) && R.Level >= 1)
            {
                Drawing.DrawCircle(Player.Position, R.Range, Color.Chartreuse);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            RDamage(Player);
           if (Player.IsDashing() || Player.HasBuff("lucianpassivebuff") || casted) return;
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    if (ValidTarget(R.Range))
                    {
                        if (passive) return;
                        if (passive || Player.IsDashing() || Player.HasBuff("lucianpassivebuff")) return;
                        var targets = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        var targetsr = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

                        WCast(targets, "usewc");
                        ExtendedQ(targets, "useqc", "useqcs");
                        RCast(targetsr, "userc");
                       Utility.DelayAction.Add(1000, () =>  ECast(targets, "useec"));
                    }
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                    case   Orbwalking.OrbwalkingMode.Mixed:
                    if (ValidTarget(Q.Range))
                    {
                        var targets = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
                        if (ManaCheck("minmanah")) return;
                        ExtendedQ(targets, "useqh", "useqhs");
                        WCast(targets, "usewh");
                    }
                    break;
            }

            if (ValidTarget(R.Range))
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
                //the check + int to prevent weird stutters
                if (Player.HasBuff("lucianr") && GetBool("usercmove", typeof(bool)))
                {
                    if ((int) Player.ServerPosition.Y != (int) target.ServerPosition.Y ||
                        (int) Player.ServerPosition.Z != (int) target.ServerPosition.Z)
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo,
                            new Vector3(Game.CursorPos.X, (int) target.ServerPosition.Y,
                                (int) Player.ServerPosition.Z).To2D().To3D());
                    }
                }
            }
        }

        public static bool ValidTarget(float range)
        {
            var targets = TargetSelector.GetTarget(range, TargetSelector.DamageType.Physical);
            return targets != null;
        }


        public static void LaneClear()
        {
            if (ManaCheck("minmanal")) return;
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minion == null) return;

            foreach (var minions in minion)
            {
                var prediction = Prediction.GetPrediction(minions, Q.Delay, 10);

                var collision = Q.GetCollision(Player.Position.To2D(),
                    new List<Vector2> {prediction.UnitPosition.To2D()});
                foreach (var collisions in collision)
                {
                    if (collision.Count() >= GetValue("xminions") && GetBool("useq", typeof (bool)) && Q.IsReady())

                    {
                        if (collision.Last().Distance(Player) - collision[0].Distance(Player) < 600 &&
                            collision[0].Distance(Player) < 500)
                            Q.Cast(collisions);
                    }
                }

                // var wprd = W.GetLineFarmLocation(minion);
                if (GetBool("usew", typeof (bool)) && W.IsReady())
                {
                    W.Cast(minions);
                }

            }
        }

        public static void RCast(Obj_AI_Hero target, string name)
        {
            if (!GetBool(name, typeof (bool))) return;
            if (target == null) return;
            if (target.Distance(Player) < Player.AttackRange+ Player.BoundingRadius) return;
            if (R.IsReady() && !Player.HasBuff("lucianr"))
            {
                if (!E.IsReady() && (target.Health < RDamage(Player)))
                R.Cast(target);
            }
        }

        public static float RDamage(Obj_AI_Hero hero)
        {
            double damage = 0;
            var shots = 7.5 + 10.5*hero.AttackCastDelay*1000;
            var damageperbolt = 65 + 0.4*hero.TotalMagicalDamage + 0.33*hero.TotalAttackDamage;
            if (R.IsReady())
            {
                damage += shots*damageperbolt;
            }

            return (float) damage;
        }
        public static void ExtendedQ(Obj_AI_Hero target, string menuname, string smart)
        {
            if (Player.HasBuff("lucianpassivebuff") || Player.IsDashing() || Player.IsWindingUp) return;
            if (!GetBool(menuname, typeof (bool))) return;

            if (target == null) return;
            var prediction = Prediction.GetPrediction(target, Q.Delay, 10);
            
            var collision = Q.GetCollision(Player.Position.To2D(),
                new List<Vector2> {prediction.UnitPosition.To2D()});
            

            if (!Q.IsReady()) return;

            if (collision.Any() && GetBool(smart, typeof(bool)))
            {
                    if (target.Distance(Player) <= Q.Range && collision[0].Distance(Player) <= 500 )
                        Q.Cast(collision[0]);
            }
            else
            {
                if (target.IsValidTarget(500))
                    Q.Cast(target);
            }
        }

        public static void WCast(Obj_AI_Hero target, string name)
        {
            if (!GetBool(name, typeof (bool))) return;
            if (Player.HasBuff("lucianpassivebuff") || Player.IsDashing() || Player.IsWindingUp) return;

            if (W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.Cast(target.Position);
            }
        }

        public static void ECast(Obj_AI_Hero target, string name)
        {
            if (!GetBool(name, typeof (bool))) return;

            if (E.IsReady() && target.IsValidTarget(Q.Range) && !Player.HasBuff("lucianr"))
            {
                casted = true;
                E.Cast(Game.CursorPos);
            }
            if (casted)
            {
                Utility.DelayAction.Add(1000, () => casted = false);
            }
        }
    }
}
