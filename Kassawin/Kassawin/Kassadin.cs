using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;


namespace Kassawin
{
    internal class Kassadin : Helper
    {
        private static SpellSlot _ignite;
        public static Spell Q, W, E, R;
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");
        private static readonly Color _color = Color.Red;
        private static readonly Color FillColor = Color.Blue;
        public static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Kassadin") return;
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 200);
            E = new Spell(SpellSlot.E, 400);
            R = new Spell(SpellSlot.R, 700);
            DamageToUnit = GetComboDamage;
            E.SetSkillshot(0.25f, 20, int.MaxValue, false, SkillshotType.SkillshotCone);
            R.SetSkillshot(0.25f, 270, int.MaxValue, false, SkillshotType.SkillshotCircle);

            Printmsg("Kassadin Assembly Loaded");
            Printmsg1("Current Version: " + typeof(Program).Assembly.GetName().Version);
            Printmsg2("Don't Forget To " + "<font color='#00ff00'>[Upvote]</font> <font color='#FFFFFF'>" + "The Assembly In The Databse" + "</font>");

            MenuConfig.OnLoad();
            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnDoCast += OnDoCast;
            Obj_AI_Base.OnDoCast += OnDoCasts;
            Interrupter2.OnInterruptableTarget += OnInteruppt;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "RiftWalk" || args.SData.Name == "ForcePulse" || args.SData.Name == "NetherBlade" || args.SData.Name == "NullSphere")
                {
                    dontAtt = true;
                }
                else
                    dontAtt = false;
            }

        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (args.Slot == SpellSlot.R || args.Slot == SpellSlot.E || args.Slot == SpellSlot.Q || args.Slot == SpellSlot.W)
            {
                dontAtt = true;
                dontatttimer = Environment.TickCount;
            }
        }

        private static void Printmsg(string message)
        {
            Game.PrintChat(
                "<font color='#6f00ff'>[Slutty Kassadin]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg1(string message)
        {
            Game.PrintChat(
                "<font color='#ff00ff'>[Slutty Kassadin]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg2(string message)
        {
            Game.PrintChat(
                "<font color='#00abff'>[Slutty Kassadin]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static float GetComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0d;

            if (Q.IsReady())
                damage += Q.GetDamage(enemy);

            if (W.IsReady())
                damage += W.GetDamage(enemy);

            if (E.IsReady() && eCanCast())
                damage += W.GetDamage(enemy);

            if (R.IsReady())
                damage += R.GetDamage(enemy);

            if (Player.GetSpellSlot("summonerdot").IsReady())
                damage += IgniteDamage(enemy);

            return (float)damage;
        }

        public static int DangerLevel(Interrupter2.InterruptableTargetEventArgs args)
        {
            switch (args.DangerLevel)
            {
                case Interrupter2.DangerLevel.Low:
                    return 1;

                case Interrupter2.DangerLevel.Medium:
                    return 2;

                case Interrupter2.DangerLevel.High:
                    return 3;
            }
            return 0;
        }

        private static void OnInteruppt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var use = GetBool("useqint", typeof (bool));
            var dangerlevel = GetValue("interruptlevel");
            if (!use) return;

            if (!sender.IsChampion() || !sender.IsEnemy) return;
            if (!(sender.Distance(Player) < Q.Range)) return;
            if (DangerLevel(args) < dangerlevel) return;

            if (Q.IsReady())
                Q.Cast(sender);
        }

        private static void OnDraw(EventArgs args)
        {
            var draw = GetBool("enabledraw", typeof (bool));
            if (!draw) return;

            var qdraw = GetBool("drawq", typeof (bool));
            var edraw = GetBool("drawe", typeof (bool));
            var rdraw = GetBool("drawr", typeof (bool));
            var drawcount = GetBool("drawcount", typeof (bool));
            var mindraw = GetBool("drawqkill", typeof (bool));

            if (qdraw)
            {
                if (Q.Level >= 1)
                    Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Purple);
            }

            if (edraw)
            {
                if (E.Level >= 1)
                    Render.Circle.DrawCircle(Player.Position, E.Range, Color.MidnightBlue);
            }

            if (rdraw)
            {
                if (R.Level >= 1)
                    Render.Circle.DrawCircle(Player.Position, R.Range, Color.Crimson);
            }
            if (drawcount)
            {
                var pos = Drawing.WorldToScreen(Player.Position);
                Drawing.DrawText(pos.X, pos.Y, Color.Red, "[R] Stack");
                Drawing.DrawText(pos.X + 70, pos.Y, Color.Purple, ForcePulseCount().ToString());
            }

            if (!mindraw) return;

            foreach (
                var min in
                    MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy)
                        .Where(x => x.Health < Q.GetDamage(x)))
            {
                Render.Circle.DrawCircle(min.Position, 80, Color.Red, 5, true);
            }
        }

        private static void OnDoCasts(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (!sender.IsMe) return;
            if (!args.SData.IsAutoAttack()) return;
            if (args.Target.Type != GameObjectType.obj_AI_Minion) return;
            var usew = GetBool("usewl", typeof (bool));
            if (!usew) return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                var minions =
                    MinionManager.GetMinions(Player.Position, 300);
                
                if (W.IsReady())
                {
                    if(((Obj_AI_Base) args.Target).Health > Player.GetAutoAttackDamage((Obj_AI_Base) args.Target) + 50)                      
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                        Orbwalker.ForceTarget((Obj_AI_Base) args.Target);
                    }
                    foreach (var min in minions.Where(
                            x => x.NetworkId != ((Obj_AI_Base)args.Target).NetworkId))
                    {
                        if (((Obj_AI_Base) args.Target).Health > Player.GetAutoAttackDamage((Obj_AI_Base) args.Target))
                        {
                            W.Cast();
                            Orbwalking.ResetAutoAttackTimer();
                            Orbwalker.ForceTarget(min);
                        }
                    }
                }
            }   
        }

        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!Orbwalking.IsAutoAttack(args.SData.Name)) return;
            if (!sender.IsMe) return;
            if (args.Target.Type != GameObjectType.obj_AI_Hero) return;
            var target = (Obj_AI_Hero) args.Target;

            var usew = GetBool("usew", typeof (bool));
            if (Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo) return;

            if (args.SData.IsAutoAttack())
            {
                if (target == null) return;
                {
                    if (W.IsReady() && usew)
                    {
                        W.Cast();
                        Orbwalking.ResetAutoAttackTimer();
                        Orbwalker.ForceTarget(target);
                    }
                }
            }
        }

        private static bool eCanCast()
        {
            return Player.HasBuff("forcepulsecancast");
        }


        public static int GetPassiveBuff
        {
            get
            {
                var data = Player.Buffs.FirstOrDefault(b => b.Name.ToLower() == "forcepulsecounter");
                if (data != null)
                {
                    return data.Count == -1 ? 0 : data.Count == 0 ? 1 : data.Count;
                }
                return 0;
            }
        }

        public static int ForcePulseCount()
        {
            var manacost = Player.Spellbook.GetSpell(SpellSlot.R).ManaCost;
            switch (manacost.ToString(CultureInfo.InvariantCulture))
            {
                case "50":
                    return 0;
                case "100":
                    return 1;
                case "200":
                    return 2;
                case "400":
                    return 4;
                case "800":
                    return 5;
            }
            return 0;
        }


        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    JungleClear();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:
                    LastHit();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
            }

            if (GetBool("fleemode", typeof (KeyBind)))
            {
                fleeMode();
            }

            Killsteal();

        }

        private static void JungleClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (minions.FirstOrDefault() == null) return;

            var useq = GetBool("useqj", typeof (bool));
            var usee = GetBool("useej", typeof (bool));
            var user = GetBool("userj", typeof (bool));
            var mana = GetValue("minmanajungleclear");
            var rslider = GetValue("rcountj");
            if (Player.ManaPercent < mana) return;
            if (Player.IsWindingUp) return;

            if (useq)
            {
                if (Player.Distance(minions[0]) < Orbwalking.GetRealAutoAttackRange(minions[0]) && !W.IsReady())
                {
                    Q.Cast(minions[0]);
                }
                else
                {
                    Q.Cast(minions[0]);
                }
            }

            if (usee && eCanCast())
            {
                if (E.IsReady())
                {
                    E.Cast(minions[0]);
                }
            }

            if (user)
            {
                if (ForcePulseCount() >= rslider) return;
                if (R.IsReady())
                {
                    R.Cast(minions[0]);
                }
            }
        }

        private static void LastHit()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (minions.FirstOrDefault() == null) return;
            var useq = GetBool("useqlh", typeof (bool));
            var mana = GetValue("minmanalasthit");
            if (Player.ManaPercent < mana) return;

            if (useq)
            {
                if (minions.FirstOrDefault() == null) return;
                if (minions[0].Health >= Q.GetDamage(minions[0])) return;

                if (Player.Distance(minions[0]) < Orbwalking.GetRealAutoAttackRange(minions[0]) && !W.IsReady())
                {
                    Q.Cast(minions[0]);
                }
                else
                {
                    Q.Cast(minions[0]);
                }
            }

        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            var useq = GetBool("useql", typeof (bool));
            if (useq)
            {
                foreach (var mins in minions)
                {
                    if (mins.Health <= Q.GetDamage(mins))
                    {
                        if (mins.Health >= Player.GetAutoAttackDamage(mins) + 50 && mins.Distance(Player) <= Orbwalking.GetRealAutoAttackRange(mins))
                        {
                            Q.Cast(mins);
                        }

                        if (mins.Distance(Player) >= Orbwalking.GetRealAutoAttackRange(mins) + 100)
                        {
                            Q.Cast(mins);
                        }
                    }

                }
            }
            if (minions.FirstOrDefault() == null) return;


            if (Player.ManaPercent < GetValue("minmanalaneclear")) return;

            var usee = GetBool("useel", typeof (bool));
            var user = GetBool("userl", typeof (bool));
            var useeslider = GetValue("useels");
            var userslider = GetValue("userls");
            var count = GetValue("rcountl");

            if (usee)
            {
                if (E.IsReady() && eCanCast())
                {
                    // if (Player.Distance(minions) < Orbwalking.GetRealAutoAttackRange(minions) && !W.IsReady())
                    //    E.CastIfWillHit(minions, useeslider);
                    //else if (Player.Distance(minions) > Orbwalking.GetRealAutoAttackRange(minions))
                    //{
                    var miniosn =
                        MinionManager.GetMinions(Player.Position, 400);
                    {
                        if (miniosn.FirstOrDefault() != null)
                        {
                            var predict = E.GetCircularFarmLocation(miniosn, 500);
                            var minhit = predict.MinionsHit;
                            if (minhit >= useeslider)
                            {
                                E.Cast(predict.Position);
                            }
                        }
                    }
                }
            }

            if (user)
            {
                if (ForcePulseCount() >= count) return;
                if (R.IsReady())
                {

                    var min =
                        MinionManager.GetMinions(Player.Position, R.Range);
                    if (min.FirstOrDefault() != null)
                    {
                        var prediction = R.GetCircularFarmLocation(min, R.Width);
                        var predict = prediction.MinionsHit;
                        if (predict >= userslider)
                        {
                            R.Cast(prediction.Position);
                        }

                    }
                }
            }




        }

        public static Geometry.Polygon SafeZone { get; set; }

        public static Geometry.Polygon.Arc newCone { get; set; }

        private static void Killsteal()
        {
            var target = TargetSelector.GetTarget(Q.Range + 500, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget(Q.Range + 500)) return;
            var extendedposition = Player.Position.Extend(target.ServerPosition, 500);
            var ks = GetBool("ks", typeof (bool));
            if (!ks) return;
            var qks = GetBool("qks", typeof (bool));
            var rks = GetBool("rks", typeof (bool));
            var eks = GetBool("eks", typeof (bool));
            var rgks = GetBool("rgks", typeof (bool));
            if (target.Distance(Player) > Q.Range - 20 && rgks)
            {
                if ((target.Health < Q.GetDamage(target) && Q.IsReady()) ||
                    (target.Health < E.GetDamage(target) && E.IsReady()))
                    R.Cast(extendedposition);
            }

            if (target.Health < Q.GetDamage(target) && target.IsValidTarget(Q.Range))
            {
                if (qks)
                    Q.Cast(target);
            }

            if (target.Health < E.GetDamage(target) && eCanCast() && target.Distance(Player) < 500)
            {
                if (eks)
                    E.Cast(target.Position);
            }

            if (target.Health < R.GetDamage(target) && R.IsReady() && target.IsValidTarget(700))
            {
                if (rks)
                    R.Cast(extendedposition);
            }
        }

        private static void fleeMode()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            //    var getrrcount = GetValue("fleercoutn");
            var extendedposition = Player.Position.Extend(Game.CursorPos, 500);

            if (R.IsReady())
            {
                R.Cast(extendedposition);
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null) return;

            var mana = GetValue("harassmana");
            if (Player.ManaPercent < mana) return;
            var useq = GetBool("useqharass", typeof (bool));
            var usee = GetBool("useeharass", typeof (bool));

            if (useq)
            {
                if (Q.IsReady())
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && eCanCast() && target.Distance(Player) < 500)
            {
                if (usee)
                    E.Cast(target);
            }

        }

        public static SpellSlot GetIgniteSlot()
        {
            return _ignite;
        }

        public static void SetIgniteSlot(SpellSlot nSpellSlot)
        {
            _ignite = nSpellSlot;
        }

        public static float IgniteDamage(Obj_AI_Hero target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range + 200, TargetSelector.DamageType.Magical);
            if (target == null) return;
            var useq = GetBool("useq", typeof (bool));
            var user = GetBool("user", typeof (bool));
            var usee = GetBool("usee", typeof (bool));
            var userturret = GetBool("usert", typeof (bool));
            var ignite = GetBool("useignite", typeof (bool));
            if (Player.IsWindingUp) return;

            SetIgniteSlot(Player.GetSpellSlot("summonerdot"));

            if (ignite)
            {
                if (target.IsValidTarget(Q.Range) &&
                    (target.Health < IgniteDamage(target) + Q.GetDamage(target)))
                    Player.Spellbook.CastSpell(GetIgniteSlot(), target);
            }

            if (Q.IsReady() && useq && target.IsValidTarget(Q.Range))
            {
                if (Player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) && !W.IsReady())
                    Q.Cast(target);
                else if (Player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target))
                {
                    Q.Cast(target);
                }
            }

            if (E.IsReady() && usee && target.Distance(Player) < 500 && eCanCast())
            {
                if (Player.Distance(target) < Orbwalking.GetRealAutoAttackRange(target) && !W.IsReady())
                    E.Cast(target.Position);
                else if (Player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target))
                {
                   Utility.DelayAction.Add(200, () =>  E.Cast(target.Position));
                }
            }

            var rCount = GetValue("rcount");
            var extendedposition = Player.Position.Extend(target.Position, 500);
            if (ForcePulseCount() < rCount && user && R.IsReady() && Player.IsFacing(target))
            {
                if (target.UnderTurret(true) && userturret) return;
                if (target.HealthPercent - 15 > Player.HealthPercent) return;
                if (Q.IsReady() || (E.IsReady() && (eCanCast() || GetPassiveBuff == 5)) || W.IsReady())
                {
                    if (Player.Mana >= Player.Spellbook.GetSpell(SpellSlot.R).ManaCost + Q.ManaCost)
                    {
                        if (Player.Distance(target) > Orbwalking.GetRealAutoAttackRange(target))
                            R.Cast(extendedposition);
                    }
                }
            }

        }

        private static DamageToUnitDelegate _damageToUnit;
        private static int facing;
        private static int nofacing;
        private static GameObject Veigare;
        public static int dontatttimer;
        public static bool dontAtt = false;
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

            if (!Config.Item("drawdamage").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(4000, TargetSelector.DamageType.Magical);
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
                    Text.text = "Killable " + (int) (unit.Health - damage);
                    Text.OnEndScene();
                }
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);
                var differenceInHp = xPosCurrentHp - xPosDamage;
                var pos1 = barPos.X + 9 + (107*percentHealthAfterDamage);
                for (var i = 0; i < differenceInHp; i++)
                {
                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, FillColor);
                }
            }

        }
    }
}
