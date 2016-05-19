using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Swain
{
    /// <summary>
    /// Main Class
    /// </summary>
    class Swain : Helper
    {
        public static bool RavenForm = false;
        public static Spell Q, W, E, R;
        public static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != "Swain") return; 
                  
            MenuHelper.MenuOnLoad();

            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 900);
            E = new Spell(SpellSlot.E, 625);
            R = new Spell(SpellSlot.R, 625);
            W.SetSkillshot(0.5f, 275, 1250, false, SkillshotType.SkillshotCircle);
            E.SetTargetted(0.5f, 1400);
            Q.SetTargetted(0f, float.MaxValue);
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreateObject;
            GameObject.OnDelete += OnDeleteObject;
            Drawing.OnDraw += OnDraw;
        }

        /// <summary>
        /// Here goes all drawings
        /// </summary>
        /// <param name="args"></param>
        private static void OnDraw(EventArgs args)
        {
            var qdraw = GetBool("drawq", typeof (bool));
            var wdraw = GetBool("draww", typeof(bool));
            var edraw = GetBool("drawe", typeof(bool));

            if (qdraw && Q.IsReady() && Q.Level >= 1)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.DarkBlue, 3);
            }

            if (wdraw && W.IsReady() && W.Level >= 1)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.DarkRed, 3);
            }

            if (edraw && E.IsReady() && E.Level >= 1)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.LimeGreen, 3);
            }
        }

        /// <summary>
        /// Will be Changed to a better method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("swain_demonForm"))
                return;
            RavenForm = false;
        }

        /// <summary>
        /// Will be changed to a better method
        /// </summary> 
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!sender.Name.Contains("swain_demonForm"))
                return;
            RavenForm = true;
        }

        
        /// <summary>
        /// On Update (Updates every tick)
        /// </summary>
        /// <param name="args"></param>
        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.LastHit:
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    Mixed();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    FormChange();
                    break;
                case Orbwalking.OrbwalkingMode.Freeze:
                    break;
                case Orbwalking.OrbwalkingMode.CustomMode:
                    break;
                case Orbwalking.OrbwalkingMode.None:
                    break;
            }
        }

        private static void FormChange()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget()) return;

            var useq = GetBool("useq", typeof(bool));
            var usew = GetBool("usew", typeof(bool));
            var usee = GetBool("usee", typeof(bool));
            var user = GetBool("user", typeof(bool));
            var uservalue = GetValue("minmanarc");


            if (Player.Level >= 6 && R.IsReady() && user)
            {
                foreach (var heros in HeroManager.Enemies.Where(x => x.IsValidTarget(900)))
                {
                    if (RavenForm == false && Player.ManaPercent > uservalue && heros.IsValidTarget(R.Range))
                    {
                        R.Cast();
                    }
                    if (RavenForm == true && (Player.ManaPercent <= uservalue || !heros.IsValidTarget(R.Range)))
                    {
                        R.Cast();
                    }
                }
            }

            if (RavenForm)
            {
                foreach (var heros in HeroManager.Enemies.Where(x => x.IsValidTarget(900)))
                {
                    if (W.IsReady() && heros.IsValidTarget(W.Range) && usew)
                    {
                        W.Cast(heros);
                    }

                    if (E.IsReady() && target.IsValidTarget(E.Range) && usee)
                    {
                        E.Cast(target);
                    }

                    if (Q.IsReady() && target.IsValidTarget(Q.Range) && useq)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        /// <summary>
        /// Lane Clear
        /// </summary>
        private static void LaneClear()
        {
            var minion = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAllyForEnemy,
                MinionOrderTypes.MaxHealth);

            var useq = GetBool("useql", typeof(bool));
            var usee = GetBool("useel", typeof(bool));
            var user = GetBool("userl", typeof(bool));

            var userrminminions = GetValue("minminionsrl");
            var userrminmana = GetValue("minmanarl");

            if (R.IsReady() && minion.Count >= userrminminions && Player.ManaPercent >= userrminmana && user)
            {
                if (RavenForm == true)
                {
                    R.Cast();
                }
            }

            if (R.IsReady() && (minion.Count < userrminminions || Player.ManaPercent < userrminmana) && user)
            {
                if (RavenForm == false)
                {
                    R.Cast();
                }
            }


            if (minion.FirstOrDefault() == null) return;

            var min = minion.FirstOrDefault();

            if (min == null) return;

            if (Q.IsReady() && useq)
            {
                Q.Cast(min);
            }

            if (E.IsReady() && usee)
            {
                E.Cast(min);
            }

        }


        /// <summary>
        /// Mixed/Harass mode
        /// </summary>
        private static void Mixed()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget()) return;

            var useq = GetBool("useqm", typeof (bool));
            var usee = GetBool("useem", typeof(bool));

            if (Q.IsReady() && useq)
            {
                Q.Cast(target);
            }

            if (E.IsReady() && usee)
            {
                E.Cast(target);
            }
        }

        /// <summary>
        /// Combo
        /// </summary>
        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget()) return;

            var useq = GetBool("useq", typeof(bool));
            var usew = GetBool("usew", typeof(bool));
            var usee = GetBool("usee", typeof(bool));
            var user = GetBool("user", typeof(bool));
            var uservalue = GetValue("minmanarc");



            if (RavenForm == false)
            {
                if (Q.IsReady() && target.IsValidTarget(Q.Range) && useq)
                {
                    Q.Cast(target);
                }

                if (W.IsReady() && usew)
                {
                    var wpred = W.GetPrediction(target);
                    if (wpred.Hitchance == HitChance.Immobile || wpred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wpred.CastPosition);
                    }
                }

                if (E.IsReady() && target.IsValidTarget(E.Range) && usee)
                {
                    E.Cast(target);
                }
            }
        }
    }
}
