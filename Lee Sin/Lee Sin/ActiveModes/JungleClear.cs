using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lee_Sin.ActiveModes
{
    class JungleClear : LeeSin
    {
        public static void Jungle()
        {
            var jungleminion = MinionManager.GetMinions(Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth).FirstOrDefault();
            var jungleminions = MinionManager.GetMinions(Player.ServerPosition, E.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth);

            if (jungleminion == null) return;

            var useq = GetBool("useqjl", typeof(bool));
            var usew = GetBool("usewjl", typeof(bool));
            var usee = GetBool("useejl", typeof(bool));
            var usesmart = GetBool("usesjl", typeof(bool));

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
                    if (W1() && CanCast(SpellSlot.W) && Player.Distance(jungleminion) < Player.AttackRange + Player.BoundingRadius)

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
                    if (jungleminion.Distance(Player) <= E.Range && CanCast(SpellSlot.E) && E1())
                    {
                        E.Cast();
                        _lastej = Environment.TickCount;
                    }
                    if (jungleminion.Distance(Player) <= Player.AttackRange + Player.BoundingRadius && CanCast(SpellSlot.E) && E2())
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
    }
}
