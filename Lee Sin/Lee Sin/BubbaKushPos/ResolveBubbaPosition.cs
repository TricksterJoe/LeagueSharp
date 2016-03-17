using System;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using LeagueSharp;
using LeagueSharp.Common;
using Lee_Sin.InsecPos;

namespace Lee_Sin.BubbaKushPos
{
    internal class ResolveBubbaPosition
    {
        public static void GetPosition()
        {
            var heros = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(1100));
            {
                if (heros != null)
                {
                    OnLoad.PredictionRnormal.From = LeeSin.R.GetPrediction(heros).UnitPosition;
                    foreach (
                        var x in
                            HeroManager.Enemies.Where(
                                y => y.NetworkId != heros.NetworkId && y.IsValidTarget(LeeSin.R.Range)))
                    {

                        OnLoad.PredictionRnormal.Unit = x;
                        var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(OnLoad.PredictionRnormal);
                        var castPos = poutput2.CastPosition;
                        var ext = castPos.Extend(OnLoad.PredictionRnormal.From,
                            castPos.Distance(OnLoad.PredictionRnormal.From) + 200);

                        Render.Circle.DrawCircle(ext, 100, Color.Aqua);
                        if (Helper.Player.Distance(ext) < LeeSin.R.Range && LeeSin.HasFlash())
                        {

                            //WardManager.WardJump.WardJumped(ext, true);
                            if (LeeSin.R.Cast(x) == Spell.CastStates.SuccessfullyCasted)
                            {
                                LeeSin.LastBubba = Environment.TickCount;
                            }
                            if (Environment.TickCount - LeeSin.LastBubba < 1000)
                            {
                                Helper.Player.Spellbook.CastSpell(Helper.Player.GetSpellSlot("SummonerFlash"), ext);
                            }
                        }
                    }
                }
            }
        }
    }
}
