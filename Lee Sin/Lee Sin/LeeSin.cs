using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Color = System.Drawing.Color;
namespace Lee_Sin
{
    internal class LeeSin : Helper
    {
        #region vars, enums, misc

        public static Spell Q, W, E, R, Rk;
        private static int _lastward;
        private static bool _casted;
        private static bool _qcasted;
        public static bool Castedonward;
        private static bool _qcasteds;
        private static int _lastq;
        private static bool _jumped;
        private static bool _wardcasteds;
        private static Vector3 _position;
        private static Vector3 _playerpos;
        private static bool _qcast;
        private static Vector3 _wardpos;
        public static steps? Steps;
        private static bool _wcasteds;
        private static int _laste;
        private static int _lastqc;
        private static int _lastelane;
        private static bool _processw;
        private static int _lastqj;
        private static int _lastwj;
        private static int _lastej;
        private static bool _process;
        public static SpellSlot Smite;
        private static readonly int Hydra;
        private static readonly int Tiamat;
        private static readonly int Youm;
        private static readonly int Omen;

        static LeeSin()
        {
            Hydra = 3074;
            Tiamat = 3077;
            Youm = 3142;
            Omen = 3143;
        }

        public enum steps
        {
            Q,
            Q2,
            WardJump,
            WardJumpMelee,
            FlashMelee,
            Flash,
            R,
            WFlash,

        }

        private static DamageToUnitDelegate _damageToMonster;

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);


        public static DamageToUnitDelegate DamageToMonster
        {
            get { return _damageToMonster; }

            set
            {
                if (_damageToMonster == null)
                {
                    Drawing.OnDraw += OnCamps;
                }
                _damageToMonster = value;

                #endregion
            }
        }


        #region On Load

