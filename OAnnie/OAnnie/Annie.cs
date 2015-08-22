using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace OAnnie
{
    internal class Annie : MenuConfig
    {
        public const string ChampName = "Annie";
        public static Spell Q, W, E, R;
        public static SpellSlot Ignite;
        public static SpellSlot FlashSlot;
        public static float FlashRange = 450f;
        public static Obj_AI_Hero Player = ObjectManager.Player;

        /// <summary>
        ///     Passive Buff
        /// </summary>
        #region Passive buff
        public static int GetPassiveBuff
        {
            get
            {
                var data = Player.Buffs.FirstOrDefault(b => b.DisplayName == "Pyromania");
                // Does not use C# v6+ T_T
                // return data?.Count ?? 0;
                return data != null ? data.Count : 0;
            }
        }

        #endregion

        /// <summary>
        ///     When Game Loads
        /// </summary>
        /// <param name="args"></param>
        #region called when loaded
        internal static void Load(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;

            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 600);
            GlobalManager.DamageToUnit = GlobalManager.GetComboDamage;
            W.SetSkillshot(0.5f, 250f, float.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.2f, 250f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            CreateMenu();
            FlashSlot = Player.GetSpellSlot("SummonerFlash");
            Ignite = Player.GetSpellSlot("SummonerDot");
            Game.OnUpdate += OnGameUpdate;
            Drawing.OnDraw += DrawManager.OnDraw;
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            // CustomEvents.Unit.OnDash += Unit_OnDash;
            //  AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
        }


        #endregion

        
        /// <summary>
        /// Gapcloser
        /// </summary>
        /// <param name="gapcloser"></param>
        #region GapCloser
        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (!gapcloser.Sender.IsEnemy)
                return;
            var gap = Config.Item("miscMenu.qwgap").GetValue<bool>();
            if (!gap)
                return;
            if (Player.HasBuff("pyromania_particle"))
            {
                if (Q.IsReady()
                    && Q.IsInRange(gapcloser.Start))
                {
                    Q.Cast(gapcloser.Start);
                }

                if (W.IsReady() && W.IsInRange(gapcloser.Start))
                {
                    W.Cast(gapcloser.Start);
                }
            }
        }

        #endregion



        /// <summary>
        /// Dash
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #region ondash
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var dash = Config.Item("miscMenu.qwdash").GetValue<bool>();
            if (!dash)
                return;
            if (sender == null)
                return;
            if (!sender.IsEnemy)
                return;

            if (sender.NetworkId != target.NetworkId) return;


            if (Player.HasBuff("pyromania_particle"))
            {
                if (Q.IsReady()
                    && Q.IsInRange(sender.ServerPosition))
                {
                    Q.Cast(sender);
                }

                if (W.IsReady() && W.IsInRange(sender.ServerPosition))
                {
                    W.Cast(sender);
                }
            }
        }
         
         

        #endregion
         

        /// <summary>
        ///     E On auto attack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        #region Process spell
        private static void Game_ProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Config.Item("comboMenu.emenu.eaa").GetValue<bool>()) return;
            if (sender.IsEnemy
                && sender.IsChampion()
                && args.SData.IsAutoAttack()
                && args.Target.IsMe)
            {
                E.Cast();
            }
        }

        #endregion

        /// <summary>
        ///     Every tick update
        /// </summary>
        /// <param name="args"></param>
        #region On Update
        private static void OnGameUpdate(EventArgs args)
        {
            /*
            foreach (var buff in Player.Buffs)
            {
                if (buff.Name.ToLower() != "odinplayerbuff"
                    || buff.Name.ToLower() != "kalistacoopstrikeally"
                    || buff.Name != "pyromania_marker")
                    Game.PrintChat(buff.Name.ToLower());
            }
             */

            Orbwalker.SetAttack(true);
            if (Player.IsRecalling())
                return;

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    JungleClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
            }
            var target = TargetSelector.GetTarget(Q.Range + 200, TargetSelector.DamageType.Magical);
            if (target != null)
            {
                if (Player.Distance(target) > Q.Range &&
                    (( !Q.IsReady() || !W.IsReady() || !R.IsReady())) )
                {
                    Orbwalker.SetAttack(false);
                }
                else
                {
                    Orbwalker.SetAttack(true);
                }
            }
            Parostack();
            KillSteal();

            if (!Config.Item("tibmove").GetValue<KeyBind>().Active)
            {
                Tibbers.Tibbersmove();
            }
            else if (Config.Item("tibmove").GetValue<KeyBind>().Active)
            {
                MoveTibbers();
            }

            if (Config.Item("comboMenu.flashmenu.flashr").GetValue<KeyBind>().Active
                || Config.Item("comboMenu.flashmenu.flasher").GetValue<KeyBind>().Active)
            {
                TibbersFlash();
            }        
             
        }

        #endregion

        /// <summary>
        ///     Kill Steal
        /// </summary>
        #region Kill Steal
        private static void KillSteal()
        {
            var ks = Config.Item("killstealMenu.ks").GetValue<bool>();
            var useq = Config.Item("killstealMenu.q").GetValue<bool>();
            var usew = Config.Item("killstealMenu.w").GetValue<bool>();
            var user = Config.Item("killstealMenu.r").GetValue<bool>();

            if (!ks)
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            if (useq && target.IsValidTarget(Q.Range) && Q.IsReady() && Q.GetDamage(target) >= target.Health)
            {
                Q.Cast(target);
            }

            if (usew && target.IsValidTarget(W.Range) && W.IsReady() && W.GetDamage(target) >= target.Health)
            {
                W.Cast(target);
            }

            if (user && target.IsValidTarget(R.Range) && R.IsReady() && R.GetDamage(target) >= target.Health)
            {
                Q.Cast(target);
            }
        }

        #endregion

        /// <summary>
        ///     passive stack
        /// </summary>
        #region Pyro Stacking
        private static void Parostack()
        {
            var usee = Config.Item("comboMenu.passivemanagement.e.stack").GetValue<bool>();
            var usew = Config.Item("comboMenu.passivemanagement.w.stack").GetValue<bool>();

            if (Player.HasBuff("pyromania_particle"))
                return;

            if (usee && E.IsReady())
            {
                E.Cast();
            }

            if (usew && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }
        }

        #endregion

        /// <summary>
        ///     jungleclear
        /// </summary>
        #region jungle clear
        private static void JungleClear()
        {
            var useq = Config.Item("clearMenu.jungleMenu.useq").GetValue<bool>();
            var usew = Config.Item("clearMenu.jungleMenu.usew").GetValue<bool>();
            var usee = Config.Item("clearMenu.jungleMenu.usee").GetValue<bool>();

            var minionCount =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All,
                    MinionTeam.Neutral).FirstOrDefault();

            if (minionCount == null)
                return;
            var minion = minionCount;

            if (useq && Q.IsReady() && minion.IsValidTarget(Q.Range))
            {
                Q.Cast(minion);
            }

            if (usew && W.IsReady() && minion.IsValidTarget(W.Range))
            {
                W.Cast(minion);
            }

            if (usee && E.IsReady() && minion.IsValidTarget(W.Range))
            {
                E.Cast();
            }
        }

        #endregion

        /// <summary>
        ///     Lane clear
        /// </summary>
        #region Lane Clear
        private static void Laneclear()
        {
            var useq = Config.Item("clearMenu.laneMenu.useq").GetValue<bool>();
            var usestun = Config.Item("clearMenu.laneMenu.keepstun").GetValue<bool>();
            var useql = Config.Item("clearMenu.laneMenu.useqlast").GetValue<bool>();
            var usew = Config.Item("clearMenu.laneMenu.usew").GetValue<bool>();
            var usewslider = Config.Item("clearMenu.laneMenu.usewslider").GetValue<Slider>().Value;
            var minMana = Config.Item("clearMenu.laneMenu.manaslider").GetValue<Slider>().Value;
            if (usestun && Player.HasBuff("pyromania_particle"))
                return;
            var minionCount =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly).FirstOrDefault();

            if (minionCount == null)
                return;
            var minion = minionCount;
            var minionhp = minionCount.Health;


            if (useql && Q.IsReady() && minion.IsValidTarget(Q.Range) && minionhp <= Q.GetDamage(minion) && minionhp > Player.GetAutoAttackDamage(minion))
            {
                Q.Cast(minion);
            }

            if (useq && Q.IsReady() && minion.IsValidTarget(Q.Range))
            {
                Q.Cast(minion);
            }

            var wminion =
                MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All,
                    MinionTeam.NotAlly);
            if (wminion == null)
                return;

            var pred = W.GetLineFarmLocation(wminion);

            foreach (var mincount in wminion)
            {
                if (usew && W.IsReady() && minion.IsValidTarget(W.Range) && pred.MinionsHit >= usewslider &&
                    Player.ManaPercent >= minMana)
                {
                    W.Cast(mincount);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Last hit
        /// </summary>
        #region Last Hit
        private static void LastHit()
        {
            var useql = Config.Item("clearMenu.lastMenu.useqlast").GetValue<bool>();
            var usestun = Config.Item("clearMenu.lastMenu.keepstun").GetValue<bool>();
            if (usestun && Player.HasBuff("pyromania_particle"))
                return;
            var minionCount =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All,
                    MinionTeam.NotAlly).FirstOrDefault();

            if (minionCount == null)
                return;

            var minion = minionCount;
            var minionhp = minion.Health;

            if (minionhp <= Q.GetDamage(minion) && useql && Q.IsReady())
            {
                Q.Cast(minion);
            }
        }

        #endregion

        /// <summary>
        ///     Tibber Flash Modes
        /// </summary>
        #region Flash
        private static void TibbersFlash()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(R.Range + FlashRange, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (!R.IsReady())
            {
                Combo();
            }

            var x = target.Position.Extend(Prediction.GetPrediction(target, 1).UnitPosition, FlashRange);
            var predpos = R.GetPrediction(target);
            if (Config.Item("comboMenu.flashmenu.flashr").GetValue<KeyBind>().Active)
            {
                if (Player.HasBuff("pyromania_particle"))
                {
                        Player.Spellbook.CastSpell(FlashSlot, x);
                        R.Cast(predpos.CastPosition);
                }
            }

            if (Config.Item("comboMenu.flashmenu.flasher").GetValue<KeyBind>().Active)
            {
                if (GetPassiveBuff == 3)
                {
                        Player.Spellbook.CastSpell(FlashSlot, x);
                        E.Cast();
                }
                if (Player.HasBuff("pyromania_particle"))
                {
                    R.Cast(predpos.CastPosition);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Mixed Mode
        /// </summary>
        #region Mixed Mode
        private static void Mixed()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var useq = Config.Item("harrasMenu.useq").GetValue<bool>();
            var usew = Config.Item("harrasMenu.usew").GetValue<bool>();

            if (useq && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (usew && W.IsReady() && target.IsValidTarget(W.Range))
            {
                Q.Cast(target);
            }
        }

        #endregion

        /// <summary>
        ///     Combo
        /// </summary>

        #region Combo
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var useq = Config.Item("comboMenu.useq").GetValue<bool>();
            var usew = Config.Item("comboMenu.usew").GetValue<bool>();
            var usee = Config.Item("comboMenu.usee").GetValue<bool>();
            var user = Config.Item("comboMenu.user").GetValue<bool>();
            var usersmart = Config.Item("comboMenu.user.smart").GetValue<bool>();
            var useebefore = Config.Item("comboMenu.passivemanagement.e.before").GetValue<bool>();
            var userslider = Config.Item("comboMenu.user.Slider").GetValue<Slider>().Value;

            if (Ignite.IsReady() && (target.Health <= Q.GetDamage(target) +Player.GetAutoAttackDamage(target)))
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && useq)
            {
                if (useebefore)
                {
                    if (GetPassiveBuff == 3 && E.IsReady() && !Player.HasBuff("summonerteleport"))
                    {
                        E.Cast();
                    }

                    if (!R.IsReady())
                    {
                        Q.Cast(target);
                    }
                    else
                    {
                        if (Player.HasBuff("pyromania_particles") && usersmart)
                            return;
                        Q.Cast(target);
                    }
                }

                if (!R.IsReady())
                {
                    Q.Cast(target);
                }
                else
                {
                    if (Player.HasBuff("pyromania_particles") && usersmart)
                        return;
                    Q.Cast(target);
                }
            }

            if (usee && E.IsReady() && !Player.HasBuff("pyromania_particles") && !Player.HasBuff("summonerteleport"))
            {
                switch (Config.Item("comboMenu.emenu.emode").GetValue<StringList>().SelectedIndex)
                {
                    case 0:
                        if (GetPassiveBuff == 3)
                            E.Cast();
                        break;

                    case 1:
                        E.Cast();
                        break;
                }
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && usew && !Player.HasBuff("summonerteleport"))
            {
                if (Player.HasBuff("pyromania_particles") && R.IsReady() && usersmart)
                    return;
                    W.Cast(target);
            }

            if (R.IsReady()
                && user && target.IsValidTarget(R.Range) && !Player.HasBuff("summonerteleport") && Player.HasBuff("pyromania_particle"))
            {
                foreach (var rhit in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(enemy => enemy.IsValidTarget())
                        .Select(x => R.GetPrediction(x, true))
                        .Where(pred => pred.AoeTargetsHitCount >= userslider))
                {
                    R.Cast(rhit.CastPosition);

                }
               if (target.Health >= Q.GetDamage(target) +W.GetDamage(target))
                {
                    R.Cast(target.Position);
                }
            }
        }

        #endregion

        /// <summary>
        ///     Tibbers Configure
        /// </summary>
        #region Tibbers
        private static void MoveTibbers()
        {
            Player.IssueOrder(GameObjectOrder.MovePet, Game.CursorPos);
        }

        #endregion
    }
}