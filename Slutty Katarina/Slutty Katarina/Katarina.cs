using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace Slutty_Katarina
{
    class Katarina : Helper
    {
        private static GameObject _blade;
        private static bool count;
        private static int starttimer;
        private static int timersart;
        private static GameObject _enemy;
        public static SpellSlot Ignite;
        public static Spell Q, W, E, R;
        public static void OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Katarina") return;
            MenuConfig.OnLoad();
            Ignite = Player.GetSpellSlot("SummonerDot");
            Q = new Spell(SpellSlot.Q, 670);
            W = new Spell(SpellSlot.W, 370);
            E = new Spell(SpellSlot.E, 690);
            R = new Spell(SpellSlot.R, 540);
            
            Game.OnUpdate += OnUpdate;

            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            Obj_AI_Base.OnDoCast += MinionClear;
            Drawing.OnDraw += OnDraw;

            Obj_AI_Base.OnDoCast += OnDoCast;
            GameObject.OnDelete += OnDelete1;
            Game.OnWndProc += OnWndProc;
            Obj_AI_Base.OnIssueOrder += OnOrder;
            Spellbook.OnCastSpell += OnCastSpell;
        }


        private static void OnDraw(EventArgs args)
        {
            var drawq = GetBool("drawq", typeof (bool));
            var draww = GetBool("draww", typeof(bool));
            var drawe = GetBool("drawe", typeof(bool));
            var drawr = GetBool("drawr", typeof(bool));

            if (Q.Level > 0 && drawq)
            {
                var color = Q.IsReady() ? Color.CadetBlue : Color.Red;
                Render.Circle.DrawCircle(Player.Position, Q.Range, color, 3);
            }

            if (W.Level > 0 && draww)
            {
                var color = W.IsReady() ? Color.Green : Color.Red;
                Render.Circle.DrawCircle(Player.Position, W.Range, color, 3);
            }

            if (E.Level > 0 && drawe)
            {
                var color = E.IsReady() ? Color.DarkOrchid : Color.Red;
                Render.Circle.DrawCircle(Player.Position, E.Range, color, 3);
            }

            if (R.Level > 0 && drawr)
            {
                var color = R.IsReady() ? Color.Teal : Color.Red;
                Render.Circle.DrawCircle(Player.Position, R.Range, color, 3);
            }
        }

        private static void MinionClear(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (!sender.IsMe) return;
            if (!args.SData.IsAutoAttack()) return;
            if (args.Target.Type != GameObjectType.obj_AI_Minion) return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                if (W.IsReady() && GetBool("wlasthitlane", typeof(bool)))
                {
                    if (((Obj_AI_Base) args.Target).Health > Player.GetAutoAttackDamage((Obj_AI_Base) args.Target) + 30
                        &&
                        ((Obj_AI_Base) args.Target).Health <
                        W.GetDamage(((Obj_AI_Base) args.Target)) + Player.GetAutoAttackDamage((Obj_AI_Base) args.Target)) 
                    {
                        W.Cast();
                    }
                }
            }
        }

        public static void SetEvade()
        {
            var menu = Menu.GetMenu("ezEvade", "ezEvade");
            if (menu == null) return;
            if (IsChanneling && !menu.Item("DodgeSkillShots").GetValue<KeyBind>().Active) return;
           
            menu.Item("DodgeSkillShots").SetValue(!IsChanneling);
            Game.PrintChat("Disabling Evade.");
        }
        #region spell cancel
        /// <summary>
        /// Allow user to cancel channeling
        /// </summary>
        public static bool CanBeCanceledByUser { get; set; }

        /// <summary>
        /// check if the spell is being channeled
        /// </summary>
        public static bool IsChanneling = false;

        /// <summary>
        /// Is spell type channel
        /// </summary>
        public static bool IsChannelTypeSpell { get; set; }

        /// <summary>
        /// Is spell targettable
        /// </summary>
        public static bool TargetSpellCancel { get; set; }

        /// <summary>
        /// Should the spell  be interuptable by casting other spells
        /// </summary>
        public static bool LetSpellcancel { get; set; }

        /// <summary>
        /// Last time casting has been issued
        /// </summary>
        private static int _cancelSpellIssue;

        private static int lastq;
        private static int laste;
        private static int _lastwcasted;
        private static int lastnoenemies;


        private static void OnDelete1(GameObject sender, EventArgs args)
        {
            if (sender.Name == "katarina_deathlotus_success.troy")
            {
                IsChanneling = false;
            }

            if (sender.Name == "katarina_deathLotus_tar.troy")
            {
                IsChanneling = false;
                
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (LetSpellcancel) return;

            args.Process = !IsChanneling;
        }

        private static void OnOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {

            if (!IsChanneling) return;

            if (args.Order == GameObjectOrder.MoveTo || args.Order == GameObjectOrder.AttackTo ||
                args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AutoAttack)
            {
                args.Process = false;
            }
        }

        private static void OnWndProc(WndEventArgs args)
        {
            if (!CanBeCanceledByUser) return;

            if (args.Msg == 517)
            {
                IsChanneling = false;
            }
        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (args.SData.Name == "KatarinaR")
            {
                IsChanneling = true;
            }
        }

        #endregion

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "KatarinaQ")
            {
                lastq = Environment.TickCount;
            }

            if (args.SData.Name == "KatarinaE")
            {
                laste = Environment.TickCount;
            }

            if (args.SData.Name == "KatarinaE" && !count)
            {
                count = true;
                Utility.DelayAction.Add(350, () =>  _enemy = null);
                starttimer = Environment.TickCount;
                
            }
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "katarina_Base_bouncingBlades_mis.troy" && _blade != null)
            {
                _blade = null;
            }
        }

        private static float GetTravelTime(GameObject obj, GameObject enemy)
        {
            if (obj == null || enemy == null) return 0;
            // s=vt, t=s/v, v=s/t
           // var time = Environment.TickCount - timersart;
            // var distance = _enemy.Position.Distance(Player.Position);
            // var velocity = distance * 1000 / time; = 1700

            var distance = enemy.Position.Distance(obj.Position);
            var gettime = distance*1000/600;
            return gettime;
        }
        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("ward") && E.IsReady() && sender.IsAlly
                && GetBool("wardjump", typeof(KeyBind)))
            {
                _lastwcasted = Environment.TickCount;
                var ward = (Obj_AI_Base)sender;

                if (ward.IsMe) return;
                E.Cast(ward);
            }

            if (!sender.IsValid<MissileClient>())
                return;
            var missile = (MissileClient) sender;

            if (missile.SData.Name == "KatarinaQ" && _blade == null)
            {
                if (missile.Target != null)
                {
                    _enemy = missile.Target;
                }

                _blade = missile;
                timersart = Environment.TickCount;
            }

        }

        public static bool CanKill(Spell spells, Obj_AI_Base obj)
        {
            return spells.IsReady() && spells.GetDamage(obj) > obj.Health && Player.Distance(obj) < spells.Range;
        }

        public static float SpellUpSoon(SpellSlot slot)
        {
            var expires = (Player.Spellbook.GetSpell(slot).CooldownExpires);
            var cd =
                (float)
                    (expires -
                     (Game.Time - 1));

            return cd;
        }


        private static void OnUpdate(EventArgs args)
        {
        //  SetEvade();
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
            if (GetBool("wardjump", typeof (KeyBind)))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                WardJump.WardJumped(Player.Position.Extend(Game.CursorPos, 590));
            }

          //  Game.PrintChat((Environment.TickCount - lastnoenemies).ToString());
            
            if (Player.CountEnemiesInRange(R.Range + 30) == 0 && IsChanneling)
            {
                IsChanneling = false;
                LetSpellcancel = true;
                var movpos = new Vector2(Player.Position.X + 10, Player.Position.Y + 15);
                Player.IssueOrder(GameObjectOrder.MoveTo, movpos.To3D());

            }

            foreach (var enemy in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player) <= Q.Range + 600))
            {
                if (Player.Distance(enemy) < Q.Range + 550 && Player.Distance(enemy) > Q.Range + 20)
                {
                    if (CanKill(Q, enemy))
                    {
                        var position = Player.ServerPosition.Extend(enemy.ServerPosition, E.Range);
                        WardJump.WardJumped(position);
                        IsChanneling = false;
                        LetSpellcancel = true;
                        if (enemy.IsValidTarget(Q.Range))
                        {
                            Q.Cast(enemy);
                        }
                    }

                }

            }

            if (Player.CountEnemiesInRange(R.Range + 30) == 0 && IsChanneling && (E.IsReady() || SpellUpSoon(SpellSlot.E) < 0.2))
            {
                lastnoenemies = Environment.TickCount;
                IsChanneling = false;
                LetSpellcancel = true;

            }

            foreach (var enemies in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player) <= Q.Range))
            {
                var ks = GetBool("ks", typeof(bool));
                var qks = GetBool("useqks", typeof(bool));
                var wks = GetBool("usewks", typeof(bool));
                var eks = GetBool("useeks", typeof(bool));

                if (!ks) return;


                if ((Q.IsReady() && enemies.Health < Q.GetDamage(enemies) && qks && enemies.Distance(Player) < Q.Range) ||
                    (W.IsReady() && enemies.Health < W.GetDamage(enemies) && enemies.Distance(Player) < W.Range && wks) ||
                    (E.IsReady() && enemies.Health < E.GetDamage(enemies) && enemies.Distance(Player) < E.Range && eks))
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                }

                if (CanKill(Q, enemies))
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    Q.Cast(enemies);
                }

                if (CanKill(E, enemies) && enemies.Health < E.GetDamage(enemies) && eks)
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    E.Cast(enemies);
                }

                if (CanKill(W, enemies) && wks)
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    W.Cast();
                }




                    if (Q.IsReady() && E.IsReady() && enemies.Distance(Player) < E.Range &&
                    enemies.Health < E.GetDamage(enemies) + Q.GetDamage(enemies) && qks && eks)
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    E.Cast(enemies);
                    Q.Cast(enemies);
                }

                if (Q.IsReady() && E.IsReady() && enemies.Distance(Player) < E.Range &&
                    enemies.Health < E.GetDamage(enemies) + Q.GetDamage(enemies) + W.GetDamage(enemies) &&
                    SpellUpSoon(SpellSlot.Q) < 1 && qks && eks) 
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    E.Cast(enemies);
                    Q.Cast(enemies);
                }


                if (Q.IsReady() && W.IsReady() && enemies.Distance(Player) < W.Range &&
                    enemies.Health < W.GetDamage(enemies) + Q.GetDamage(enemies) && eks && qks)
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    Q.Cast(enemies);
                    W.Cast();
                }

                if (E.IsReady() && (W.IsReady() || SpellUpSoon(SpellSlot.Q) < 1 )&& enemies.Distance(Player) < W.Range &&
                    enemies.Health < W.GetDamage(enemies) + E.GetDamage(enemies) && qks)
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                    E.Cast(enemies);
                    W.Cast();
                }


                if (!Player.IsChannelingImportantSpell())
                {
                    IsChanneling = false;
                    LetSpellcancel = true;
                }
                else
                {
                    if (CanKill(W, enemies) && wks) return;

                    if (CanKill(E, enemies) &&
                        Player.Health < E
                        .GetDamage(enemies) + Player.GetAutoAttackDamage(enemies) && eks)
                        return;
                    if (CanKill(Q, enemies)) return;

                    if ((Q.IsReady() && enemies.Health < Q.GetDamage(enemies) && qks) ||
                        (W.IsReady() && enemies.Health < W.GetDamage(enemies) && enemies.Distance(Player) < W.Range &&
                         wks) ||
                        (E.IsReady() && enemies.Health < E.GetDamage(enemies) && enemies.Distance(Player) < E.Range &&
                         eks))
                        return;

                    if (E.IsReady() && (W.IsReady() || SpellUpSoon(SpellSlot.Q) < 1.4) &&
                        enemies.Distance(Player) < W.Range &&
                        enemies.Health < W.GetDamage(enemies) + E.GetDamage(enemies) && qks)
                        return;

                    if (Player.CountEnemiesInRange(R.Range + 70) == 0) return;

                    if (Q.IsReady() && E.IsReady() && enemies.Distance(Player) < E.Range &&
                        enemies.Health < E.GetDamage(enemies) + Q.GetDamage(enemies) && qks && eks)
                        return;

                    if (Q.IsReady() && E.IsReady() && enemies.Distance(Player) < E.Range &&
                        enemies.Health < E.GetDamage(enemies) + Q.GetDamage(enemies) + W.GetDamage(enemies) &&
                        SpellUpSoon(SpellSlot.Q) < 1.4 && qks && eks)
                        return;

                    if (Q.IsReady() && W.IsReady() && enemies.Distance(Player) < W.Range &&
                        enemies.Health < W.GetDamage(enemies) + Q.GetDamage(enemies) && eks && qks)
                        return;

                    IsChanneling = true;
                    LetSpellcancel = false;
                }
            }


        }

        private static void LastHit()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
    MinionOrderTypes.MaxHealth);

            var lasthitq = GetBool("qlasthit", typeof(bool));
            var lasthitw = GetBool("wlasthit", typeof(bool));

            foreach (var minion in minions)
            {
                if (minion.Health <= Q.GetDamage(minion) && lasthitq && Q.IsReady())
                {
                    Q.Cast(minion);
                }
                if (minion.Health <= W.GetDamage(minion) && lasthitw && W.IsReady() && minion.Distance(Player) < W.Range)
                {
                    W.Cast();
                }
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            var lasthitq = GetBool("qlasthitlane", typeof (bool));
            var lasthitw = GetBool("wlasthitlane", typeof(bool));
            var alwaysq = GetBool("qlaneclear", typeof(bool));
            var alwaysw = GetBool("wlaneclear", typeof(bool));
            var qcount = GetValue("qlaneclearmin");
            var wcount = GetValue("wlaneclearmin");
            foreach (var minion in minions)
            {
                if (minion.Health <= Q.GetDamage(minion) && lasthitq && Q.IsReady())
                {
                    Q.Cast(minion);
                }
                if (minion.Health <= W.GetDamage(minion) && lasthitw && W.IsReady() && minion.Distance(Player) <= W.Range)
                {
                    W.Cast();
                }
            }
            if (minions.FirstOrDefault() == null) return;
            if (minions.Count >= qcount && Q.IsReady() && alwaysq)
            {
                Q.Cast(minions.FirstOrDefault());
            }
            var minions1 = MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minions1.FirstOrDefault() == null) return;

            if (minions1.Count >= wcount && W.IsReady() && alwaysw)
            {
                W.Cast();
            }

        }

        private static void Mixed()
        {
            var target = TargetSelector.GetTarget(Player, Q.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget()) return;

            if (Q.IsReady() && GetBool("useqh", typeof (bool)))
            {
                Q.Cast(target);
            }

            if (W.IsReady() && GetBool("usewh", typeof(bool)) && target.IsValidTarget(W.Range))
            {
                W.Cast();
            }


        }
        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Player, Q.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget()) return;

            if (GetBool("useignite", typeof (bool)))
            {
                if (Q.IsReady() && Ignite.IsReady() && (target.Health <= Q.GetDamage(target) + IgniteDamage(target)))
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }

                if (Ignite.IsReady() && (target.Health <= IgniteDamage(target) - 30))
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }
            }
            switch (GetStringValue("combomode"))
            {
                case 0:
                    if (E.IsReady() && Environment.TickCount - lastnoenemies < 1500 && target.IsValidTarget(E.Range) && target.Distance(Player) > R.Range - 230)
                    {
                        E.Cast(target);
                    }

                    if (Q.IsReady() && Environment.TickCount - lastnoenemies > 1000)
                    {
                        Q.Cast(target);
                    }

                    if (_blade != null && _enemy != null && GetBool("usee", typeof(bool)))
                    {
                        if (GetTravelTime(_blade, _enemy) < 500)
                        {
                            Player.Spellbook.CastSpell(SpellSlot.E, _enemy);
                        }
                    }


                    if (Environment.TickCount - lastq > 1500 && E.IsReady() && !Q.IsReady() 
                        && target.Distance(Player) > Orbwalking.GetRealAutoAttackRange(target) + 30 && GetBool("usee", typeof(bool)))
                    {
                        E.Cast(target);
                    }
                    if (W.IsReady() && target.IsValidTarget(W.Range) && Environment.TickCount - laste > 350)
                    {
                        W.Cast();
                    }
                    if (CanKill(Q, target)) return;
                    if (CanKill(W, target)) return;
                    if (CanKill(E, target)) return;
                    if ((R.GetDamage(target, 1) * 10) / 10 > target.Health && target.Distance(Player) < R.Range - 150)
                    {
                        R.Cast();
                    }

                    //if (Player.CountEnemiesInRange(R.Range - 130) > 0 && !E.IsReady() && !Q.IsReady() && !W.IsReady())
                    //{
                    //    R.Cast();
                    //}

                    if (R.IsReady() && !E.IsReady() && !Q.IsReady() && !W.IsReady() &&
                        target.IsValidTarget(R.Range - 150) && target.Health < (R.GetDamage(target, 1) * 10) / 8)
                    {
                        R.Cast();
                    }
                    break;
            }
        }
    }
}