        internal static void Load(EventArgs args)
        {
            MenuConfig.OnLoad();
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 375);
            Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);

            foreach (var spell in Player.Spellbook.Spells)
            {
                if (spell.Name.Contains("smite"))
                    Smite = spell.Slot;
            }

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnDraw += OnCamps;
            Drawing.OnDraw += OnSpells;
            GameObject.OnCreate += OnCreate;
            CustomEvents.Unit.OnDash += OnDash;
            Obj_AI_Base.OnProcessSpellCast += OnSpellcast;
            Spellbook.OnCastSpell += OnSpell;
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!GetBool("wardinsec", typeof (KeyBind))) return;

            if (sender.Name.Contains("ward") && W.IsReady() && sender.IsAlly && !sender.IsDead && sender.IsValid) 
            {
                W.Cast((Obj_AI_Base) sender);
            }
        }

        #endregion


        #region dash,spellcast,

        private static void OnSpellcast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender != null && (sender.IsMe || sender.IsAlly || !sender.IsChampion())) return;

            switch (args.SData.Name)
            {
                case "MonkeyKingDecoy":
                case "AkaliSmokeBomb":
                    if (sender.Distance(Player) < E.Range)
                        E.Cast();
                    break;
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
//            if (sender.IsMe || !sender.IsChampion() || sender.IsAlly) return;
//
//            if (args.EndPos.Distance(Player) < 200)
//            {
//                Jump(args.EndPos.Extend(Player.Position.To2D(), args.EndPos.Distance(Player.Position) + 300).To3D2());
//            }
//
//            if (sender.Distance(Player) < 250 && R.IsReady())
//            {
//                R.Cast(sender);
//            }
        }

        private static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W && GetBool("wardinsec", typeof(KeyBind)))
            {
                _processw = true;
                lastprocessw = Environment.TickCount;
            }

            if (args.Slot == Player.GetSpellSlot("summonerflash") && GetBool("wardinsec", typeof(KeyBind)))
            {
                _processr = true;
                lastprocessr = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.W || args.Slot == SpellSlot.E || args.Slot == SpellSlot.Q)
            {
                _process = true;
            }
            if (args.Slot == SpellSlot.Q && Player.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" &&
                GetBool("wardinsec", typeof (KeyBind)))
            {
                Playerpos = Player.Position;
            }

            Utility.DelayAction.Add(1000, () => _process = false);

        }

        #endregion

        #region Jump, Insec pos,has passive, on update

        public static void Jump(Vector3 pos)
        {
            foreach (var wards in ObjectManager.Get<Obj_AI_Base>())
            {

                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                    target = TargetSelector.GetSelectedTarget();
                if (target == null) return;

                if (wards.IsAlly &&
                    wards.Distance(Player.Position.Extend(target.Position, Player.Distance(target.Position + 270))) <
                    200 && wards.Name.ToLower().Contains("ward") &&
                    Player.GetSpell(SpellSlot.W).Name != "blindmonkwone")
                {
                    W.Cast(wards);
                }

                var ward = Items.GetWardSlot();
                if (W.IsReady() && ward != null && ward.IsValidSlot() && Environment.TickCount - _lastwards > 400 &&
                    Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
                {
                    Player.Spellbook.CastSpell(ward.SpellSlot, pos);
                    _lastwards = Environment.TickCount;
                }
            }
        }

        public static Vector2 Insec(Obj_AI_Hero target)
        {
            {
//            switch (GetStringValue("wardinsecmode")) // disabled due to minor bugs with allies that causes the insec to do it backwards, fix soontm
//            {
//                case 0:
//                    var turrets = ObjectManager.Get<Obj_AI_Turret>().Where(x => x.IsAlly && !x.IsDead && x.Distance(target) < 800);
//                    var allies = HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && x.Distance(Player) < 1200).OrderBy(x => x.Distance(Player)).FirstOrDefault();
//                    var objAiTurrets = turrets as Obj_AI_Turret[] ?? turrets.ToArray();
//
//                    if (objAiTurrets.Any())
//                    {
//                        foreach (var turret in objAiTurrets.Where(turret => turret != null))
//                        {
//                            Game.PrintChat("to tower");
//                            return
//                                turret.Position.Extend(target.Position, turret.Distance(target.Position + 260))
//                                    .To2D();
//                            
//                        }
//                    }
//
//                   if (!objAiTurrets.Any() && Playerpos.Distance(target.Position) > 350 && allies != null &&
//                             Playerpos.Distance(target.Position) <
//                             Playerpos.Distance(allies.Position.Extend(target.Position,
//                                 allies.Distance(target.Position + 200)))) 
//                    {
//                        Game.PrintChat("to ally");
//                         return allies.Position.Extend(target.Position, allies.Distance(target.Position + 270)).To2D();
//                       
//                    }
//
//                   if (!objAiTurrets.Any() && allies != null && Playerpos.Distance(target.Position) >= 
//                             Playerpos.Distance(allies.Position.Extend(target.Position,
//                                 allies.Distance(target.Position + 200))))
//                    {
//                        return Player.Position.Extend(target.Position, Player.Distance(target.Position + 270)).To2D();
//                    }
//
//                    if (!objAiTurrets.Any() && allies != null && Playerpos.Distance(target.Position) < 350)
//                    {
//                        Game.PrintChat("to playa");
//                        return Player.Position.Extend(target.Position, Player.Distance(target.Position + 270)).To2D();
//                    }
//
//                    if (!objAiTurrets.Any() && allies == null)
//                    {
//                        Game.PrintChat("to playa");
//                        return Player.Position.Extend(target.Position, Player.Distance(target.Position + 270)).To2D();
//                    }
//
//                    break;
//                case 1:
//                {                    
                return Player.Position.Extend(target.Position, Player.Distance(target.Position + 260)).To2D();
            }

//                    break;
//                default:
//                    Console.WriteLine("Plantb made me do it");
//                    break;

        }

        public static Vector2 Star(Obj_AI_Hero target)
        {
            return Player.Position.Extend(target.Position, target.Distance(Player) + 300).To2D();
        }

        public static bool HasPassive()
        {
            return Player.HasBuff("blindmonkpassive_cosmetic");
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.IsRecalling() || MenuGUI.IsChatOpen || MenuGUI.IsShopOpen) return;

            if (_processw && Environment.TickCount - lastprocessw > 500)
            {
                Utility.DelayAction.Add(2500, () => _processw = false);
            }

            if (_processr && Environment.TickCount - lastprocessr > 100)
            {
                Utility.DelayAction.Add(400, () => _processr = false);
            }

            if (GetBool("smiteenable", typeof (KeyBind)))
            {
                AutoSmite();
            }
            if (GetBool("wardjump", typeof (KeyBind)))
            {
                WardJump();
            }

            if (GetBool("wardinsec", typeof (KeyBind)))
            {
                Orbwalking.MoveTo(Game.CursorPos);
                Wardinsec();
            }

            if (GetBool("starcombo", typeof (KeyBind)))
            {
                StarCombo();
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
            }
//            if (GetBool("wardinsec", typeof(KeyBind)) || GetBool("starcombo", typeof(KeyBind)) ||
//                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
//            {
//                if (!Q.IsReady() || Player.Spellbook.GetSpell(SpellSlot.Q).Name != "BlindMonkQOne") return;
//                if (!Smite.IsReady()) return;
//                var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
//                if (target == null) return;
//
//                var prediction = Prediction.GetPrediction(target, Q.Delay);
//
//                var collision = Q.GetCollision(Player.Position.To2D(),
//                    new List<Vector2> {prediction.UnitPosition.To2D()});
//
//                if (collision.Count == 1 && collision[0].IsMinion && collision[0].Health < SmiteDamage(collision[0]) &&
//                    collision[0].Distance(Player) < 500 && Smite.IsReady() && target.Distance(Player) < Q.Range - 300)
//                {
//                    Player.Spellbook.CastSpell(Smite, collision[0]);
//                }
//            }
        }

        #endregion

        #region Jungle Clear

        private static void JungleClear()
        {
            var jungleminion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    Q.Range,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();
            var jungleminions =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    400,
                    MinionTypes.All,
                    MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth);
            if (jungleminion == null) return;
            if (jungleminions.FirstOrDefault() == null) return;
            var useq = GetBool("useqjl", typeof (bool));
            var usew = GetBool("usewjl", typeof (bool));
            var usee = GetBool("useejl", typeof (bool));
            var usesmart = GetBool("usesjl", typeof (bool));

            if (GetStringValue("hydrati") == 0 || GetStringValue("hydrati") == 2)
            {
                if (jungleminions.Count > 0 && (ItemReady(Tiamat) || ItemReady(Hydra)) &&
                    (HasItem(Tiamat) || HasItem(Hydra)))
                {
                    SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                }
            }

            if (useq)
            {
                if (usesmart)
                {
                    if (Q.IsReady() && !HasPassive() &&
                        Player.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" &&
                        Environment.TickCount - _lastqj > 200 &&
                        Environment.TickCount - _lastwj > 200 && Environment.TickCount - _lastej > 200)
                    {
                        Q.Cast(jungleminion);
                        _lastqj = Environment.TickCount;
                    }
                    if (!HasPassive() &&
                        Environment.TickCount - _lastqj > 200 &&
                        Environment.TickCount - _lastwj > 200 && Environment.TickCount - _lastej > 200)
                    {
                        Q.Cast();
                    }
                }
                else
                {
                    if (Q.IsReady() && Player.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
                    {
                        Q.Cast(jungleminion);
                    }
                    if (Q.IsReady() && Player.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo")
                    {
                        Q.Cast();
                    }
                }
            }

            if (usew)
            {
                if (usesmart)
                {
                    if (!HasPassive() && W.IsReady() && Player.Mana > 100 &&
                        Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne" &&
                        Player.Distance(jungleminion) < Player.AttackRange + Player.BoundingRadius
                        && Environment.TickCount - _lastqj > 200 &&
                        Environment.TickCount - _lastwj > 200 && Environment.TickCount - _lastej > 200)

                    {
                        W.Cast(Player);
                        _lastwj = Environment.TickCount;
                    }
                    if (!HasPassive() &&
                        Player.Distance(jungleminion) < Player.AttackRange + Player.BoundingRadius &&
                        Environment.TickCount - _lastqj > 200 &&
                        Environment.TickCount - _lastwj > 200 && Environment.TickCount - _lastej > 200)
                    {
                        W.Cast();
                    }
                }
                else
                {
                    if (W.IsReady() && jungleminion.Distance(Player) < Player.AttackRange)
                    {
                        W.Cast(Player);
                    }
                }
            }

            if (usee)
            {
                if (usesmart)
                {
                    if (E.IsReady() && jungleminion.Distance(Player) <= E.Range &&
                        Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne" &&
                        !HasPassive() &&
                        Environment.TickCount - _lastqj > 200 &&
                        Environment.TickCount - _lastwj > 200 && Environment.TickCount - _lastej > 200)
                    {
                        E.Cast();
                        _lastej = Environment.TickCount;
                    }
                    if (jungleminion.Distance(Player) <= Player.AttackRange + Player.BoundingRadius &&
                        !HasPassive() &&
                        Player.GetSpell(SpellSlot.E).Name == "blindmonketwo"
                        && Environment.TickCount - _lastqj > 200 &&
                        Environment.TickCount - _lastwj > 200 && Environment.TickCount - _lastej > 200)
                    {
                        E.Cast();
                    }
                }

            }

        }

        #endregion

        #region Lane clear

        private static void LaneClear()
        {
            var minion =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    Q.Range,
                    MinionTypes.All,
                    MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            var minione =
                MinionManager.GetMinions(
                    Player.ServerPosition,
                    E.Range,
                    MinionTypes.All,
                    MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth);

            if (minion == null) return;
            if (minione.FirstOrDefault() == null) return;



            if (minion.Health <= Player.GetAutoAttackDamage(minion) &&
                !minion.HasBuff("BlindMonkQOne")
                && minion.Distance(Player) <= Player.AttackRange + Player.BoundingRadius) return;


            var useq = GetBool("useql", typeof (bool));
            var usee = GetBool("useel", typeof (bool));
            var useeslider = GetValue("useelv");

            if (usee && minione.Count >= useeslider && E.IsReady() && minion.Distance(Player) < E.Range &&
                Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne"
                && Player.GetSpell(SpellSlot.Q).Name != "blindmonkqtwo")
            {
                E.Cast();
                _lastelane = Environment.TickCount;
            }

            if (minion.Distance(Player) < Player.AttackRange + Player.BoundingRadius
                && (Player.GetSpell(SpellSlot.E).Name == "blindmonketwo" && Environment.TickCount - _lastelane > 2900))
            {
                E.Cast();
            }

            foreach (var minions in minione)
            {

                if (GetStringValue("hydrati") == 1 || GetStringValue("hydrati") == 2)
                {
                    if (minione.Count > 2 && (ItemReady(Tiamat) || ItemReady(Hydra)) &&
                        (HasItem(Tiamat) || HasItem(Hydra)))
                    {
                        SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                    }
                }

                if (Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqone" &&
                    minions.Health <= GetQDamage(minions) && useq &&
                    minions.Health >= Q.GetDamage(minions))
                {
                    Q.Cast(minions);
                }
                if (Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqtwo" && Q.IsReady() &&
                    minions.HasBuff("BlindMonkQOne"))
                {
                    Q.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }
                if (minions.Health <= Q.GetDamage(minions) && Q.IsReady() &&
                    Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
                {
                    Q.Cast(minions);
                }
            }


        }

        #endregion

        #region Q Damage

        public static float GetQDamage(Obj_AI_Base unit)
        {
            var firstq = Q.GetDamage(unit);
            var secondq = Q.GetDamage(unit) + (unit.MaxHealth - unit.Health - Q.GetDamage(unit))*0.08;
            return (float) (firstq + secondq);
        }

        #endregion

        #region Combo

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
                return;
            if (Player.IsWindingUp) return;
            var useq = GetBool("useq", typeof (bool));
            var usee = GetBool("usee", typeof (bool));
            var user = GetBool("user", typeof (bool));
            var smite = GetBool("usessmite", typeof (bool));
            if (GetStringValue("hydrati") == 0 || GetStringValue("hydrati") == 2)
            {
                if (target.IsValidTarget(400) && (ItemReady(Tiamat) || ItemReady(Hydra)) &&
                    (HasItem(Tiamat) || HasItem(Hydra)))
                {
                    SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                }
            }

            if (GetBool("youm", typeof (bool)) && HasItem(Youm) && ItemReady(Youm) &&
                target.Distance(Player) < Q.Range - 300)
            {
                SelfCast(Youm);
            }

            if (GetBool("omen", typeof (bool)) && HasItem(Omen) && ItemReady(Omen) &&
                Player.CountAlliesInRange(400) >= GetValue("minrand"))
            {
                SelfCast(Omen);
            }

            if (useq)
            {
                var qpred = Q.GetPrediction(target);
                if (Q.IsReady() && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" &&
                    qpred.Hitchance >= HitChance.Medium)
                {
                    Q.Cast(target);
                    _lastqc = Environment.TickCount;
                }

                if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blinkmonkqtwo" && Q.IsReady() &&
                    (Environment.TickCount - _lastqc > 1000 ||
                     target.Distance(Player) > Player.AttackRange + 200))
                {
                    Q.Cast();
                }
            }

            if (usee)
            {
                if (target.Distance(Player) <= E.Range &&
                    Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne")
                {
                    E.Cast();
                    _laste = Environment.TickCount;
                }
                if ((Player.Distance(target) > Player.AttackRange + Player.BoundingRadius + target.BoundingRadius + 100 ||
                     Environment.TickCount - _laste > 2700) && Player.GetSpell(SpellSlot.E).Name == "blindmonketwo")
                {
                    E.Cast();
                }
            }

            if (user)
            {
                if (Q.IsReady() && R.IsReady() && target.IsValidTarget(R.Range) &&
                    Player.Mana > 80 &&
                    target.Health <= R.GetDamage(target) + GetQDamage(target) + Player.GetAutoAttackDamage(target))
                {
                    R.Cast(target);
                }
            }

            if (Smite.IsReady() && target.Distance(Player) < 500 && smite)
            {
                Player.Spellbook.CastSpell(Smite, target);
            }

//            foreach (var heros in HeroManager.Enemies.OrderBy(x => x.Distance(Player)))
//            {
//                var prediction = Prediction.GetPrediction(target, Rk.Delay, heros.BoundingRadius);
//                var collision =
//                    prediction.CollisionObjects.Where(x => x.IsEnemy && !x.IsDead && x.Distance(Player) < 1200).ToList();
//                    if (collision.Count() >= 2)
//                    {
//                        R.Cast(collision[0]);
//                    }
//            }
//            Game.PrintChat(AngleBetween2Points(target, Player).ToString());
//            var hero = HeroManager.Enemies.Where(
//                x =>
//                    Math.Abs(AngleBetween2Points(Player, x) - AngleBetween2Points(x, x)) < 1&& !x.IsDead &&
//                    x.IsChampion() && x.Distance(Player) < 1200).ToList();
//
//            if (hero[0].Distance(Player) < R.Range)
//            {
//                if (hero.Count > 1)
//                {
//                    R.Cast(hero[0]);
//                }
//            }


        }

        #endregion

        #region starcombo +angle

        public static float AngleBetween2Points(Obj_AI_Base first, Obj_AI_Base second)
        {
            return first.Position.To2D().AngleBetween(second.Position.To2D());
        }

        private static void StarCombo()
        {
            Orbwalking.MoveTo(Game.CursorPos);

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

//            var slot = Items.GetWardSlot();
//            if (target.IsValidTarget(Q.Range) && Q.IsReady() &&
//                Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" &&
//                target.Distance(Player) > R.Range + 250 && R.IsReady() && slot.IsValidSlot())
//            {
//                Q.Cast(target);
//                Steps = steps.WardJump;             
//            }
//            if (slot.IsValidSlot() && target.Distance(Player) > R.Range + 250 && R.IsReady())
//            {
//                Steps = steps.WardJump;
//            }   
//            if (Steps == steps.WardJump)
//            {
//                Jump(Player.Position.Extend(target.Position, target.Distance(Player) - 200));
//            }


            if (Smite.IsReady() && target.Distance(Player) < 500)
            {
                Player.Spellbook.CastSpell(Smite, target);
            }

            if (R.IsReady() && target.IsValidTarget(R.Range))
            {
                Steps = steps.Q;
            }

            if (Q.IsReady() && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" && Steps == steps.Q)
            {
                Q.Cast(target);
            }

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo" &&
                target.IsValidTarget(R.Range))
            {
                R.Cast(target);
            }

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqtwo" && Q.IsReady() && !R.IsReady())
            {
                Utility.DelayAction.Add(300, () => Q.Cast());
            }

        }

        #endregion


        private static void Wardinsec()
        {
            #region Target, Slots, Prediction
            Game.PrintChat(Steps.ToString());
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                target = GetStringValue("targetmode") == 1
                    ? TargetSelector.GetSelectedTarget()
                    : target;
            }

            

            var slot = Items.GetWardSlot();
            if (target == null) return;
            var qpred = Q.GetPrediction(target);
            var col = qpred.CollisionObjects;

            #endregion

            #region Smite Cause why not

//            if (Smite.IsReady() && target.Distance(Player) < 500 && GetBool("UseSmite", typeof(bool)))
//            {
//                Player.Spellbook.CastSpell(Smite, target);
//            }

            #endregion

            #region Ward Jump

            if (Steps == steps.WardJump || Environment.TickCount - lastwardjump < 2000)
            {
                if (Player.Distance(target) <= 170 && W.IsReady())
                {
                    foreach (var wards in ObjectManager.Get<Obj_AI_Base>())
                    {
                        if (wards.IsAlly &&
                            wards.Distance(Player.Position.Extend(target.Position,
                                Player.Distance(target.Position + 265))) <
                            200 && wards.Name.ToLower().Contains("ward") &&
                            Player.GetSpell(SpellSlot.W).Name != "blindmonkwone")
                        {
                            W.Cast(wards);
                        }
                    }

                    var pos = Player.Position.Extend(target.Position, Player.Distance(target.Position + 270));
                    if (Environment.TickCount - _lastwards > 1000 &&
                        Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
                    {
                        Player.Spellbook.CastSpell(slot.SpellSlot, pos);
                        _lastwards = Environment.TickCount;
                    }
                }
            }
#endregion

            #region R Casting
            if (_processw || _processr)
            {
                R.Cast(target);
            }
            #endregion

            #region Determine if we want to flash or ward jump

            if (R.IsReady())
            {
                if (slot.IsValidSlot() && slot != null && W.IsReady() && Player.Distance(target) <= 150)
                {
                    Steps = steps.WardJump;
                    lastwardjump = Environment.TickCount;
                    //   Game.PrintChat("Wardjump");
                }
                else if (!slot.IsValidSlot() && slot == null &&
                         GetBool("useflash", typeof (bool)) &&
                         Player.GetSpellSlot("summonerflash").IsReady() && Environment.TickCount - lastwardjump > 1000)
                {
                    Steps = steps.Flash;
                 //   Game.PrintChat("Wardflashe");
                }
            }

            #endregion

            #region General Q Casting

            if (Q.IsReady() && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" &&
                qpred.Hitchance >= HitChance.Medium)
            {
                Q.Cast(target);
                if (slot != null && Environment.TickCount - lastwardjump > 1000 && R.IsReady() && W.IsReady() && target.Distance(Player) > 300)
                {
                    Steps = steps.WardJump;
                }
            }

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo" && (target.Distance(Player) > 300 || (Environment.TickCount - lastprocessw > 400 && _processw))) 
            {
                Utility.DelayAction.Add(200, () => Q.Cast());
            }

            #endregion

            #region Flash Casting

            if (Steps != steps.Flash) return;

            if (Player.Distance(target) < 200 && R.IsReady() &&
                Player.Spellbook.GetSpell(Player.GetSpellSlot("summonerflash")).IsReady())
            {
                Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerflash"),
                    Player.Position.Extend(target.Position, Player.Distance(target) + 250));
            }
//            else if (R.IsReady() && Player.Spellbook.GetSpell(Player.GetSpellSlot("summonerflash")).IsReady()
//                     && Player.Distance(target) < 500 && Player.Distance(target) > 200)
//            {
//                Jump(Player.Position.Extend(target.Position, Player.Distance(target) - 80));
//
//                if (Player.Distance(target) < 100 && R.IsReady() &&
//                    Player.Spellbook.GetSpell(SpellSlot.Summoner2).IsReady())
//                {
//                    R.Cast(target);
//                    Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerflash"),
//                        Player.Position.Extend(target.Position, Player.Distance(target) + 250));
//                }
//            }

            #endregion

            #region Future stuff


//            else if (Steps == steps.FlashMelee && slot == null && Steps != steps.WardJump && Steps!= steps.WardJumpMelee && Steps != steps.R)
//            {
//                if (Player.Distance(target) < 250 && R.IsReady() &&
//                    Player.Spellbook.GetSpell(Player.GetSpellSlot("summonerflash")).IsReady())
//                {
//                    R.Cast(target);
//                    Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerflash"),
//                        Player.Position.Extend(target.Position, Player.Distance(target) + 130));
//                }
//            }

            //            if (Player.Distance(target) <= 200 && R.IsReady() && Steps != steps.Q2 && Steps != steps.WardJump
            //                && Steps == null && Steps != steps.R && slot != null)
            //            {
            //                Steps = steps.WardJumpMelee;
            //                Game.PrintChat("MeleeWardjump");
            //            }
            //
            //            if (Player.Distance(target) <= 250 && R.IsReady() && Player.GetSpellSlot("summonerflash").IsReady() &&
            //                slot == null && Steps == null && Steps != steps.R && Steps != steps.WardJump &&
            //                Steps != steps.WardJumpMelee
            //                && Steps != steps.Q2)  
            //            {
            //                Steps = steps.FlashMelee;
            //                Game.PrintChat("Wardflashmelee");
            //            }


            //            if (col.Any() && Player.Distance(target) > 500 && W.IsReady() && R.IsReady() &&
            //                Player.GetSpellSlot("summonerflash").IsReady()) 
            //            {
            //                if (Steps != steps.Q2 && !_wcasteds && Steps != steps.WardJump && Steps != steps.WardJumpMelee)
            //                {
            //                    Steps = steps.WFlash;
            //                }
            //            }
            //
            //            if (Steps == steps.WFlash)
            //            {
            //                Jump(Player.Position.Extend(target.Position, Player.Distance(target) - 200));
            //                Steps = steps.FlashMelee;
            //            }

            //            if (Steps != steps.Q2 && !_wcasteds && R.IsReady()) 
            //            {
            //                if (target.Distance(Player) <= 300 && Steps != steps.WardJump && Steps != steps.Flash)
            //                {
            //                    if (slot != null && W.IsReady() && slot.IsValidSlot())
            //                    {
            //                        Steps = steps.WardJumpMelee;
            //                        Game.PrintChat("Wardjumpmeele");
            //                    }
            //                    else if (slot == null && !_wcasteds && !W.IsReady() && Steps != steps.WardJumpMelee && Steps != steps.WardJump &&
            //                             Player.GetSpellSlot("summonerflash").IsReady() && GetBool("useflash", typeof (bool))) 
            //                    {
            //                        Steps = steps.FlashMelee;
            //                        Game.PrintChat("Wardflashemeele");
            //                    }
            //                }
            //            }

            #endregion

        }

        #region Ward Jump

        private static void WardJump()
        {

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            var pos = Player.Position.Extend(Game.CursorPos, 590);
            var minions =
                MinionManager
                    .GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Ally)
                    .FirstOrDefault(x => !x.IsDead && x.IsValid && x.Distance(pos) < 200);

            var allies = HeroManager.Allies.FirstOrDefault(x => !x.IsDead && x.IsValid && x.Distance(pos) < 200);
                //todo use pinkwards or not
            foreach (var wards in ObjectManager.Get<Obj_AI_Base>())
            {
                if (!_processw && W.IsReady() && Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne" &&
                    Player.Spellbook.GetSpell(SpellSlot.Q).Name != "blindmonkwtwo"
                    && ((wards.Name.ToLower().Contains("ward") &&
                         wards.Distance(Player.Position.Extend(Game.CursorPos, 590)) < 200 && wards.IsAlly) ||
                        (allies != null) || (minions != null)))
                {


                    if (allies != null && minions == null)
                        W.CastOnUnit(allies);
                    if (allies != null && minions != null)
                        W.CastOnUnit(allies);
                    if (minions != null && allies == null)
                        W.CastOnUnit(minions);
                    if (allies == null && minions == null)
                        W.Cast(wards);

                }
            }

            var ward = Items.GetWardSlot();
            if (W.IsReady() && ward != null && !_casted && ward.IsValidSlot() && Environment.TickCount - _lastward > 400 &&
                Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne" && allies == null && minions == null)
            {
                Player.Spellbook.CastSpell(ward.SpellSlot, Player.Position.Extend(Game.CursorPos, 590));
                _lastward = Environment.TickCount;
            }

        }

        #endregion

        #region Everything Jungle Based

        public static readonly string[] Names =
        {
            "Krug",
            "Razorbeak",
            "Murkwolf",
            "Gromp",
            "Crab",
            "Blue",
            "Red",
            "Dragon",
            "Baron"
        };

        private static int _lastwards;
        private static bool _castedward;
        private static int lastprocessw;
        private static int lastprocessward;
        private static int lastwardjump;
        private static bool _processr;
        private static int lastprocessr;

        private static void AutoSmite()
        {
            if (!GetBool("smiteonkillable", typeof (bool))) return;

            foreach (var mob in
                MinionManager.GetMinions(Player.Position, 550, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth))
            {
                foreach (var name in Names)
                {
                    if (mob.CharData.BaseSkinName == "SRU_" + name
                        && GetBool("usesmiteon" + name, typeof (bool)))
                    {
                        if (!mob.IsValidTarget()) return;
                        if (SmiteDamage(mob) > mob.Health && Smite.IsReady())
                        {
                            Player.Spellbook.CastSpell(Smite, mob);
                        }

                        if (GetBool("qcalcsmite", typeof (bool)))
                        {
                            if ((SmiteDamages(mob) >= mob.Health && Q.IsReady()))
                            {
                                Q.Cast(mob);
                                if (mob.HasBuff("blindmonkqtwo"))
                                {
                                    Player.Spellbook.CastSpell(Smite, mob);
                                }
                                Q.Cast();
                            }

                            if (GetFuckingSmiteDamage() + Q.GetDamage(mob) + (mob.MaxHealth - mob.Health)*0.08 >=
                                mob.Health
                                && Q.IsReady() && Player.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo" &&
                                mob.HasBuff("blindmonkqtwo"))
                            {
                                Q.Cast();
                                Player.Spellbook.CastSpell(Smite, mob);
                            }
                        }
                    }
                }
            }
        }

        public static float SmiteDamage(Obj_AI_Base target)
        {
            float damage = 0;

            if (Smite.IsReady())
            {
                if (target.IsValidTarget(500))
                    damage += GetFuckingSmiteDamage();
            }

            return damage;
        }


        public static float SmiteDamages(Obj_AI_Base target)
        {
            float damage = 0;

            if (Smite.IsReady())
            {
                if (target.IsValidTarget(500))
                    damage += GetFuckingSmiteDamage();
            }

            if (Q.IsReady() && target.IsValidTarget(Q.Range) && GetBool("qcalcsmite", typeof (bool)))
            {
                damage += GetQDamage(target);
            }

            return damage;
        }


        private static float GetFuckingSmiteDamage()
        {
            var level = Player.Level;
            var index = Player.Level/5;
            float[] dmgs =
            {
                370 + 20*level,
                330 + 30*level,
                240 + 40*level,
                100 + 50*level
            };
            return dmgs[index];
        }

        private static void OnCamps(EventArgs args)
        {
            if (!GetBool("jungledraws", typeof (bool))) return;
            if (!GetBool("ovdrawings", typeof (bool))) return;
            if (GetBool("enabledisablesmite", typeof (bool)))
            {
                var color = GetBool("smiteenable", typeof (KeyBind)) ? Color.LimeGreen : Color.Black;
                var text = GetBool("smiteenable", typeof (KeyBind)) ? "Smite Enabled!" : "Smite Disabled!";
                Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X - 20,
                    Drawing.WorldToScreen(Player.Position).Y - 20, color, text);
            }

            if (GetBool("jungledraw", typeof (bool)))
            {
                foreach (var minion in ObjectManager.Get<Obj_AI_Minion>())
                {
                    if (minion.Team == GameObjectTeam.Neutral && minion.IsValidTarget() && minion.IsHPBarRendered)
                    {
                        var smiteDamage = SmiteDamages(minion);

                        // Monster bar widths and offsets from ElSmite
                        var barWidth = 0;
                        var xOffset = 0;
                        var yOffset = 0;
                        var yOffset2 = 0;
                        var display = true;
                        string name = "";
                        switch (minion.CharData.BaseSkinName)
                        {
                            case "SRU_Red":
                                barWidth = 145;
                                xOffset = 3;
                                yOffset = 18;
                                yOffset2 = 10;
                                name = "Red Buff";
                                break;

                            case "SRU_Blue":
                                barWidth = 145;
                                xOffset = 3;
                                yOffset = 18;
                                yOffset2 = 10;
                                name = "Blue Buff";
                                break;

                            case "SRU_Dragon":
                                barWidth = 145;
                                xOffset = 3;
                                yOffset = 18;
                                yOffset2 = 10;
                                name = "Dragon";
                                break;

                            case "SRU_Baron":
                                barWidth = 194;
                                xOffset = -22;
                                yOffset = 13;
                                yOffset2 = 16;
                                name = "Baron";
                                break;

                            case "Sru_Crab":
                                barWidth = 61;
                                xOffset = 45;
                                yOffset = 34;
                                yOffset2 = 3;
                                name = "Crab";
                                break;

                            case "SRU_Krug":
                                barWidth = 81;
                                xOffset = 58;
                                yOffset = 18;
                                yOffset2 = 4;
                                name = "Krug";
                                break;

                            case "SRU_Gromp":
                                barWidth = 87;
                                xOffset = 62;
                                yOffset = 18;
                                yOffset2 = 4;
                                name = "Gromp";
                                break;

                            case "SRU_Murkwolf":
                                barWidth = 75;
                                xOffset = 54;
                                yOffset = 19;
                                yOffset2 = 4;
                                name = "Murkwolf";
                                break;

                            case "SRU_Razorbeak":
                                barWidth = 75;
                                xOffset = 54;
                                yOffset = 18;
                                yOffset2 = 4;
                                name = "Razorbeak";
                                break;

                            default:
                                display = false;
                                break;
                        }
                        if (!display) continue;
                        var barPos = minion.HPBarPosition;
                        var percentHealthAfterDamage = Math.Max(0, minion.Health - smiteDamage)/minion.MaxHealth;
                        var yPos = barPos.Y + yOffset;
                        var xPosDamage = barPos.X + xOffset + barWidth*percentHealthAfterDamage;
                        var xPosCurrentHp = barPos.X + xOffset + barWidth*minion.Health/minion.MaxHealth;

                        var differenceInHp = xPosCurrentHp - xPosDamage;
                        var pos1 = barPos.X + xOffset;

                        for (var i = 0; i < differenceInHp; i++)
                        {
                            Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + yOffset2, 1, Color.OrangeRed);
                        }

                        Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + yOffset2, 1, Color.Red);
                        Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y, Color.Red, name);
                        if (GetBool("killmob", typeof (bool)))
                        {
                            if (smiteDamage >= minion.Health)
                            {
                                Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y,
                                    Color.Red, "Killable");
                            }
                        }
                    }
                }
            }
        }



        private static void OnSpells(EventArgs args)
        {
            if (!GetBool("spellsdraw", typeof (bool))) return;
            if (!GetBool("ovdrawings", typeof (bool))) return;
            if (GetBool("qrange", typeof (bool)) && Q.Level > 0)
            {
                var color = Q.IsReady() ? Color.DodgerBlue : Color.Red;
                Render.Circle.DrawCircle(Player.Position, Q.Range, color);
            }

            if (GetBool("wrange", typeof (bool)) && W.Level > 0)
            {
                var colorw = W.IsReady() ? Color.BlueViolet : Color.Red;
                Render.Circle.DrawCircle(Player.Position, W.Range, colorw);
            }

            if (GetBool("erange", typeof (bool)) && E.Level > 0)
            {
                var colore = E.IsReady() ? Color.Plum : Color.Red;
                Render.Circle.DrawCircle(Player.Position, E.Range, colore);
            }

            if (GetBool("rrange", typeof (bool)) && R.Level > 0)
            {
                var colorr = R.IsReady() ? Color.LawnGreen : Color.Red;
                Render.Circle.DrawCircle(Player.Position, R.Range, colorr);
            }

        }



        private static void OnDraw(EventArgs args)
        {

            if (!GetBool("spellsdraw", typeof (bool))) return;
            if (!GetBool("targetexpos", typeof (bool))) return;
            if (!GetBool("ovdrawings", typeof (bool))) return;
            var allies =
                HeroManager.Allies.Where(x => !x.IsMe && !x.IsDead && x.Distance(Player) < 1200)
                    .OrderBy(x => x.Distance(Player))
                    .FirstOrDefault();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target != null && !target.IsDead)
            {
                target = TargetSelector.SelectedTarget;
            }

            if (target == null) return;

            if (target.IsDead) return;

