﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using SPrediction;

namespace Slutty_Veigar
{
    internal class Veigar : Helper
    {
        private static readonly Items.Item TearoftheGoddess = new Items.Item(3070);
        private static readonly Items.Item TearoftheGoddessCrystalScar = new Items.Item(3073);
        private static readonly Items.Item ArchangelsStaff = new Items.Item(3003);
        private static readonly Items.Item ArchangelsStaffCrystalScar = new Items.Item(3007);
        private static readonly Items.Item Manamune = new Items.Item(3004);
        private static readonly Items.Item ManamuneCrystalScar = new Items.Item(3008);
        public static SpellSlot Ignite;
        public static Spell Q, W, E, Ew, R;
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");
        private static readonly Color _color = Color.Red;
        private static readonly Color FillColor = Color.Blue;

        internal static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Veigar")
                return;

            MenuConfig.OnLoad();
            Config.AddToMainMenu();
            
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 880);
            E = new Spell(SpellSlot.E, 800);
            R = new Spell(SpellSlot.R, 650);
            SPrediction.Prediction.Initialize(Config);
            DamageToUnit = GetComboDamage;

            Q.SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(2f, 225f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1500f, false, SkillshotType.SkillshotCircle);
          //  Ew.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
           // GameObject.OnCreate += OnCreate;
           // CustomEvents.Unit.OnDash += Ondash;
        }
//
//        private static void Ondash(Obj_AI_Base sender, Dash.DashItem args)
//        {
//            if (sender.IsAlly || sender.IsMe || !sender.IsChampion()) return;
//            E.Cast(args.EndPos.Extend(Player.Position.To2D(), 375));
//        }




//        private static void OnCreate(GameObject sender, EventArgs args)
//        {
//           // Game.PrintChat(sender.Name);
//            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
//            if (target == null) return;
//            if (sender.Name == "Veigar_Base_E_cage_green.troy")
//            {
//                if (sender.Position.Distance(Player.ServerPosition) < 375)
//                {
//                    W.Cast(target.Position);
//                }
//            }
//        }

        public static SpellSlot GetIgniteSlot()
        {
            return _ignite;
        }

        public static void SetIgniteSlot(SpellSlot nSpellSlot)
        {
            _ignite = nSpellSlot;
        }

        private static void OnUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(true);
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    if (!GetBool("autoqtoggle", typeof (KeyBind))) return;
                    if (GetStringValue("autoq") == 0 || GetStringValue("autoq") == 1)
                        if (!Player.IsRecalling())
                        autoqs();
                    break;
            }
            TearStack();
            KillSteal();
            if (GetBool("fleemode", typeof (KeyBind)))
                flee();

            SetIgniteSlot(Player.GetSpellSlot("summonerdot"));
            Orbwalker.SetAttack(!GetBool("aablock", typeof(bool)));

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            GetComboDamage(target);

            if (GetBool("autoe", typeof(bool)))
            autoe(target);

            if (target.HasBuffOfType(BuffType.Stun))
                W.Cast(target.Position);

