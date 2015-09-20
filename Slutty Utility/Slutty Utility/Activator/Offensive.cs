using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Activator
{
    class Offensive : Helper
    {
        public static int Botrk, Bilge, Hydra, Tiamat, Hextech, Muraman, Youm;
        static Offensive()
        {
            Botrk = 3153;
            Bilge = 3144;
            Hydra = 3074;
            Tiamat = 3077;
            Hextech = 3146;
            Muraman = 3042;
            Youm = 3142;
        }


        public static void OnLoad()
        {
            Orbwalking.AfterAttack += After_Attack;
            Orbwalking.BeforeAttack += Before_Attack;
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            var target = TargetSelector.GetTarget(1500, TargetSelector.DamageType.Physical);
            if (target == null) return;

            #region Botrk

            if (GetBool("offensive.botrk", typeof (bool)))
            {
                if ((GetBool("offensive.botrk.combo", typeof (bool))
                     && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                    || !GetBool("offensive.botrk.combo", typeof (bool)))
                {
                    if (HealthCheck("offensive.botrkvalue") && target.Distance(Player) <= 550)
                    {
                        UseUnitItem(HasItem(Botrk) ? Botrk : Bilge, target);
                    }
                }
            }

            #endregion


            #region Hextech/Youm

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (ItemReady(Hextech) && target.IsValidTarget(700))
                {
                    UseUnitItem(Hextech, target);
                }

                if (ItemReady(Youm) && HasItem(Youm) && target.IsValidTarget(1000))
                    SelfCast(Youm);
            }

            #endregion

        }

        private static
            void Before_Attack(Orbwalking.BeforeAttackEventArgs args)
        {
            #region Muramana

            var targets = TargetSelector.GetTarget(Player.AttackRange + Player.BoundingRadius,
                TargetSelector.DamageType.Physical);
            if (targets == null) return;
            if (!GetBool("offensive.muramana", typeof(bool)))
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
            #region Hydra/Tiamat

            var minion = MinionManager.GetMinions(Player.Position, 800, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);
            if (minion != null)
            {
                if (ItemReady(Hydra) || ItemReady(Tiamat))
                {
                    if (GetBool("offensive.hydraminions", typeof(bool)) &&
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
                if (GetBool("offensive.hydracombo", typeof(bool)) && Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    SelfCast(HasItem(Hydra) ? Hydra : Tiamat);
                }
            }

            #endregion

        }
    }
}