//            if (allies != null && GetStringValue("wardinsecmode") == 0)
//            {
//                if (Playerpos.Distance(target.Position) <
//                    Playerpos.Distance(allies.Position.Extend(target.Position,
//                        allies.Distance(target.Position + 350))))
//                {                  
//                    var pos1 = Drawing.WorldToScreen(target.Position);
//                    var pos2 = Drawing.WorldToScreen(target.Position.Extend(allies.Position, 1200));
//                    Drawing.DrawLine(pos1, pos2, 3,
//                        Color.Red);
//                    Drawing.DrawCircle(target.Position.Extend(allies.Position, 1200), 100, Color.Blue);
//                    Drawing.DrawText(pos2.X, pos2.Y, Color.Black, "XA");
//                }
//
//            }
            var pos1 = Drawing.WorldToScreen(target.Position);
            var pos2 = Drawing.WorldToScreen(target.Position.Extend(Player.Position, 1200));
            Drawing.DrawLine(pos1, pos2, 3, Color.Red);
            Drawing.DrawCircle(target.Position.Extend(Player.Position, 1200), 100, Color.Blue);
            Drawing.DrawText(pos2.X, pos2.Y, Color.Black, "X");

        }

        #endregion
        public static Vector3 Playerpos { get; set; }
    }
}
