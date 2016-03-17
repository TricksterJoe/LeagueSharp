using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Lee_Sin.BubbaKush
{
    internal class ResolveBubbaPosition
    {
        public static void GetPosition()
        {
            foreach (Obj_AI_Hero heros in HeroManager.Enemies.Where(x => x.IsValidTarget(1100)))
            {
                 OnLoad.PredictionRnormal.From = LeeSin.R.GetPrediction(heros).CastPosition;

                foreach (
                    var enemy in
                        HeroManager.Enemies.Where(
                            x =>
                                x.IsValidTarget() && x.NetworkId != heros.NetworkId &&
                                x.Distance(heros) < LeeSin.Rnormal.Range)) 
                {
                    OnLoad.PredictionRnormal.Unit = enemy;

                    var poutput2 = SebbyLib.Prediction.Prediction.GetPrediction(OnLoad.PredictionRnormal);
                    var castPos = poutput2.CastPosition;
                    var ext = castPos.Extend(OnLoad.PredictionRnormal.From, castPos.Distance(OnLoad.PredictionRnormal.From) + 250);

                    if (Helper.Player.Distance(ext) < LeeSin.W.Range)
                    {
                        WardManager.WardJump.WardJumped(ext, true);
                    }
                }

            }
        }
    }
}