//
//            if (target.IsValidTarget(E.Range) && E.IsReady() && target.HasBuffOfType(BuffType.Stun))
//            {
//                var pred = E.GetPrediction(target).CastPosition;
//                E.Cast(pred.Extend(Player.ServerPosition, 375));
//            }

            if (Ignite.IsReady() && target.Health <= (Q.GetDamage(target) + Player.GetAutoAttackDamage(target)))
                Player.Spellbook.CastSpell(Ignite, target);
        }

        private static void KillSteal()
        {
            var ksq = GetBool("useqks", typeof (bool));
            var ksw = GetBool("usewks", typeof(bool));
            var ksr = GetBool("userks", typeof(bool));
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (ksq && target.Health <= Q.GetDamage(target))
            {
                Q.Cast(target);
            }

            if (ksw && target.Health <= W.GetDamage(target))
            {
                W.Cast(target.Position);
            }
            foreach (var hero in HeroManager.Enemies)
            {
                if (ksr && hero.Health <= R.GetDamage(hero) && GetStringValue("user" + hero.ChampionName) == 0)
                {
                    R.Cast(target);
                }
            }

        }

        private static void autoe(Obj_AI_Hero target)
        {
            var pred = W.GetPrediction(target, true);
            var predss = pred.AoeTargetsHitCount;

            if (predss >=  GetValue("AutoE") - 1)
            {
                E.Cast(pred.CastPosition);
            }
        }

        private static void autoqs()
        {
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minion != null)
            {
                if ((GetStringValue("autoq") == 0 || GetStringValue("autoq") == 1))
                {
                    foreach (var minions in minion.Where(x => x.Name.Contains("siege")))
                    {

                        if (minions.Health <= Q.GetDamage(minions))
                            Q.Cast(minions);
                    }
                }

                foreach (var minions in minion)
                {
                    if (GetStringValue("autoq") == 1)
                    {
                        if (minions != null)
                        {
                            if (minions.Health <= Q.GetDamage(minions))
                                Q.Cast(minions);
                        }
                    }

                    var prediction = LeagueSharp.Common.Prediction.GetPrediction(minions, Q.Delay);

                    var collision = Q.GetCollision(Player.Position.To2D(),
                        new List<Vector2> {prediction.UnitPosition.To2D()});
                    foreach (var collisions in collision)
                    {
                        if (collision.Count == 2 && collisions.IsMinion)
                        {
                            switch (GetStringValue("autoq"))
                            {
                                case 0:
                                    if ((collision[0].Health < Q.GetDamage(collision[0])) &&
                                        (collision[1].Health < Q.GetDamage(collision[1])))
                                        Q.Cast(prediction.CastPosition);
                                    break;
                                case 1:
                                    if ((collision[0].Health < Q.GetDamage(collision[0])) ||
                                        (collision[1].Health < Q.GetDamage(collision[1])))
                                    {
                                        Q.Cast(prediction.CastPosition);
                                    }
                                    break;
                                case 2:

                                    break;
                            }
                        }
                    }
                }
            }
        }


        private static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            EComboHarasscast("efleemode", target);
        }

        private static void LastHit()
        {
            if (ManaCheck("minmanalast")) return;
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minion == null) return;

            if (!GetBool("useqlaneclearlast", typeof(bool))) return;

            foreach (var minions in minion.Where(x => x.Name.Contains("siege")))
            {
                if (minions.Health <= Q.GetDamage(minions))
                    Q.Cast(minions);
            }
            if (GetStringValue("qmodelast") == 1)
            {
                if (minion[0].Health <= Q.GetDamage(minion[0]))
                    Q.Cast(minion[0]);
            }

            foreach (var minions in minion)
            {
                var prediction = LeagueSharp.Common.Prediction.GetPrediction(minions, Q.Delay);

                var collision = Q.GetCollision(Player.Position.To2D(),
                    new List<Vector2> { prediction.UnitPosition.To2D() });
                foreach (var collisions in collision)
                {
                    if (collision.Count == 2 && collisions.IsMinion)
                    {
                        switch (GetStringValue("qmodelast"))
                        {
                            case 0:
                                if ((collision[0].Health < Q.GetDamage(collision[0])) &&
                                    (collision[1].Health < Q.GetDamage(collision[1])))
                                    Q.Cast(prediction.CastPosition);
                                break;
                            case 1:
                                if ((collision[0].Health < Q.GetDamage(collision[0])) ||
                                    (collision[1].Health < Q.GetDamage(collision[1])))
                                {
                                    Q.Cast(prediction.CastPosition);
                                }
                                break;
                            case 2:
                                break;
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (!ManaCheck("minmanajungle")) return;

            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minion == null) return;



            foreach (var minions in minion)
            {
                if (GetBool("useqjungle", typeof(bool)) && Q.IsReady())
                {
                    Q.Cast(minions);
                }

                if (GetBool("usewjungle", typeof(bool)))
                {
                    var wcircle = W.GetCircularFarmLocation(minion);
                    if (wcircle.MinionsHit >= 2 && W.IsReady())
                    {
                        W.Cast(wcircle.Position);
                    }
                }
            }
        }
        private static void LaneClear()
        {
            if (ManaCheck("minmana")) return;
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minion == null) return;

            if (GetBool("usewlaneclear", typeof(bool)))
            {
                var wcircle = W.GetCircularFarmLocation(minion);
                if (wcircle.MinionsHit >= GetValue("wminionjigolo") && W.IsReady())
                    W.Cast(wcircle.Position);
            }


            if (!GetBool("useqlaneclear", typeof(bool))) return;

            if (GetStringValue("qmode") == 1)
            {
                if (minion[0].Health <= Q.GetDamage(minion[0]))
                    Q.Cast(minion[0]);
            }

            foreach (var minions in minion.Where(x => x.Name.Contains("siege")))
            {
                if (minions.Health <= Q.GetDamage(minions))
                    Q.Cast(minions);
            }
            if (GetStringValue("qmode") == 1)
            {
                if (minion[0].Health <= Q.GetDamage(minion[0]))
                    Q.Cast(minion[0]);
            }
            foreach (var minions in minion)
            {
                var prediction = LeagueSharp.Common.Prediction.GetPrediction(minions, Q.Delay);

                var collision = Q.GetCollision(Player.Position.To2D(),
                    new List<Vector2> { prediction.UnitPosition.To2D() });
                foreach (var collisions in collision)
                {
                    if (collision.Count == 2 && collisions.IsMinion)
                    {
                        switch (GetStringValue("qmode"))
                        {
                            case 0:
                                if ((collision[0].Health < Q.GetDamage(collision[0])) &&
                                    (collision[1].Health < Q.GetDamage(collision[1])))
                                    Q.Cast(prediction.CastPosition);
                                break;
                            case 1:
                                if ((collision[0].Health < Q.GetDamage(collision[0])) ||
                                    (collision[1].Health < Q.GetDamage(collision[1])))
                                {
                                    Q.Cast(prediction.CastPosition);
                                }
                                break;
                            case 2:
                                break;
                        }
                    }
                }
            }
        }


        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            QComboHarassCast("useqharass", target);
            EComboHarasscast("useeharass", target);
            WComboHarassCast("usewmodeharass", target);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            if (_ignite.IsReady() && target.Health <= (Q.GetDamage(target) + Player.GetAutoAttackDamage(target)))
            {
                Player.Spellbook.CastSpell(_ignite, target);
            }
            
            RCast("users", target);
            EComboHarasscast("useecombo", target);
            QComboHarassCast("useqcombo", target);
            WComboHarassCast("usewmode", target);
        }

        public static void QComboHarassCast(string name, Obj_AI_Hero target)
        {
            if (!GetBool(name, typeof(bool))) return;
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
    MinionOrderTypes.MaxHealth);

            if (minion == null)
            {
                QColCast(target);
            }
            else
            {
                QColCast(target, false);
            }

        }

        public static void RCast(string name, Obj_AI_Hero target)
        {
            if (target.HasBuffOfType(BuffType.Invulnerability)) return;
            if (!GetBool(name, typeof(bool))) return;
            if (!R.IsReady() || !target.IsValidTarget(R.Range)) return;
            var prediction = LeagueSharp.Common.Prediction.GetPrediction(target, Q.Delay);

            var collision = Q.GetCollision(Player.Position.To2D(),
                new List<Vector2> { prediction.UnitPosition.To2D() });
            foreach (var targets in HeroManager.Enemies)
            {
            if (
                (R.GetDamage(targets) + Q.GetDamage(targets) >= targets.Health && Q.IsReady() && !collision.Any()) ||
                (R.GetDamage(targets) + W.GetDamage(targets) >= targets.Health && W.IsReady() && target.HasBuffOfType(BuffType.Stun) && target.Distance(Player) < W.Range) ||
                (R.GetDamage(targets) + Q.GetDamage(targets) + IgniteDamage(targets) >= targets.Health && Ignite.IsReady() && Q.IsReady() && !collision.Any()) ||
                (R.GetDamage(targets) + IgniteDamage(targets) >= targets.Health && Ignite.IsReady()))
            {
                    if (GetStringValue("user" + targets.ChampionName) == 0)
                        R.Cast(targets);
                }
            }
        }

        public static void QColCast(Obj_AI_Hero target, bool col = true)
        {
            var prediction = LeagueSharp.Common.Prediction.GetPrediction(target, Q.Delay);

            var collision = Q.GetCollision(Player.Position.To2D(),
                new List<Vector2> { prediction.UnitPosition.To2D() });
            if (col)
            {
              
                if (collision.Count == 2 && collision[0].IsValid && collision[1].IsValid)
                {
                    if (collision[0].IsMinion &&
                        collision[1].Target.IsEnemy)
                    {
                        if ((collision[0].Health < Q.GetDamage(collision[0])))
                            Q.Cast(prediction.CastPosition);
                    }
                    if (collision[0].IsEnemy && collision[1].IsEnemy)
                    {
                        Q.Cast(prediction.CastPosition);
                    }

                }
            }
            else if (!collision.Any())
            {
                var qpreds = Q.GetPrediction(target);
                Q.Cast(qpreds.CastPosition);
            }
        }

        public static void EComboHarasscast(string name, Obj_AI_Hero target)
        {
            if (!GetBool(name, typeof(bool))) return;

            if (!target.IsValidTarget(E.Range) || !E.IsReady()) return;

//            var pred = E.GetPrediction(target);
//
////            if (Player.IsFacing(target))
////                facing = Environment.TickCount;
////
////            if (!Player.IsFacing(target))
////                nofacing = Environment.TickCount;
//
//            if (Player.IsFacing(target))
//            {
//                E.Cast(pred.CastPosition.Extend(Player.Position, 300));
//                laste = Environment.TickCount;
//            }
//            else
//            {
//                E.Cast(pred.CastPosition.Extend(Player.Position, 530));
//                laste = Environment.TickCount;
//
//            }

//            var pred = E.GetPrediction(target);
//            var targetPos = pred.UnitPosition;
//            E.Cast(targetPos.Extend(Player.Position, 220).To2D());

//            var epred = E.GetPrediction(target);
//            var pos = epred.CastPosition;
//            if (pos.Distance(Player.Position) < E.Range
//                && epred.Hitchance >= HitChance.VeryHigh)
//            {
//                if (Utility.IsBothFacing(Player, target))
//                {
//                    E.Cast(pos.Extend(Player.Position, 300));
//                }
//                else if (Player.IsFacing(target) && !target.IsFacing(Player))
//                {
//                    E.Cast(pos.Extend(Player.Position, 150));
//                }
//                else if (!Player.IsFacing(target) && targetIsFacing(Player))
//                {
//                    E.Cast(pos.Extend(Player.Position, 300));
//                }
//            }

            var epred = E.GetSPrediction(target);
            var pos = epred.CastPosition;
            if ((epred.HitChance >= HitChance.VeryHigh || epred.HitChance == HitChance.Immobile) && target.Distance(Player) < 300)
            {
                E.Cast(pos.Extend(Player.Position.To2D(), 100));
            }

            if ((epred.HitChance >= HitChance.VeryHigh || epred.HitChance == HitChance.Immobile) && target.Distance(Player) > 300)
            {
                E.Cast(pos.Extend(Player.Position.To2D(), 375));
            }
            // E.Cast(targetPos);

        }

        public static void WComboHarassCast(string name, Obj_AI_Hero target)
        {
            if (!target.IsValidTarget(W.Range) || !W.IsReady()) return;
            if ((!Player.HasBuffOfType(BuffType.Stun) && !Player.HasBuffOfType(BuffType.Taunt) &&
                 !Player.HasBuffOfType(BuffType.Snare)) && Environment.TickCount - laste < 1500) return;
            var wpred = W.GetSPrediction(target);
            switch (GetStringValue(name))
                {
                    case 0:
                        W.Cast(wpred.CastPosition);
                        break;
                    case 1:
                        if (target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Taunt))
                            W.Cast(target.Position);
                        break;
                    case 2:
                        break;
                
            }
        }

        private static DamageToUnitDelegate _damageToUnit;
        private static SpellSlot _ignite;
        private static int facing;
        private static int nofacing;
        private static GameObject Veigare;
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);



        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }

        public static Color Color1
        {
            get { return _color; }
        }

        public static void Drawing_OnDrawChamp(EventArgs args)
        {

            if (!Config.Item("FillDamage").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(4000, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = DamageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable With Combo Rotation " + (unit.Health - damage);
                    Text.OnEndScene();
                }
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

                if (Config.Item("RushDrawWDamageFill").GetValue<bool>())
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);
                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawq = GetBool("displayQrange", typeof(bool));
            var draww = GetBool("displayWrange", typeof(bool));
            var drawe = GetBool("displayErange", typeof(bool));
            var drawr = GetBool("displayRrange", typeof(bool));
            var draw = GetBool("displayrange", typeof(bool));

            if (!draw) return;

            if (drawq)
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Magenta);

            if (draww)
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Aqua);

            if (drawe)
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Brown);

            if (drawr)
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red);
        }

        private static float GetComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Q.GetDamage(enemy);


            if (W.IsReady())
                damage += W.GetDamage(enemy);

            if (R.IsReady())
                damage += R.GetDamage(enemy);

            if (Player.GetSpellSlot("summonerdot").IsReady())
                damage += IgniteDamage(enemy);

            return (float)damage;
        }

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static void TearStack()
        {

            if (Config.Item("tearoptions").GetValue<bool>()
                && !Player.InFountain())
                return;

            if (Player.IsRecalling()) return;


            if (!Q.IsReady() ||
                (!TearoftheGoddess.IsOwned(Player) && !TearoftheGoddessCrystalScar.IsOwned(Player) &&
                 !ArchangelsStaff.IsOwned(Player) && !ArchangelsStaffCrystalScar.IsOwned(Player) &&
                 !Manamune.IsOwned(Player) && !ManamuneCrystalScar.IsOwned(Player)))
                return;

            if (!Game.CursorPos.IsZero)
                Q.Cast(Game.CursorPos);
            else
                Q.Cast();
        }

        public static int laste { get; set; }
    }
}
