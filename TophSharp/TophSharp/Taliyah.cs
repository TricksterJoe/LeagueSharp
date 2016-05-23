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

namespace TophSharp
{
    internal class Taliyah : Helper
    {
        private static Spell _q;
        private static Spell _w;
        private static Spell _e;
        private static Spell _r;
        public static SpellSlot Ignite;
        private static bool casted;

        public static void OnLoad(EventArgs args)
        {
            MenuConfig.MenuLoaded();
            Ignite = Player.GetSpellSlot("SummonerDot");
            // thanks to Shine for spell values!

            _q = new Spell(SpellSlot.Q, 900f);
            _q.SetSkillshot(0.5f, 60f, _q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            _w = new Spell(SpellSlot.W, 800f);
            _w.SetSkillshot(0.8f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _e = new Spell(SpellSlot.E, 700f);
            _e.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += OnGameUpdate;
            CustomEvents.Unit.OnDash += OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;
            Interrupter2.OnInterruptableTarget += OnInterrupt;
            Drawing.OnDraw += OnDraw;
            Spellbook.OnCastSpell += OnCastSpell;
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            var usee = GetBool("usee", typeof(bool));
            var target = TargetSelector.GetTarget(Player, _q.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget())
                return;

            if (CanUse(_e, target) && (CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f) && usee)
            {

                if (args.Slot == SpellSlot.E)
                {
                    EJustUsed = Environment.TickCount;
                }

            }
        }

        private static
            void OnDraw(EventArgs args)
        {
            var drawq = GetBool("drawq", typeof(bool));
            var draww = GetBool("draww", typeof(bool));
            var drawe = GetBool("drawe", typeof(bool));

            if (_q.Level > 0 && drawq)
            {
                var color = _q.IsReady() ? Color.CadetBlue : Color.Red;
                Render.Circle.DrawCircle(Player.Position, _q.Range, color, 3);
            }

            if (_w.Level > 0 && draww)
            {
                var color = _w.IsReady() ? Color.Green : Color.Red;
                Render.Circle.DrawCircle(Player.Position, _w.Range, color, 3);
            }

            if (_e.Level > 0 && drawe)
            {
                var color = _e.IsReady() ? Color.DarkOrchid : Color.Red;
                Render.Circle.DrawCircle(Player.Position, _e.Range, color, 3);
            }
        }

        private static void OnInterrupt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
                if (sender.IsValid && CanUse(_w, sender))
                {

                    _w.Cast(sender);
                }
        }

        private static void OnGapClose(ActiveGapcloser gapcloser)
        {
            if (gapcloser.End.Distance(Player.Position) <= _w.Range && CanUse(_w, gapcloser.Sender))
            {
                _w.Cast(gapcloser.End);
            }
        }

        private static void OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (Player.Distance(args.EndPos) <= _w.Range && CanUse(_w, args.Unit))
            {
                _w.Cast(args.Unit);
            }
        }

        private static void OnGameUpdate(EventArgs args)
        {
            switch (Helper.Orbwalker.ActiveMode)
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
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }

            if (GetBool("onofftoggle", typeof (KeyBind)))
            {
                Mixed();
            }
        }

        private static void LastHit()
        {
            var minions = MinionManager.GetMinions(Player.Position, _q.Range, MinionTypes.All, MinionTeam.Enemy,
MinionOrderTypes.MaxHealth);

            var mana = GetValue("minmanal");
            var useqlasthit = GetBool("qlasthit", typeof(bool));
            var usewlasthit = GetBool("wlasthit", typeof(bool));

            if (Player.ManaPercent < mana)
                return;

            foreach (var minion in minions)
            {
                if (minion.Health <= _q.GetDamage(minion) && useqlasthit && _q.IsReady())
                {
                    _q.Cast(minion);
                }
                if (minion.Health <= _w.GetDamage(minion) && usewlasthit && _w.IsReady() && minion.Distance(Player) <= _w.Range)
                {
                    _w.Cast(minion);
                }
            }
        }

