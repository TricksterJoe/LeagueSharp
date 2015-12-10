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
                if (Environment.TickCount - _lastqh > 100 && Environment.TickCount - _lasteh > 300)
                {
                    var qpred = Q.GetPrediction(target);
                    if (Q.IsReady() && Q1() &&
                        (qpred.Hitchance >= LeagueSharp.Common.HitChance.High))
                    {
                        Q.Cast(target);
                        _lastqh = Environment.TickCount;
                    }

                    if (!useQ2) return;

                    if (Q2() && Q.IsReady() && Environment.TickCount - _lastqc > delay)
                    {
                        Q.Cast();
                        _lastqh = Environment.TickCount;
                    }
                }


                if (usee)
                {
                    if (Environment.TickCount - _lastqh > 300 && Environment.TickCount - _lasteh > 300)
                    {
                        if (target.Distance(Player) <= E.Range && Q1())
                        {
                            E.Cast();
                            _lasteh = Environment.TickCount;
                        }
                        if ((Player.Distance(target) >
                             Player.AttackRange + Player.BoundingRadius + target.BoundingRadius + 100 ||
                             Environment.TickCount - _laste > 2700) &&
                            Q2())
                        {
                            E.Cast();
                            _lasteh = Environment.TickCount;
                        }
                    }
                }
            }
        }
    }
}
