using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Swain
{
    class Swain : Helper
    {

        public static Spell Q, W, E, R;
        public static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
            if (Player.ChampionName != "Swain") return;

           
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            Config.AddToMainMenu();

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
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = false;
        }

        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!(sender.Name.Contains("swain_demonForm")))
                return;
            RavenForm = true;
        }

        public static bool RavenForm = false;

        private static void OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
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
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);
            if (!target.IsValidTarget()) return;

            if (RavenForm == false)
            {
                if (Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (W.IsReady())
                {
                    var wpred = W.GetPrediction(target);
                    if (wpred.Hitchance == HitChance.Immobile || wpred.Hitchance >= HitChance.High)
                    {
                        W.Cast(wpred.CastPosition);
                    }
                }

                if (E.IsReady() && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }
            }

            if (Player.Level >= 6 && R.IsReady())
            {
                if (RavenForm == false && Player.ManaPercent > 30 && target.IsValidTarget(R.Range))
                {
                    R.Cast();
                }
                if (RavenForm == true && (Player.ManaPercent <= 30 || !target.IsValidTarget(R.Range)))
                {
                    R.Cast();
                }
            }

            if (RavenForm == true)
            {
                foreach (var heros in HeroManager.Enemies.Where(x => x.IsValidTarget(800)))
                {
                    if (W.IsReady() && heros.IsValidTarget(W.Range))
                    {
                        W.Cast(heros);
                    }

                    if (E.IsReady())
                    {
                        E.Cast();
                    }

                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                    }
                }
            }
        }
    }
}