        private static void LaneClear()
        {
            var minions = MinionManager.GetMinions(Player.Position, _q.Range, MinionTypes.All, MinionTeam.Enemy,
       MinionOrderTypes.MaxHealth);

            var mana = GetValue("minmana");
            var useqlasthit = GetBool("qlasthitlane", typeof (bool));
            var usewlasthit = GetBool("wlasthitlane", typeof (bool));
            var qlaneclear = GetBool("qlaneclear", typeof (bool));
            var wlaneclear = GetBool("wlaneclear", typeof (bool));
            var minminionsw = GetValue("wlaneclearmin");

            if (Player.ManaPercent < mana)
                return;

            foreach (var minion in minions)
            {
                if (minion.Health <= _q.GetDamage(minion) && useqlasthit && _q.IsReady())
                {
                    _q.Cast(minion);
                }
                if (minion.Health <= _w.GetDamage(minion) && usewlasthit && _w.IsReady() && minion.Distance(Player) <= _w.Range)
                {
                    _w.Cast(minion);
                }

                if (qlaneclear && _q.IsReady())
                {
                    _q.Cast(minion);
                }
            }



            var circularposition = _w.GetCircularFarmLocation(minions);
            if (qlaneclear && circularposition.MinionsHit >= minminionsw && _w.IsReady())
            {
                _w.Cast(circularposition.Position);
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static bool CanUse(Spell spell, AttackableUnit target)
        {
            return spell.IsReady() && Player.Mana >= spell.ManaCost && target.IsValidTarget(spell.Range);
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

        private static void Mixed()
        {
            var useq = GetBool("useqh", typeof(bool));
            var usew = GetBool("usewh", typeof(bool));

            var target = TargetSelector.GetTarget(Player, _q.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget())
                return;

            var wpred = _w.GetPrediction(target);

            if (CanUse(_q, target) && useq)
            {
                _q.Cast(target);
            }

            if (CanUse(_w, target) && usew && wpred.Hitchance >= HitChance.High)
            {
                _w.Cast(wpred.CastPosition);
            }

        }

        private static void Combo()
        {
            var useq = GetBool("useq", typeof (bool));
            var usew = GetBool("usew", typeof(bool));
            var usee = GetBool("usee", typeof(bool));


            var target = TargetSelector.GetTarget(Player, _q.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget()) return;

            if (GetBool("useignite", typeof(bool)))
            {
                if (_q.IsReady() && Ignite.IsReady() && (target.Health <= _q.GetDamage(target) + IgniteDamage(target)))
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }

                if (Ignite.IsReady() && (target.Health <= IgniteDamage(target) - 30))
                {
                    Player.Spellbook.CastSpell(Ignite, target);
                }
            }

            var wpred = _w.GetPrediction(target);
            var qpred = _q.GetPrediction(target);

            if (CanUse(_q, target) && useq && qpred.Hitchance > HitChance.High)
            {
                _q.Cast(qpred.CastPosition);
            }

            if ((CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f) && usee && _e.IsReady() && target.IsValidTarget(_q.Range))
            {         
                      
                _e.Cast(target);              
            }

            if (Environment.TickCount - EJustUsed < 2500 && Environment.TickCount - EJustUsed > 500 &&
                CanUse(_w, target) && !CanUse(_e, target) && usew && wpred.Hitchance >= HitChance.VeryHigh) 
            {
                _w.Cast(wpred.CastPosition);
            }

            if (!CanUse(_e, target) && CanUse(_w, target) && SpellUpSoon(SpellSlot.E) > 1f && usew &&
                wpred.Hitchance >= HitChance.VeryHigh)
            {
                if (CanUse(_e, target) && (CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f) && usee) return;
                _w.Cast(wpred.CastPosition);
            }

            if (SpellUpSoon(SpellSlot.W) < 0.9f && CanUse(_e, target) && usee)
            {
                
                _e.Cast(target);
            }

            if (SpellUpSoon(SpellSlot.W) > 2f && CanUse(_e, target) && usee)
            {
                _e.Cast(target);
            }
        }

        public static int EJustUsed
        { get; set; }
    }
}
