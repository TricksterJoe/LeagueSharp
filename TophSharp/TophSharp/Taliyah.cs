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

        public static void OnLoad(EventArgs args)
        {
            MenuConfig.MenuLoaded();
            Ignite = Player.GetSpellSlot("SummonerDot");
            // thanks to Shine for spell values!
            _q = new Spell(SpellSlot.Q, 900f);
            _q.SetSkillshot(0f, 60f, _q.Instance.SData.MissileSpeed, true, SkillshotType.SkillshotLine);

            _w = new Spell(SpellSlot.W, 800f);
            _w.SetSkillshot(0.5f, 50f, float.MaxValue, false, SkillshotType.SkillshotCircle);

            _e = new Spell(SpellSlot.E, 700f);
            _e.SetSkillshot(0.25f, 150f, 2000f, false, SkillshotType.SkillshotLine);

            Game.OnUpdate += OnGameUpdate;
            CustomEvents.Unit.OnDash += OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapClose;
            Interrupter2.OnInterruptableTarget += OnInterrupt;
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
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        private static bool CanUse(Spell spell, Obj_AI_Base target)
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

        private static void Combo()
        {
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

            if (CanUse(_q, target))
            {
                _q.Cast(target);
            }

            if (CanUse(_e, target) && (CanUse(_w, target) || SpellUpSoon(SpellSlot.W) < 0.5f))
            {
                _e.Cast(target);
                    EJustUsed = Environment.TickCount;
                
            }

            if (Environment.TickCount - EJustUsed < 2500 && Environment.TickCount - EJustUsed > 500 && CanUse(_w, target) && !CanUse(_e, target))
            {
                _w.Cast(wpred.CastPosition);
            }

            if (!CanUse(_e, target) && CanUse(_w, target) && SpellUpSoon(SpellSlot.E) > 1f)
            {
                _w.Cast(wpred.CastPosition);
            }

            if (SpellUpSoon(SpellSlot.W) < 0.9f && CanUse(_e, target))
            {
                _e.Cast(target);
            }
        }

        public static int EJustUsed
        { get; set; }
    }
}
