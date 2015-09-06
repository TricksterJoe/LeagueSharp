using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace OAhri
{
    /// <summary>
    /// Class
    /// </summary>
    internal class Ahri : MenuConfig
    {
        /// <summary>
        /// Setting needed stuff
        /// </summary>
        public const string ChampName = "Ahri";
        public static Spell Q, W, E, R;
        public static SpellSlot Ignite;
        public static SpellSlot FlashSlot;
        public static float FlashRange = 450f;
        public static GameObject QMissile { get; set; }

        /// <summary>
        /// Called Upon game load
        /// </summary>
        /// <param name="args"></param>
        internal static void Load(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;

            LoadMenu();

            Game.PrintChat("<font color = \"asasdasdasd\"OAhri By 'Hoes' is Loaded!");
            Q = new Spell(SpellSlot.Q, 780);
            W = new Spell(SpellSlot.W, 400);
            E = new Spell(SpellSlot.E, 810);
            R = new Spell(SpellSlot.R, 800);

            GlobalManager.DamageToUnit = GlobalManager.GetComboDamage;
            Q.SetSkillshot(0.25f, 50, 1600f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.25f, 60, 1550f, true, SkillshotType.SkillshotLine);
            Game.OnUpdate += GameOnUpdate;
            Drawing.OnDraw += DrawManager.Drawing_OnDraw;
            Drawing.OnDraw += DrawManager.Drawing_OnDrawChamp;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            //GameObject.OnDelete += ondelete;
            //Obj_AI_Base.OnIssueOrder += IssueOrder;
            Spellbook.OnCastSpell += Oncast;
        }

        #region On Dash

        /// <summary>
        /// Dash type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
                return;
            var delay = (int) (args.EndTick - Game.Time - E.Delay - 0.1f);
            if (target == null)
                return;
            if (delay > 0)
                E.Cast(args.EndPos);
        }

        #endregion

        #region Later~

        /*
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            if (sender == null)
                return;
            if (sender.Name == "Ahri_Base_Q_Speed.troy")
            {
                QOrb = sender;
            }
            if (QOrb == null)
                return;

            if (target.Distance(QOrb.Position.Normalized()) <= 100f)
            {
                Orbwalker.SetMovement(false);
            }
        }
         */

        #endregion

        #region On Object Create

        /// <summary>
        /// Object Create
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
                return;
            var missile = (MissileClient) sender;

            if (missile.IsValid && missile.IsAlly
                && missile.SData.Name != null
                && (missile.SData.Name == "AhriOrbReturn"))
            {
                QMissile = null;
            }
        }

        #endregion

        #region On Object Delete

        /// <summary>
        /// Object Delete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid<MissileClient>())
                return;

            var missile = (MissileClient) sender;

            if (missile.IsValid && missile.IsAlly
                && (missile.SData.Name == "AhriOrbMissile"
                    || missile.SData.Name == "AhriOrbReturn"))
            {
                QMissile = sender;
            }
        }

        #endregion

        #region On Spell Cast

        /// <summary>
        /// Upon Spell casting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void Oncast(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var target = TargetSelector.GetTarget(E.Range + R.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (args.Slot != SpellSlot.R)
                return;

            if (!Config.Item("comboMenu.blockr").GetValue<bool>())
                return;

            if (Config.Item("flee").GetValue<KeyBind>().Active)
                return;

            if (Game.CursorPos.CountEnemiesInRange(500) > 2 && (target.Health > GlobalManager.RComboCalc(target))
                && Player.HealthPercent < 10)
            {
                args.Process = false;
            }
        }

        #endregion

        #region On Game Update

        /// <summary>
        /// Called Every Tick 
        /// </summary>
        /// <param name="args"></param>
        private static void GameOnUpdate(EventArgs args)
        {
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
                    Jungleclear();
                    break;
            }
            Orbwalker.SetMovement(true);
            Orbwalker.SetAttack(true);

            if (Config.Item("flee").GetValue<KeyBind>().Active)
            {
                flee();
            }

            // Game.PrintChat(Q.Delay + Q.Width.ToString() + Q.Speed + Q.Collision + Q.IsSkillshot);
            var target = TargetSelector.GetTarget(Q.Range + 200, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (Player.Distance(target) > E.Range &&
                    ((!Q.IsReady() || !W.IsReady() || !R.IsReady())))
                {
                    Orbwalker.SetAttack(false);
                }
            }
            if (QMissile != null && target.Distance(QMissile.Position) <= 100)
            {
                Orbwalker.SetMovement(false);
            }

        }

        #endregion

        #region Flee

        /// <summary>
        /// Flee ~
        /// </summary>
        private static void flee()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var length = GlobalManager.GetWallLength(Player.ServerPosition, Game.CursorPos) < 500;
            var overWall = GlobalManager.IsOverWall(Player.ServerPosition, Game.CursorPos + R.Range);
            var fleer = Config.Item("flee.user").GetValue<bool>();

            if (Config.Item("flee.useq").GetValue<bool>())
            {
                Q.Cast(GlobalManager.Extend(Player.ServerPosition, Game.CursorPos, -400));
            }

            if (overWall && fleer)
            {
                if (length)
                {
                    R.Cast(GlobalManager.Extend(Player.ServerPosition, Game.CursorPos, R.Range));
                }
            }

            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            if (Config.Item("flee.usee").GetValue<bool>())
            {
                E.Cast(target);
            }

        }

        #endregion

        #region Jungle Clear

        /// <summary>
        /// Jungle settings
        /// </summary>
        private static void Jungleclear()
        {
            var jungle = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth).FirstOrDefault();
            if (jungle == null)
                return;

            var useq = Config.Item("jungleMenu.useq").GetValue<bool>();
            var usew = Config.Item("jungleMenu.usew").GetValue<bool>();
            var manaslider = Config.Item("jungleMenu.manaslider").GetValue<Slider>().Value;

            if (Player.ManaPercent < manaslider)
                return;

            if (useq && Q.IsReady())
            {
                Q.Cast(jungle);
            }

            if (usew && W.IsReady() && jungle.Distance(Player) < W.Range)
            {
                W.Cast();
            }
        }

        #endregion

        #region Lane clear

        /// <summary>
        /// Lane Clear
        /// </summary>
        private static void LaneClear()
        {
            var minion =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            var minionw =
                MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth);

            if (minion == null)
                return;
            var useq = Config.Item("laneMenu.useq").GetValue<bool>();
            var usew = Config.Item("laneMenu.usew").GetValue<bool>();
            var qslider = Config.Item("laneMenu.minminions").GetValue<Slider>().Value;
            var manaslider = Config.Item("laneMenu.manaslider").GetValue<Slider>().Value;

            if (Player.ManaPercent < manaslider)
                return;

            var qline = Q.GetLineFarmLocation(
                MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(Q.Range),
                    Q.Delay, Q.Width, Q.Speed,
                    Player.Position, Q.Range,
                    false, SkillshotType.SkillshotLine), Q.Width);

            if (Q.IsReady() && useq)
            {
                if (qline.MinionsHit >= qslider)
                {
                    Q.Cast(qline.Position);
                }
            }

            if (W.IsReady() && usew && minionw.Count() >= 2)
            {
                W.Cast();
            }
        }

        #endregion

        #region Harass

        /// <summary>
        /// Harass Settings
        /// </summary>
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var useq = Config.Item("harrasMenu.useq").GetValue<bool>();
            var usew = Config.Item("harrasMenu.usew").GetValue<bool>();
            var usee = Config.Item("harrasMenu.usee").GetValue<bool>();
            var qpred = Q.GetPrediction(target);
	        var epred = E.GetPrediction(target, false, E.Range,
		        new[] {CollisionableObjects.Minions, CollisionableObjects.YasuoWall});
            if (Q.IsReady() && useq && target.IsValidTarget(Q.Range))
            {
                Q.Cast(qpred.CastPosition);
            }

            if (E.IsReady() && usee && target.IsValidTarget(E.Range))
            {
                E.Cast(epred.CastPosition);
            }

            if (W.IsReady() && usew && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }
        }

        #endregion

        #region Combo
        /// <summary>
        /// Combo
        /// </summary>
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var useq = Config.Item("comboMenu.useq").GetValue<bool>();
            var usew = Config.Item("comboMenu.usew").GetValue<bool>();
            var usee = Config.Item("comboMenu.usee").GetValue<bool>();
            var combomode = Config.Item("comboMenu.combomode").GetValue<StringList>().SelectedIndex;
            var qpred = Q.GetPrediction(target);
           // var epred = E.GetPrediction(target);

            if (Ignite.IsReady() && target.Health <= (Q.GetDamage(target) + Player.GetAutoAttackDamage(target)))
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }

            AhriR.RUlt();
            switch (combomode)
            {
                case 0:
                {
                    if (!E.IsReady() && Q.IsReady() && target.IsValidTarget(Q.Range) && useq)
                    {
                        Q.Cast(qpred.CastPosition);
                    }

                    if (E.IsReady() && usee && target.IsValidTarget(E.Range))
                    {
                        E.Cast(E.GetPrediction(target, false, E.Range, new []{CollisionableObjects.Minions, CollisionableObjects.YasuoWall}).CastPosition);
                    }

                    if (W.IsReady() && target.IsValidTarget(600) && usew
                        && (Player.Mana >= W.Instance.ManaCost + E.Instance.ManaCost))
                    {
                        W.Cast();
                    }
                    break;
                }
                case 1:
                {
                    if (E.IsReady() && !Q.IsReady() && target.IsValidTarget() && usee)
                    {
	                    if (qpred.CollisionObjects.Count == 0 || qpred.CollisionObjects == null)
	                    {
		                    E.Cast(qpred.CastPosition);
	                    }
                    }

                    if (Q.IsReady() && useq)
                    {
                        Q.Cast(target);
                    }

                    if (W.IsReady() && target.IsValidTarget(600) && usew
                        && (Player.Mana >= W.Instance.ManaCost + E.Instance.ManaCost))
                    {
                        W.Cast();
                    }
                    break;
                }
            }
        }

        #endregion
    }
}
