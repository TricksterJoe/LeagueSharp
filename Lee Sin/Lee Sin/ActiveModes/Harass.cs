using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Lee_Sin.ActiveModes
{
    class Harass : LeeSin
    {
        public static void Harassed()
        {
            if (Player.Mana < GetValue("minenergy")) return;

            var useq = GetBool("useqh", typeof(bool));
            var usee = GetBool("useeh", typeof(bool));
            var useQ2 = GetBool("useq2h", typeof(bool));
            var delay = GetValue("secondqdelayh");
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!target.IsValidTarget())
                return;

            if (useq)
            {
                if (Environment.TickCount - Lastqh > 100 && Environment.TickCount - Lasteh > 300)
                {
                    var qpred = Q.GetPrediction(target);
                    if (Q.IsReady() && Q1() &&
                        (qpred.Hitchance >= LeagueSharp.Common.HitChance.High))
                    {
                        Q.Cast(target);
                        Lastqh = Environment.TickCount;
                    }

                    if (!useQ2) return;

                    if (Q2() && Q.IsReady() && Environment.TickCount - Lastqc > delay)
                    {
                        Q.Cast();
                        Lastqh = Environment.TickCount;
                    }
                }


                if (usee)
                {
                    if (Environment.TickCount - Lastqh > 300 && Environment.TickCount - Lasteh > 300)
                    {
                        if (target.Distance(Player) <= E.Range && Q1())
                        {
                            E.Cast();
                            Lasteh = Environment.TickCount;
                        }
                        if ((Player.Distance(target) >
                             Player.AttackRange + Player.BoundingRadius + target.BoundingRadius + 100 ||
                             Environment.TickCount - Laste > 2700) &&
                            Q2())
                        {
                            E.Cast();
                            Lasteh = Environment.TickCount;
                        }
                    }
                }
            }
        }
    }
}
