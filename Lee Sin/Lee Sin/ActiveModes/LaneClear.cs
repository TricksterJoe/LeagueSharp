using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lee_Sin.ActiveModes
{
    class LaneClear : LeeSin
    {
        public static void LastHit()
        {
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            if (minion.FirstOrDefault() == null) return;
            var min = GetValue("minenergylh");
            if (Player.Mana < min) return;
            var lh = GetBool("useqlh", typeof(bool));
            if (!lh) return;

            foreach (var minionlh in minion)
            {
                if (minionlh.Health < Q.GetDamage(minionlh))
                {
                    Q.Cast(minionlh);
                }
            }
        }


        public static void Lane2()
        {
            if (Player.Mana <= GetValue("minenergyl")) return;
            var minion = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);


            var usee = GetBool("useel", typeof(bool));
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

        public static void Lane()
        {
            if (Player.Mana <= GetValue("minenergyl")) return;
            var minion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);

            if (minion.FirstOrDefault() == null) return;

            var useq = GetBool("useql", typeof(bool));

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
    }
}
