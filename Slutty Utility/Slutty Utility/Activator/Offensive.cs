using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Activator
{
    class Offensive : Helper
    {
        public static int Botrk, Bilge, Hydra, Tiamat, Hextech, Muraman;
        public static Orbwalking.Orbwalker Orbwalker;
        static Offensive()
        {
            Botrk = 3153;
            Bilge = 3144;
            Hydra = 3074;
            Tiamat = 3077;
            Hextech = 3146;
            Muraman = 3042;
        }

        public Offensive()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
        }

        private static void Before_Attack(Orbwalking.BeforeAttackEventArgs args)
        {
            #region Muramana

            var targets = TargetSelector.GetTarget(Player.AttackRange + Player.BoundingRadius,
                TargetSelector.DamageType.Physical);
            if (targets == null) return;
            if (!GetBool("offensive.muramana"))
                return;


            if (!args.Target.IsValid<Obj_AI_Hero>() || !args.Target.IsEnemy || !HasItem(Muraman) ||
                !ItemReady(Muraman))
                return;

            if (PlayerBuff("Muramana"))
                SelfCast(Muraman);

            #endregion
        }

        private static void After_Attack(AttackableUnit unit, AttackableUnit target)
        {
            #region Botrk/Bilge

            var minion = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minion != null)
            {
                if (ItemReady(Hydra) || ItemReady(Tiamat))
                {
                    if (GetBool("offensive.hydraminions") &&
                        (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                         || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit
                         || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed))
                    {
                        if (minion.Count < GetValue("offensive.hydraminonss"))
                        {
                            SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                        }
                    }
                }
            }

            var targets = TargetSelector.GetTarget(550, TargetSelector.DamageType.Physical);
            if (targets == null) return;
            if (ItemReady(Hydra) || ItemReady(Tiamat))
            {
                if (GetBool("offensive.hydracombo") && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                }
            }

            #endregion

        }

        public static void Offensives()
        {
            #region Bilge/Botrk

            var target = TargetSelector.GetTarget(550, TargetSelector.DamageType.Physical);
            if (target == null) return;

            if (ItemReady(Botrk) || ItemReady(Bilge))
            {
                if (HealthCheck("offensive.botrkvalue") && target.Distance(Player) <= 550)
                {
                    UseUnitItem(HasItem(Botrk) ? Botrk : Bilge, target);
                }
            }

            #endregion

            #region Hextech

            if (ItemReady(Hextech) && target.IsValidTarget(700))
            {
                UseUnitItem(Hextech, target);
            }

            #endregion

        }
    }
}
