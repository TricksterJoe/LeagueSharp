using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Katarina
{
    class WardJump : Katarina
    {
        public static void WardJumped(Vector3 position)
        {
            var objects =
                ObjectManager.Get<Obj_AI_Base>()
                    .FirstOrDefault(
                        x =>
                            x.IsValid && x.Distance(position) < 200 && x.IsAlly && !x.IsDead &&
                            !x.Name.ToLower().Contains("turret"));

            var ward = WardSorter.Wards();

                if (objects == null)
                {
                    if (E.IsReady() && ward != null && Environment.TickCount - Lastcastedw > 200)
                    {
                        Items.UseItem(ward.Id, position);
                    }
                }
            
            
            foreach (
                var wards in
                    ObjectManager.Get<Obj_AI_Base>()
                        .Where(wards => E.IsReady() && (objects != null)))
            {
                E.Cast(objects);
                Lastcastedw = Environment.TickCount;
            }
        }

        public static int Lastcastedw { get; set; }
    }
}