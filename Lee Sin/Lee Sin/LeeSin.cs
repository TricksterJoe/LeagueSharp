using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Version = System.Version;

namespace Lee_Sin
{
    internal class LeeSin : Helper
    {
        #region vars, enums, misc
        public static bool CanFlash = false;
       public static  bool CanWard = false;
        public static Spell Q, W, E, R;
        private static SpellSlot _flashSlot;
        private static int _lastward;
        public static steps? Steps;
        private static int _laste;
        private static int _lastqc;
        private static int _lastelane;
        private static bool _processw;
        private static int _lastqj;
        private static int _lastwj;
        private static int _lastej;
        public static SpellSlot Smite;
        private static readonly int Hydra;
        private static readonly int Tiamat;
        private static readonly int Youm;
        private static readonly int Omen;
        public static Obj_AI_Base SelectedAllyAiMinion;

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
            WFlash

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

            }
        }
        #endregion

        #region On Load

        internal static void Load(EventArgs args)
        {
            if (Player.ChampionName != "LeeSin") return;
            MenuConfig.OnLoad();
            Q = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 375);
            Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);
            _flashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");

            foreach (var spell in Player.Spellbook.Spells)
            {
                if (spell.Name.ToLower().Contains("smite"))
                    Smite = spell.Slot;
            }
            Printmsg("Lee Sin By Hoes Assembly Loaded");
            Printmsg1("Current Version: " + typeof(Program).Assembly.GetName().Version);
            Printmsg2("Don't Forget To " + "<font color='#00ff00'>[Upvote]</font> <font color='#FFFFFF'>" + "The Assembly In The Databse" + "</font>");
            UpdateCheck();
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnDraw += OnCamps;
            Drawing.OnDraw += OnSpells;
            GameObject.OnCreate += OnCreate;
            Obj_AI_Base.OnProcessSpellCast += OnSpellcast;
            Spellbook.OnCastSpell += OnSpell;
            Game.OnWndProc += OnWndProc;
       //     Interrupter2.OnInterruptableTarget += OnInterrupter;
            
        }

        //private static void OnInterrupter(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        //{
        //    if (!sender.IsEnemy) return;
        //    var wardtotargetpos = Player.Position.Extend(sender.Position, Player.Distance(sender) - 250);
        //    if (sender.Distance(Player) < 1000)
        //    {
        //        WardJump(wardtotargetpos, false);
        //    }
        //    if (sender.Distance(Player) < R.Range)
        //    {
        //        R.Cast(sender);
        //    }
        //}

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;
            Game.PrintChat(args.SData.Name);
            if (args.SData.Name.Equals("BlindMonkRKick"))
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
                }

                if (target == null) return;
                if (Steps != steps.Flash && _wardjumpedtotarget == false) return;

                if (!GetBool("wardinsec", typeof(KeyBind))) return;
               Utility.DelayAction.Add(GetValue("rflashdelay"),
                () =>  Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerflash"),
                Insec(target, 200, true).To3D(
                            )));
            }
        }

        private static void Printmsg(string message)
        {
            Game.PrintChat(
                "<font color='#6f00ff'>[Slutty Lee Sin]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg1(string message)
        {
            Game.PrintChat(
                "<font color='#ff00ff'>[Slutty Lee Sin]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg2(string message)
        {
            Game.PrintChat( 
                "<font color='#00abff'>[Slutty Lee Sin]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        #endregion

        #region OnCreate

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!GetBool("wardinsec", typeof(KeyBind)) && !GetBool("starcombo", typeof(KeyBind)) &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo)
                return;

            if (_processw2 || !W.IsReady() || Player.GetSpell(SpellSlot.W).Name != "BlindMonkWOne" ||
                Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkwtwo")
                return;

            if (sender.Name.ToLower().Contains("ward") && W.IsReady() && sender.IsAlly)
            { 
                var ward = (Obj_AI_Base) sender;
                if (ward.IsMe) return;
                W.Cast(ward);
                _created = true;
                _lastwcasted = Environment.TickCount;
            }
        }

        #endregion

        #region Ally selector 

        private static void OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }

            //Credits to jQuery's ElLeeSin
            var asec = ObjectManager.Get<Obj_AI_Hero>().Where(a => a.IsEnemy && a.Distance(Game.CursorPos) < 200 && a.IsValid && !a.IsDead);
            if (asec.Any())
            {
                return;
            }
            if (!_lastClickBool || _clickCount == 0)
            {
                _clickCount++;
                _lastClickPos = Game.CursorPos;
                _lastClickBool = true;
                SelectedAllyAiMinion = null;
                SelectedAllyAiMinionv = new Vector3();
                return;
            }

            if (ClickCounts == 0)
            {
                SelectedAllyAiMinionv = new Vector3();
            }


            if (_lastClickBool && _lastClickPos.Distance(Game.CursorPos) < 100)
            {
                _clickCount++;
                _lastClickBool = false;
            }

            SelectedAllyAiMinion =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        x => x.IsValid && x.Distance(Game.CursorPos, true) < 40000 && x.IsAlly && !x.IsMe)
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();

        }

        public static int ClickCounts { get; set; }

        public static Vector3 SelectedAllyAiMinionv
        { get; set; }

        #endregion

        #region processspellcast,

        private static void OnSpellcast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                switch (args.SData.Name)
                {
                    case "BlindMonkQOne":
                    case "blinkmonkqtwo":
                        _junglelastq = Environment.TickCount;
                        break;

                    case "BlindMonkWOne":
                    case "blindmonkwtwo":
                        _junglelastw = Environment.TickCount;
                        break;

                    case "BlindMonkEOne":
                    case "blindmonketwo":
                        _junglelaste = Environment.TickCount;
                        break;
                }
            }
            if (args.SData.Name == "BlindMonkRKick")
            {
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
                }

                if (target != null)
                {
                    if (Steps == steps.Flash || (Environment.TickCount - _lastflashward < 2000 && _wardjumpedtotarget))
                    {
                        if (GetBool("wardinsec", typeof (KeyBind)) || GetBool("starcombo", typeof (KeyBind)))
                        {
                            var pos = InsecFlash(target, 230);
                            var poss = Player.Position.Extend(target.Position,
                                +target.Position.Distance(Player.Position) + 230);

                            Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"),
                                !GetBool("wardinsec", typeof (KeyBind)) ? poss : pos, true);
                        }
                    }
                }
            }

            if (sender.IsMe || sender.IsAlly || !sender.IsChampion()) return;

            switch (args.SData.Name)
            {
                case "MonkeyKingDecoy":
                case "AkaliSmokeBomb":
                    if (sender.Distance(Player) < E.Range)
                        E.Cast();
                    break;
            }
        }

        #endregion

        #region On spell cast

        private static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.W &&
                (GetBool("wardinsec", typeof (KeyBind)) || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                 Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)) 
            {
                _processw = true;
                _lastprocessw = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.Q)
            {
                _lastqcasted = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.Q && Q1())
            {
                _lastqcasted1 = Environment.TickCount;
            }


            if (args.Slot == Player.GetSpellSlot("summonerflash") && GetBool("wardinsec", typeof(KeyBind)))
            {
                _processr = true;
                _lastprocessr = Environment.TickCount;
                lastflashed = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.R && GetBool("wardinsec", typeof(KeyBind)))
            {

                _processr2 = true;
                _processr2T = Environment.TickCount;
                Playerpos = Player.Position;
            }

            if (args.Slot == SpellSlot.W && (GetBool("wardinsec", typeof(KeyBind)) || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear))
            {
                _processw2 = true;
            }

            if (args.Slot == SpellSlot.R && GetBool("wardinsec", typeof(KeyBind)))
            {
                _processr2 = true;
                _processr2T = Environment.TickCount;
               
            }

        }

        #endregion

        #region Ward Insec Jump Postion

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

        #endregion

        #region Insec Position Flash

        public static Vector3 InsecFlash(Obj_AI_Hero target, int extendvalue)
        {

            //  var pos = Player.Position.Extend(target.Position, +target.Position.Distance(Player.Position) + 230);
            if (SelectedAllyAiMinion != null)
            {
                return
                    SelectedAllyAiMinion.Position.Extend(target.Position,
                        +target.Position.Distance(SelectedAllyAiMinion.Position) + extendvalue);

            }
            var objAiHero = GetAllyHeroes(target, 1200).FirstOrDefault();
            if (GetBool("useobjectsallies", typeof (bool)) && objAiHero != null)
            {
                return
                    objAiHero.Position.Extend(target.Position,
                        +target.Position.Distance(objAiHero.Position) + extendvalue);
            }

            if (!GetBool("useobjectsallies", typeof (bool)) || objAiHero == null)
            {
                return Player.Position.Extend(target.Position,
                    +target.Position.Distance(Player.Position) + extendvalue);
            }
            return new Vector3();
        }


        #endregion

        #region Insec Position
        private static IEnumerable<Obj_AI_Hero> GetAllyHeroes(Obj_AI_Hero unit, int range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.Distance(unit) < range).OrderBy(x => x.Distance(Player))
                    .ToList();
        }

        public static Vector2 Insec(Obj_AI_Hero target, int extendvalue, bool flashcasting)
        {


            if (SelectedAllyAiMinion != null)
            {
                return
                    SelectedAllyAiMinion.ServerPosition.Extend(target.ServerPosition,
                        SelectedAllyAiMinion.Distance(target) + extendvalue).To2D();

            }
            else
            {
                var objAiHero = GetAllyHeroes(target, 1200).FirstOrDefault();
                if (GetBool("useobjectsallies", typeof (bool)) && objAiHero != null)
                {
                    return
                        objAiHero.ServerPosition.Extend(target.ServerPosition,
                            objAiHero.Distance(target) + extendvalue).To2D();
                }

                if (!GetBool("useobjectsallies", typeof (bool)) || objAiHero == null)
                {
                    return Player.ServerPosition.Extend(target.ServerPosition,
                        Player.Distance(target) + extendvalue).To2D();
                }
            }

            return new Vector2();
        }


        #endregion

        #region #Star

        public static
        Vector2 Star(Obj_AI_Hero target)
        {
            return Player.Position.Extend(target.Position, target.Distance(Player) + 300).To2D();
        }

        #endregion

        #region Has Passive

        public static bool HasPassive()
        {
            return Player.HasBuff("blindmonkpassive_cosmetic");
        }

        #endregion

        #region Auto Warding Ult X Enemies

        private static int MaxTravelDistance()
        {
            var slot = Items.GetWardSlot();
            var flash = Player.GetSpellSlot("summonerflash");
            if (slot != null && flash.IsReady() && W.IsReady())
            {
                return 1125;
            }

            if (slot != null && !W.IsReady())
            {
                return 185;
            }

            if (slot != null && W.IsReady())
            {
                return 600;
            }

            if (slot == null)
            {
                return 185;
            }

            return 185;
        }


        public static void AutoWardUlt()
        {
            var distance = MaxTravelDistance();
            var enemiescount = GetValue("enemiescount");
            var enemies = Player.GetEnemiesInRange(2800);
            var wardflashpos = Mathematics.GetWardFlashPositions(distance, Player, (byte)enemiescount, enemies);
            var wardJumpPos = Mathematics.MoveVector(Player.Position, wardflashpos);
            var enemies1 = HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(Player) < 1125).ToList();
            var getresults = Mathematics.GetPositions(Player, 1125, (byte)enemiescount, enemies1);
            var items = Items.GetWardSlot();
            if (getresults.Count > 1)
            {
                var getposition = Mathematics.SelectBest(getresults, Player);
                if (Player.Distance(getposition) < 600 && W.IsReady() && items != null)
                {
                    var pos = getposition;
                    foreach (var wards in ObjectManager.Get<Obj_AI_Base>())
                    {
                        if (!_processw2 && W.IsReady() && Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne" &&
                            Player.Spellbook.GetSpell(SpellSlot.Q).Name != "blindmonkwtwo"
                            && ((wards.Name.ToLower().Contains("ward") && wards.IsAlly)))
                        {
                            W.Cast(wards);
                            _lastcasted = Environment.TickCount;
                        }
                    }

                    var ward = Items.GetWardSlot();
                    if (W.IsReady() && ward != null && ward.IsValidSlot() &&
                        Environment.TickCount - _lastward > 400 &&
                        Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne"
                        )
                    {
                        Player.Spellbook.CastSpell(ward.SpellSlot, pos);
                        _lastward = Environment.TickCount;
                    }
                }
            }
            if (enemies1.FirstOrDefault() == null) return;
            if (Environment.TickCount - _lastcasted < 1000)
            {
                R.Cast(enemies1.FirstOrDefault());
            }

        }

        #endregion

        #region On Update

        private static void OnUpdate(EventArgs args)
        {
            //  Game.PrintChat(HasFlash().ToString());
            if (SelectedAllyAiMinion != null)
            {
                if (SelectedAllyAiMinion.IsDead)
                {
                    SelectedAllyAiMinion = null;
                }
            }   
            
            
            if (Player.IsRecalling() || MenuGUI.IsChatOpen) return;

            if (_created)
            { Utility.DelayAction.Add(500, () => _created = false); }


            if (_processw && Environment.TickCount - _lastprocessw > 500)
            {
                Utility.DelayAction.Add(500, () => _processw = false);
            }

            if (_processroncast && Environment.TickCount - _processroncastr > 500)
            {
                Utility.DelayAction.Add(2500, () => _processroncast = false);
            }

            if (_processw2)
            {
                Utility.DelayAction.Add(2500, () => _processw2 = false);
            }

            if (_processr && Environment.TickCount - _lastprocessr > 100)
            {
                Utility.DelayAction.Add(400, () => _processr = false);
            }

            if (_processr2 && Environment.TickCount - _processr2T > 100)
            {
                Utility.DelayAction.Add(400, () => _processr = false);
            }

            if (GetBool("smiteenable", typeof(KeyBind)))
            {
                AutoSmite();
            }
            if (GetBool("wardjump", typeof(KeyBind)))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                WardJump(Player.Position.Extend(Game.CursorPos, 590), true);
            }

            if (GetBool("wardinsec", typeof(KeyBind)))
            {                
                Wardinsec();
            }

            if (GetBool("starcombo", typeof(KeyBind)))
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
                    LaneClear2();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
            }

            AutoUlt();
        }

        #endregion

        #region AutoUlt
        private static void AutoUlt()
        {
            // Hoes code below
            if (GetBool("wardinsec", typeof(KeyBind))) return;

            var target =
                HeroManager.Enemies.Where(x => x.Distance(Player) < R.Range && !x.IsDead && x.IsValidTarget(R.Range))
                    .OrderBy(x => x.Distance(Player)).FirstOrDefault();
            if (target == null || Player.IsDead)
            {
                UltPoly = null;
                _ultPolyExpectedPos = null;
                return;
            }

            UltPoly = new Geometry.Polygon.Rectangle(Player.ServerPosition,
                Player.ServerPosition.Extend(target.Position, 1100),
                target.BoundingRadius + 10);

            var counts =
                HeroManager.Enemies.Where(x => x.Distance(Player) < 1100 && x.IsValidTarget(1100))
                    .Count(h => h.NetworkId != target.NetworkId && UltPoly.IsInside(h.ServerPosition));

            if (counts >= GetValue("autoron") && R.IsReady())
            {
                R.Cast(target);
            }
        }

        #endregion

        #region Jungle Clear

        public static bool spellweave
        {
            get
            {
                return Environment.TickCount - _junglelastq > Game.Ping + 250 &&
                       Environment.TickCount - _junglelastw > Game.Ping + 250
                       && Environment.TickCount - _junglelaste > Game.Ping + 250;
            }
        }

        private static bool CanCast(SpellSlot spellSlot)
        {
            switch (spellSlot)
            {
                case SpellSlot.Q:
                    return !HasPassive() && spellweave && Player.Spellbook.GetSpell(SpellSlot.Q).IsReady()
                        && !W2() && !E2;
                case SpellSlot.W:
                    return !HasPassive() && spellweave && Player.Spellbook.GetSpell(SpellSlot.W).IsReady()
                        && !Q2() && !E2;
                case SpellSlot.E:
                    return !HasPassive() && spellweave && Player.Spellbook.GetSpell(SpellSlot.E).IsReady()
                        && !Q2() && !W2();
            }
            return false;
        }

        private static void JungleClear()
        {
            var jungleminion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            var jungleminions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (jungleminion == null) return;

            var useq = GetBool("useqjl", typeof (bool));
            var usew = GetBool("usewjl", typeof (bool));
            var usee = GetBool("useejl", typeof (bool));
            var usesmart = GetBool("usesjl", typeof (bool));

            if (useq)
            {
                if (usesmart)
                {
                    if (Q1() && CanCast(SpellSlot.Q))
                    {
                        Q.Cast(jungleminion);
                    }
                    if (Q2() && CanCast(SpellSlot.Q))
                    {
                        Q.Cast();
                    }
                }
                else
                {
                    if (Q.IsReady() && Q1())
                    {
                        Q.Cast(jungleminion);
                    }
                    if (Q.IsReady() && Q2())
                    {
                        Q.Cast();
                    }
                }
            }

            if (usew)
            {
                if (usesmart)
                {
                    if (W1() && CanCast(SpellSlot.W) &&Player.Distance(jungleminion) < Player.AttackRange + Player.BoundingRadius)

                    {
                        W.Cast(Player);
                    }
                    if (Player.Distance(jungleminion) < Player.AttackRange + Player.BoundingRadius 
                        && CanCast(SpellSlot.W) && W2())
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
                    if (jungleminion.Distance(Player) <= E.Range && CanCast(SpellSlot.E) && E1)
                    {
                        E.Cast();
                        _lastej = Environment.TickCount;
                    }
                    if (jungleminion.Distance(Player) <= Player.AttackRange + Player.BoundingRadius && CanCast(SpellSlot.E) && E2)
                    {
                        E.Cast();
                    }
                }
            }

            if (jungleminions == null) return;

            if (GetStringValue("hydrati") == 0 || GetStringValue("hydrati") == 2)
            {
                if (jungleminions.Count > 0 && (ItemReady(Tiamat) || ItemReady(Hydra)) && (HasItem(Tiamat) || HasItem(Hydra)))
                {
                    SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                }
            }
        }

        #endregion

        #region Last Hit

        private static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            if (minion.FirstOrDefault() == null) return;
            var min = GetValue("minenergylh");
            if (Player.Mana < min) return;
            var lh = GetBool("useqlh", typeof (bool));
            if (!lh) return;
            foreach (var minions in minion)
            {
                if (minions.Health < Q.GetDamage(minions))
                {
                    Q.Cast(minions);
                }
            }
        }

        #endregion

        #region Lane clear

        private static void LaneClear2()
        {
            if (Player.Mana <= GetValue("minenergyl")) return;
            var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);


            var usee = GetBool("useel", typeof (bool));
            var useeslider = GetValue("useelv");

            if (minion.FirstOrDefault() == null) return;

            if (usee && minion.Count >= useeslider && E.IsReady() && Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne" && Player.GetSpell(SpellSlot.Q).Name != "blindmonkqtwo")
            {
                E.Cast();
                _lastelane = Environment.TickCount;
            }

            if (minion.FirstOrDefault().Distance(Player) < Player.AttackRange + Player.BoundingRadius && (Player.GetSpell(SpellSlot.E).Name == "blindmonketwo" && Environment.TickCount - _lastelane > 2900))
            {
                E.Cast();
            }

            switch (GetStringValue("hydrati"))
            {
                case 1:
                case 2:
                    if (minion.Count > 1 && (ItemReady(Tiamat) || ItemReady(Hydra)) && (HasItem(Tiamat) || HasItem(Hydra)))
                    {
                        SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                    }
                    break;
            }
        }

        private static void LaneClear()
        {
            if (Player.Mana <= GetValue("minenergyl")) return;
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            if (minion.FirstOrDefault() == null) return;

            var useq = GetBool("useql", typeof (bool));

            if (!useq) return;
            foreach (var minions in minion)
            {
                if (Q1() && minions.Health <= Q.GetDamage(minions) && minions.Distance(Player) > 500)
                {
                    Q.Cast(minions);
                }
                if (Q2() && Q.IsReady() && minions.HasBuff("BlindMonkQOne"))
                {
                    Q.Cast();
                    Orbwalking.ResetAutoAttackTimer();
                }
                if (minions.Health <= GetQDamage(minions) && Q.IsReady() && Q1() && minions.Distance(Player) <= 500)
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

        #region Harass

        private static void Harass()
        {
            if (Player.Mana < GetValue("minenergy")) return;

            var useq = GetBool("useqh", typeof (bool));
            var usee = GetBool("useeh", typeof (bool));
            var useq2 = GetBool("useq2h", typeof (bool));
            var delay = GetValue("secondqdelayh");
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
                return;

            if (useq)
            {
                if (Environment.TickCount - _lastqh > 100 && Environment.TickCount - _lasteh > 300)
                {
                    var qpred = Q.GetPrediction(target);
                    if (Q.IsReady() && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne" && (qpred.Hitchance >= HitChance.High || qpred.Hitchance == HitChance.Immobile || qpred.Hitchance == HitChance.Dashing))
                    {
                        Q.Cast(qpred.CastPosition);
                        _lastqh = Environment.TickCount;
                    }

                    if (!useq2) return;

                    if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo" && Q.IsReady() && Environment.TickCount - _lastqc > delay)
                    {
                        Q.Cast();
                        _lastqh = Environment.TickCount;
                    }
                }


                if (usee)
                {
                    if (Environment.TickCount - _lastqh > 300 && Environment.TickCount - _lasteh > 300)
                    {
                        if (target.Distance(Player) <= E.Range && Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne")
                        {
                            E.Cast();
                            _lasteh = Environment.TickCount;
                        }
                        if ((Player.Distance(target) > Player.AttackRange + Player.BoundingRadius + target.BoundingRadius + 100 || Environment.TickCount - _laste > 2700) && Player.GetSpell(SpellSlot.E).Name == "blindmonketwo")
                        {
                            E.Cast();
                            _lasteh = Environment.TickCount;
                        }
                    }
                }
            }
        }

        #endregion

        #region Combo

        private static void Combo()
        {
            #region R combos

            var unit = HeroManager.Enemies.Where(x => x.Distance(Player) < 500 && !x.IsDead && x.IsValidTarget(500) && x.Health < R.GetDamage(x) + 50).OrderBy(x => x.Distance(Player)).FirstOrDefault();
            if (unit != null)
            {
                foreach (var targets in
                    HeroManager.Enemies.Where(x => !x.IsDead && x.IsValidTarget() && x.IsVisible && x.Distance(unit) < 1000 && x.Distance(unit) > 300 && x.NetworkId != unit.NetworkId && x.Health < R.GetDamage(x)))
                {
                    var prediction = Prediction.GetPrediction(targets, 0.1f);

                    var pos = prediction.UnitPosition.Extend(unit.ServerPosition, prediction.UnitPosition.Distance(unit.ServerPosition) + 250);

                    _rCombo = pos;

                    var slot = Items.GetWardSlot();
                    if (unit.Distance(Player) > 500)
                    {
                        _rCombo = null;
                    }

                    if (W.IsReady() && R.IsReady() && Player.ServerPosition.Distance(unit.ServerPosition) < 500 && slot != null)
                    {
                        if (!_processw && Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
                        {
                            Player.Spellbook.CastSpell(slot.SpellSlot, pos);
                            _lastwarr = Environment.TickCount;
                        }
                        if (Player.GetSpell(SpellSlot.W).Name == "blindmonkwtwo")
                        {
                            _lastwards = Environment.TickCount;
                        }
                    }
                }

                if (Player.IsDead)
                {
                    UltPoly = null;
                    _ultPolyExpectedPos = null;
                    return;
                }

                UltPoly = new Geometry.Polygon.Rectangle(Player.ServerPosition, Player.ServerPosition.Extend(unit.Position, 1100), unit.BoundingRadius + 30);

                var counts = HeroManager.Enemies.Where(x => x.Distance(Player) < 1100 && x.IsValidTarget(1100) && x.Health < R.GetDamage(x)).Count(h => h.NetworkId != unit.NetworkId && UltPoly.IsInside(h.ServerPosition));

                if (counts >= 1 && R.IsReady() && _created && R.IsReady())
                {
                    R.Cast(unit);
                }
            }

            #endregion

            #region Regular combo

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
                return;

            var useq = GetBool("useq", typeof (bool));
            var usee = GetBool("usee", typeof (bool));
            var user = GetBool("user", typeof (bool));
            var usew = GetBool("wardjumpcombo", typeof (bool));
            var smite = GetBool("usessmite", typeof (bool));
            if (GetStringValue("hydrati") == 0 || GetStringValue("hydrati") == 2)
            {
                if (target.IsValidTarget(400) && (ItemReady(Tiamat) || ItemReady(Hydra)) && (HasItem(Tiamat) || HasItem(Hydra)))
                {
                    SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                }
            }

            if (GetBool("youm", typeof (bool)) && HasItem(Youm) && ItemReady(Youm) && target.Distance(Player) < Q.Range - 300)
            {
                SelfCast(Youm);
            }

            if (GetBool("omen", typeof (bool)) && HasItem(Omen) && ItemReady(Omen) && Player.CountAlliesInRange(400) >= GetValue("minrand"))
            {
                SelfCast(Omen);
            }
            if (usew)
            {
                if (Environment.TickCount - _lastqc > 300 && Environment.TickCount - _laste > 300 && Environment.TickCount - _lastwcombo > 300)
                {
                    if (W.IsReady() && target.Distance(Player) <= Player.AttackRange && W1())
                    {
                        W.Cast(Player);
                        _lastwcombo = Environment.TickCount;
                    }

                    if (W.IsReady() && target.Distance(Player) <= Player.AttackRange && W2() && !HasPassive())
                    {
                        W.Cast();
                    }
                }
            }

            if (useq)
            {
                if (Environment.TickCount - _lastqc > 300 && Environment.TickCount - _laste > 300 && Environment.TickCount - _lastwcombo > 300)
                {
                    var qpred = Q.GetPrediction(target);
                    if (Q.IsReady() && !qpred.CollisionObjects.Any() && Q1() && (qpred.Hitchance >= HitChance.High || qpred.Hitchance == HitChance.Immobile))
                    {
                        Q.Cast(qpred.CastPosition);
                        _lastqc = Environment.TickCount;
                    }

                    if (Q2() && Q.IsReady() && GetBool("useq2", typeof (bool)))
                    {
                        Utility.DelayAction.Add(GetValue("secondqdelay"), () => Q.Cast());
                        _lastqc = Environment.TickCount;
                    }
                }
            }

            if (usee)
            {
                if (Environment.TickCount - _lastqc > 300 && Environment.TickCount - _laste > 300 && Environment.TickCount - _lastwcombo > 300)
                {
                    if (target.Distance(Player) <= E.Range && E1)
                    {
                        E.Cast();
                        _laste = Environment.TickCount;
                    }
                    if ((Player.Distance(target) > Player.AttackRange + Player.BoundingRadius + target.BoundingRadius + 100 || Environment.TickCount - _laste > 2300) && E2)
                    {
                        E.Cast();
                        _laste = Environment.TickCount;
                    }
                }
            }

            if (user && target.IsValidTarget(R.Range) && R.IsReady())
            {
                if (Q.IsReady() && target.Health <= R.GetDamage(target) + GetQDamage(target) + Player.GetAutoAttackDamage(target) && Q.IsReady() && target.Health > GetQDamage(target))
                {
                    R.Cast(target);
                }

                if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) && Q.IsReady() && Player.Mana > 30)
                {
                    R.Cast(target);
                }
            }

            if (Smite.IsReady() && target.Distance(Player) < 500 && smite && target.Health < GetFuckingSmiteDamage())
            {
                Player.Spellbook.CastSpell(Smite, target);
            }

            var poss = Player.ServerPosition.Extend(target.ServerPosition, 600);
            if (!GetBool("wardjumpcombo1", typeof (bool))) return;

            if (!E.IsReady() || !W.IsReady() || !(target.Distance(Player) > E.Range)) return;
            if (!Q.IsReady() && Environment.TickCount - Q.LastCastAttemptT > 1000)
            {
                WardJump(poss, false);
            }

            #endregion
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

            var slot = Items.GetWardSlot();
            var qpred = Q.GetPrediction(target);
            if (Q.IsReady() && Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne")
            {
                Q.Cast(qpred.CastPosition);
            }

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo" && target.IsValidTarget(R.Range) && Q.IsReady())
            {
                R.Cast(target);
                Steps = steps.Flash;
            }

            if (Player.Spellbook.GetSpell(SpellSlot.Q).Name.ToLower() == "blindmonkqtwo" && Q.IsReady() && !R.IsReady())
            {
                Utility.DelayAction.Add(300, () => Q.Cast());
            }

            if (R.IsReady() && Q.IsReady() && W.IsReady() && slot != null)
            {
                if (target.Distance(Player) > R.Range && target.Distance(Player) < R.Range + 580)
                {
                    var pos = target.ServerPosition.Extend(Player.ServerPosition, 200);
                    if (!_processw && Player.GetSpell(SpellSlot.W).Name == "BlindMonkWOne")
                    {
                        Player.Spellbook.CastSpell(slot.SpellSlot, pos);
                        _lastwarr = Environment.TickCount;
                    }
                    if (Player.GetSpell(SpellSlot.W).Name == "blindmonkwtwo")
                    {
                        _lastwards = Environment.TickCount;
                        _lastflashward = Environment.TickCount;
                    }
                }
            }

            if (E.IsReady() && E.Instance.Name == "BlindMonkEOne" && target.IsValidTarget(E.Range))
            {
                E.Cast();
            }

            if (E.IsReady() && E.Instance.Name != "BlindMonkEOne" && !target.IsValidTarget(Orbwalking.GetRealAutoAttackRange(Player)))
            {
                E.Cast();
            }

            #region zzz

            #endregion
        }

        #endregion

        #region States

        public static bool Q1()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne";
        }

        public static bool Q2()
        {
            return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo";
        }

        public static bool W1()
        {
            return Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne";
        }

        public static bool W2()
        {
            return Player.Spellbook.GetSpell(SpellSlot.W).Name == "blindmonkwtwo";
        }

        public static bool E1
        {
            get { return Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne"; }
        }

        public static bool E2
        {
            get { return Player.GetSpell(SpellSlot.E).Name == "blindmonketwo"; }
        }

        public static bool HasFlash()
        {
            return Player.GetSpellSlot("summonerflash").IsReady();
        }

        #endregion

        #region Ward Insec

        private static void Wardinsec()
        {
            #region Target, Slots, Prediction

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(Q.Range + 800, TargetSelector.DamageType.Physical);
            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
            }


            if (target == null) return;

            var qpred = Q.GetPrediction(target);

            var col = qpred.CollisionObjects;

            var slot = Items.GetWardSlot();

            #endregion

            if (Player.Distance(target) > 500)
            {
                if (Q2() && Q.IsReady())
                {
                    Utility.DelayAction.Add(400, () => Q.Cast());
                }
            }

            if (Q1() && Player.Distance(target) <= Q.Range && col.Count == 0)
            {
                Q.Cast(qpred.CastPosition);
            }


            var poss = Insec(target, GetValue("fixedwardrange"), true);

            foreach (var min in
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAllyForEnemy)
                    .Where(
                        x =>
                            x != null &&
                            (x.Distance(target) < 420 || (x.Distance(poss) < 600 ||
                             (canwardflash && x.Distance(target) < 800)) && x.Health > Q.GetDamage(x) + 5 && !x.IsDead &&
                            Q.GetPrediction(x).CollisionObjects.Count == 0 && x.Distance(Player) < Q.Range)))
            {
                minionss = (Obj_AI_Base) min;
                Render.Circle.DrawCircle(min.Position, 80, Color.Yellow, 5, true);
                if (Q1() && Q.IsReady())
                {
                    Q.Cast(min);
                }

                if (Q2() && min.HasBuff("blindmonkqtwo"))
                {
                    Q.Cast();
                }
            }

            if ((Steps == steps.WardJump || Environment.TickCount - _lastwardjump < 1500) && slot != null && W.IsReady() && R.IsReady())
            {
                canwardflash = false;
                if (target.Position.Distance(Player.Position) < 600)
                {
                    WardJump(poss.To3D(), false, false);
                }
            }
            else
            {
                canwardflash = true;
            }

            if (target.HasBuff("blindmonkqtwo") || Environment.TickCount - _lastqcasted1 < 250)
            {
                canwardflash = false;
            }
            else
            {
                canwardflash = true;
            }

            if (Environment.TickCount - _lastprocessw < 1500 || Steps == steps.Flash)
            {
                if (R.IsReady())
                    R.Cast(target);
            }

            #region Determine if we want to flash or ward jump

            if (R.IsReady())
            {
                if (slot != null && W.IsReady())
                {
                    if (GetBool("prioflash", typeof (bool)) && Player.GetSpellSlot("summonerflash").IsReady())
                    {
                        Steps = steps.Flash;
                    }
                    else
                    {
                        Steps = steps.WardJump;
                        _lastwardjump = Environment.TickCount;
                    }
                }
                else if (GetBool("useflash", typeof (bool)) && target.Distance(Player) < 400&& Player.GetSpellSlot("SummonerFlash").IsReady() && (slot == null || !W.IsReady() || W2()) &&  Environment.TickCount - _lastwcasted > 1000)
                {
                    Steps = steps.Flash;
                }
            }

            var wardtotargetpos = Player.Position.Extend(target.Position, Player.Distance(target) - 250);
            var wardFlashBool = GetBool("expwardflash", typeof (bool));

            if (Player.ServerPosition.Distance(target.ServerPosition) < 250 || target.Distance(Player) > 1000
                || !canwardflash || !CanWardFlash(target))
                return;

            //if ((Environment.TickCount - _lastqcasted1 <= 250) &&
            //    (col.Count <= 0))
            //    return;


            if (Environment.TickCount -_lastwcasted > 1000 &&
                ((Player.Position.Distance(target.Position) > 300 ||
                 (minionss != null))))
            {
                WardJump(wardtotargetpos, false, false);
                
                _wardjumpedto = Environment.TickCount;
                _wardjumpedtotarget = true;
                _lastflashward = Environment.TickCount;
            }

            #endregion

            #region Q Smite

            //var prediction = Prediction.GetPrediction(target, Q.Delay);

            //var collision = Q.GetCollision(Player.Position.To2D(),
            //    new List<Vector2> { prediction.UnitPosition.To2D() });

            //foreach (var collisions in collision)
            //{
            //    if (collision.Count == 1 && collision[0].IsMinion)
            //    { 
            //        if (!GetBool("UseSmite", typeof(bool))) return;
            //        if (Q.IsReady())
            //        {   
            //            if (collision[0].Distance(Player) < 500)
            //            {
            //                if (collision[0].Health <= GetFuckingSmiteDamage() && Smite.IsReady())
            //                {
            //                    Q.Cast(prediction.CastPosition);
            //                    Player.Spellbook.CastSpell(Smite, collision[0]);
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion
        }

        public static Obj_AI_Base minionss { get; set; }

        public static bool CanWardFlash(Obj_AI_Hero target)
        {
            var wardFlashBool = GetBool("expwardflash", typeof(bool));
            var slot = Items.GetWardSlot();

            return slot != null && HasFlash() && W.IsReady() &&
                   R.IsReady() && wardFlashBool ;
        }

        public static
            bool Colbool { get; set; }

        #endregion

        #region Ward Jump

        public static void FlashCast(Vector2 position)
        {
            if (!_processroncast) return;
            Player.Spellbook.CastSpell(Player.GetSpellSlot("summonerflash"), position.To3D());
        }

        public static void WardJump(Vector3 position, bool objectuse, bool use = true)
        {
            var objectss =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        x =>
                            x.IsValid && x.Distance(position) < 200 && x.IsAlly && !x.IsDead &&
                            x.Name.ToLower().Contains("ward"));

            var ward = Items.GetWardSlot();
            if (W.IsReady() && ward != null && Environment.TickCount - _lastwcasted > 400 && W1())
            {
                {
                    Player.Spellbook.CastSpell(ward.SpellSlot, position);
                }
            }

            var objects =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        x =>
                            x.IsValid && x.Distance(position) < 200 && x.IsAlly && !x.IsDead &&
                            !x.Name.ToLower().Contains("turret"));
            if (!objectuse) return;
            foreach (
                var wards in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(wards => W.IsReady() && W1() && !W2() && (objects != null)))
            {
                W.Cast(objects);
            }
        }

        #endregion

        #region Everything Jungle Based

        public static readonly string[] Names =
        {
            "Krug", "Razorbeak", "Murkwolf", "Gromp", "Crab", "Blue", "Red", "Dragon", "Baron"
        };

        private static int _lastwards;
        private static int _lastprocessw;
        private static int _lastwardjump;
        private static bool _processr;
        private static int _lastprocessr;
        private static bool _processr2;
        private static int _clickCount;
        private static bool _lastClickBool;
        private static Vector3 _lastClickPos;
        private static bool _b;
        private static bool _processw2;
        private static int _lastwcombo;
        private static int _lastwarr;
        private static int _processr2T;
        private static int _lastqh;
        private static int _lasteh;
        private static bool _processroncast;
        private static int _processroncastr;
        private static Geometry.Polygon.Rectangle _ultPolyExpectedPos;
        private static bool _created;
        private static Vector3? _rCombo;
        private static StringFormat _stringf;
        private static int _lastcasted;
        private static int _lastwcasted;
        private static bool _wardjumpedtotarget;
        private static int _lastqcasted;
        private static int _lastflashward;
        private static int _wardjumpedto;
        private static int _junglelastq;
        private static int _junglelastw;
        private static int _junglelaste;
        private static int lastflashed;
        private static bool canwardflash;
        private static int _lastqcasted1;
        private static Obj_AI_Base minions;

        private static void AutoSmite()
        {
            if (!GetBool("smiteonkillable", typeof (bool))) return;

            foreach (var mob in
                MinionManager.GetMinions(Player.Position, 550, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth))
            {
                foreach (var name in Names)
                {
                    if (mob.CharData.BaseSkinName == "SRU_" + name && GetBool("usesmiteon" + name, typeof (bool)))
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

                            if (GetFuckingSmiteDamage() + Q.GetDamage(mob) + (mob.MaxHealth - mob.Health)*0.08 >= mob.Health && Q.IsReady() && Player.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo" && mob.HasBuff("blindmonkqtwo"))
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
                370 + 20*level, 330 + 30*level, 240 + 40*level, 100 + 50*level
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
                Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X - 20, Drawing.WorldToScreen(Player.Position).Y - 20, color, text);
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
                                Drawing.DrawText(minion.HPBarPosition.X + xOffset, minion.HPBarPosition.Y, Color.Red, "Killable");
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Drawings

        private static void OnSpells(EventArgs args)
        {
            if (Player.IsDead) return;

            if (UltPoly != null && GetBool("rpolygon", typeof (bool)))
            {
                UltPoly.Draw(Color.Red);
            }

            if (_rCombo != null && GetBool("rpolygon", typeof (bool))) Render.Circle.DrawCircle((Vector3) _rCombo, 100, Color.Red, 5, true);

            if (GetBool("counthitr", typeof (bool)))
            {
                var getresults = Mathematics.GetPositions(Player, 1125, (byte) 3, HeroManager.Enemies);
                if (getresults.Count > 1)
                {
                    var getposition = Mathematics.SelectBest(getresults, Player);
                    Render.Circle.DrawCircle(getposition, 100, Color.Red, 3, true);
                }
            }


            if (!GetBool("spellsdraw", typeof (bool))) return;
            if (!GetBool("ovdrawings", typeof (bool))) return;
            if (GetBool("qrange", typeof (bool)) && Q.Level > 0)
            {
                var color = Q.IsReady() ? Color.DodgerBlue : Color.Red;
                Render.Circle.DrawCircle(Player.Position, Q.Range, color, 2);
            }

            if (GetBool("wrange", typeof (bool)) && W.Level > 0)
            {
                var colorw = W.IsReady() ? Color.BlueViolet : Color.Red;
                Render.Circle.DrawCircle(Player.Position, W.Range, colorw, 2);
            }

            if (GetBool("erange", typeof (bool)) && E.Level > 0)
            {
                var colore = E.IsReady() ? Color.Plum : Color.Red;
                Render.Circle.DrawCircle(Player.Position, E.Range, colore, 2, true);
            }

            if (GetBool("rrange", typeof (bool)) && R.Level > 0)
            {
                var colorr = R.IsReady() ? Color.LawnGreen : Color.Red;
                Render.Circle.DrawCircle(Player.Position, R.Range, colorr, 2, true);
            }
            var target = HeroManager.Enemies.Where(x => x.Distance(Player) < R.Range && !x.IsDead && x.IsValidTarget(R.Range)).OrderBy(x => x.Distance(Player)).FirstOrDefault();
            if (target == null || Player.IsDead)
            {
                UltPoly = null;
                _ultPolyExpectedPos = null;
                return;
            }

            UltPoly = new Geometry.Polygon.Rectangle(Player.ServerPosition, Player.ServerPosition.Extend(target.Position, 1100), target.BoundingRadius + 20);
            if (GetBool("counthitr", typeof (bool)))
            {
                var counts = HeroManager.Enemies.Where(x => x.Distance(Player) < 1200 && x.IsValidTarget(1200)).Count(h => h.NetworkId != target.NetworkId && UltPoly.IsInside(h.ServerPosition));

                Drawing.DrawText(Drawing.WorldToScreen(Player.Position).X - 50, Drawing.WorldToScreen(Player.Position).Y + 30, Color.Magenta, "Ult Will Hit " + counts);
            }
        }

        #region Range draws

        private static void OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;
            if (!GetBool("spellsdraw", typeof (bool))) return;
            if (!GetBool("targetexpos", typeof (bool))) return;
            if (!GetBool("ovdrawings", typeof (bool))) return;

            if (SelectedAllyAiMinion != null)
            {
                Render.Circle.DrawCircle(SelectedAllyAiMinion.Position, 200, Color.Blue, 2, true);
            }
            if (SelectedAllyAiMinionv != new Vector3() && GetBool("clickto", typeof (bool)))
            {
                Render.Circle.DrawCircle(SelectedAllyAiMinionv, 200, Color.Blue, 2, true);
            }


            var target = TargetSelector.GetTarget(2000, TargetSelector.DamageType.Physical);

            if (target != null)
            {
                target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
            }


            if (target == null || target.IsDead || !target.IsVisible) return;


            var pos = Insec(target, 260, false);
            if (GetBool("wardpositionshow", typeof (bool)))
            {
                Render.Circle.DrawCircle(pos.To3D(), 100, Color.Yellow, 1, true);
                var f = Drawing.WorldToScreen(pos.To3D()).X - 20;
                var text2 = Drawing.WorldToScreen(pos.To3D()).Y;
                const string texts = "Insec Position";
                Drawing.DrawText(f, text2, Color.Red, texts);
            }
            //            color = new ColorBGRA(100, 100, 100, 100);
            //            text = new Render.Text(pos, "Ward Here", 3, color);


            if (!GetBool("linebetween", typeof (bool))) return;
            var objAiHero = GetAllyHeroes(target, 1200).FirstOrDefault();
            if (SelectedAllyAiMinion == null && SelectedAllyAiMinionv == new Vector3())
            {
                if (objAiHero != null && GetBool("useobjectsallies", typeof (bool)))
                {
                    var pos11 = Drawing.WorldToScreen(target.Position);
                    var distance = target.Distance(objAiHero);
                    var pos22 = Drawing.WorldToScreen(target.Position.Extend(objAiHero.Position, distance));
                    Drawing.DrawLine(pos11, pos22, 1, Color.Red);
                    Render.Circle.DrawCircle(objAiHero.Position, 100, Color.Blue, 2, true);
                    Drawing.DrawText(pos22.X, pos22.Y, Color.Black, "X");
                }
                else
                {
                    var pos11 = Drawing.WorldToScreen(target.Position);
                    var distance = target.Distance(Player);
                    var pos22 = Drawing.WorldToScreen(target.Position.Extend(Player.Position, distance));
                    Drawing.DrawLine(pos11, pos22, 1, Color.Red);
                    Render.Circle.DrawCircle(Player.Position, 100, Color.Blue, 2, true);
                    Drawing.DrawText(pos22.X, pos22.Y, Color.Black, "X");
                }
            }

            if (SelectedAllyAiMinion != null)
            {
                var pos3 = Drawing.WorldToScreen(target.Position);
                var distance = target.Distance(SelectedAllyAiMinion);
                var pos4 = Drawing.WorldToScreen(target.Position.Extend(SelectedAllyAiMinion.Position, distance));
                Drawing.DrawLine(pos3, pos4, 3, Color.Red);
                Drawing.DrawText(pos4.X, pos4.Y, Color.Black, "X");
            }
        }

        #endregion

        #endregion

        public static void UpdateCheck()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var c = new WebClient())
                    {
                        var rawVersion = c.DownloadString("https://raw.githubusercontent.com/HoesLeaguesharp/LeagueSharp/master/Lee%20Sin/Lee%20Sin/Properties/AssemblyInfo.cs");
                        var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(rawVersion);

                        if (match.Success)
                        {
                            var gitVersion = new Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));

                            if (gitVersion != typeof (Program).Assembly.GetName().Version)
                            {
                                Game.PrintChat("<font color='#15C3AC'>Support:</font> <font color='#FF0000'>" + "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                                Game.PrintChat("<font color='#15C3AC'>Support:</font> <font color='#FF0000'>" + "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                            }
                            else
                            {
                                Game.PrintChat("<font color='#15C3AC'>Slutty Lee Sin:</font> <font color='#40FF00'>" + "UPDATED - Version: " + gitVersion + "</font>");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }

        public static Vector3 Playerpos { get; set; }

        public static Geometry.Polygon.Rectangle UltPoly { get; set; }

        public static Vector3 RCombos { get; set; }

        public static Render.Text Text { get; set; }

        public static ColorBGRA color { get; set; }
    }
}