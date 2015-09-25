using System;
using System.Collections.Generic;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

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
        public static Spell Q, W, E, R;
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
            Q = new Spell(SpellSlot.Q, 890);
            W = new Spell(SpellSlot.W, 880);
            E = new Spell(SpellSlot.E, 690);
            R = new Spell(SpellSlot.R, 650);

            DamageToUnit = GetComboDamage;

            Q.SetSkillshot(0.25f, 70f, 2000f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.25f, 225f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(1.2f, 25f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            R.SetTargetted(0f, 20f);

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;

        }

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
            }
            TearStack();
            if (GetBool("fleemode", typeof (KeyBind)))
                flee();
            SetIgniteSlot(Player.GetSpellSlot("summonerdot"));
            Orbwalker.SetAttack(!GetBool("aablock", typeof (bool)));
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            GetComboDamage(target);
            if (target.IsValidTarget(E.Range) && E.IsReady() && target.HasBuffOfType(BuffType.Stun))
            {
                var pred = E.GetPrediction(target).CastPosition;
                E.Cast(pred.Extend(Player.ServerPosition, 375));
            }
            if (Ignite.IsReady() && target.Health <= (Q.GetDamage(target) + Player.GetAutoAttackDamage(target)))
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }   
        }


        private static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;
            EComboHarasscast("efleemode",target);
        }

        private static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minion == null) return;

            if (!GetBool("useqlaneclearlast", typeof(bool))) return;
            foreach (var minions in minion)
            {
                var prediction = Prediction.GetPrediction(minions, Q.Delay);

                var collision = Q.GetCollision(Player.Position.To2D(),
                    new List<Vector2> {prediction.UnitPosition.To2D()});
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
                if (GetBool("useqjungle", typeof (bool)) && Q.IsReady())
                {
                    Q.Cast(minions);
                }

                if (GetBool("usewjungle", typeof (bool)))
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
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minion == null) return;
            if (GetBool("usewlaneclear", typeof (bool)))
            {
                var wcircle = W.GetCircularFarmLocation(minion);
                if (wcircle.MinionsHit >= GetValue("wminionjigolo") && W.IsReady())
                    W.Cast(wcircle.Position);
            }

            if (!GetBool("useqlaneclear", typeof (bool))) return;
            foreach (var minions in minion)
            {
                var prediction = Prediction.GetPrediction(minions, Q.Delay);

                var collision = Q.GetCollision(Player.Position.To2D(),
                    new List<Vector2> {prediction.UnitPosition.To2D()});
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
            EComboHarasscast("useecombo", target);
            QComboHarassCast("useqcombo", target);
            WComboHarassCast("usewmode", target);
            RCast("user", target);
        }

        public static void QComboHarassCast(string name, Obj_AI_Hero target)
        {
            if (!GetBool(name, typeof (bool))) return;
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
            if (!GetBool(name, typeof (bool))) return;
            if (!R.IsReady() || !target.IsValidTarget(R.Range)) return;
            if (R.GetDamage(target) > target.Health)
                foreach (var targets in HeroManager.Enemies.Where(x =>x.IsValid && x.IsChampion()))
                {
                    if (GetStringValue("user" + targets.ChampionName) != 0) return;
                    R.CastOnUnit(target);
                }
        }

        public static void QColCast(Obj_AI_Hero target, bool col = true)
        {
            var prediction = Prediction.GetPrediction(target, Q.Delay);

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
            if (!GetBool(name, typeof (bool))) return;
            if (!target.IsValidTarget(E.Range) || !E.IsReady()) return;

            var pred = E.GetPrediction(target);
            E.Cast(pred.CastPosition);
        }

        public static void WComboHarassCast(string name, Obj_AI_Hero target)
        {
            if (!target.IsValidTarget(W.Range) || !W.IsReady()) return;
            var wpred = W.GetPrediction(target, true);
            switch (GetStringValue(name))
            {
                case 0:
                    W.Cast(wpred.CastPosition);
                    break;
                case 1:
                    if (target.HasBuffOfType(BuffType.Stun))
                        W.Cast(target.Position);
                    break;
                case 2:
                    break;
            }
        }

        private static DamageToUnitDelegate _damageToUnit;
        private static SpellSlot _ignite;
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

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = DamageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage)/unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width*unit.Health/unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int) barPos.X + XOffset;
                    Text.Y = (int) barPos.Y + YOffset - 13;
                    Text.text = "Killable With Combo Rotation " + (unit.Health - damage);
                    Text.OnEndScene();
                }
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

                if (Config.Item("RushDrawWDamageFill").GetValue<bool>())
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107*percentHealthAfterDamage);
                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                    }
                }
            }
        }

        private static void OnDraw(EventArgs args)
        {
            var drawq = GetBool("displayQrange", typeof (bool));
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
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            if (Player.GetSpellSlot("summonerdot").IsReady())
                damage += IgniteDamage(enemy);

            return (float) damage;
        }

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
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
    }
}
