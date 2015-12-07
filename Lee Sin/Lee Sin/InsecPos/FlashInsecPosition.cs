using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Lee_Sin.InsecPos
{
    class FlashInsecPosition : LeeSin
    {
        public static IEnumerable<Obj_AI_Hero> GetAllyHeroes(Obj_AI_Hero unit, int range)
        {
            return
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(hero => hero.IsAlly && !hero.IsMe && !hero.IsDead && hero.Distance(unit) < range).OrderBy(x => x.Distance(Player))
                    .ToList();
        }
        public static Vector3 InsecPos(Obj_AI_Hero target, int extendvalue)
        {

            //  var pos = Player.Position.Extend(target.Position, +target.Position.Distance(Player.Position) + 230);
            if (SelectedAllyAiMinion != null)
            {
                return
                    SelectedAllyAiMinion.Position.Extend(target.Position,
                        +target.Position.Distance(SelectedAllyAiMinion.Position) + extendvalue);

            }
            var objAiHero = GetAllyHeroes(target, 1200).FirstOrDefault();
            if (GetBool("useobjectsallies", typeof(bool)) && objAiHero != null)
            {
                return
                    objAiHero.Position.Extend(target.Position,
                        +target.Position.Distance(objAiHero.Position) + extendvalue);
            }

            if (!GetBool("useobjectsallies", typeof(bool)) || objAiHero == null)
            {
                return Player.Position.Extend(target.Position,
                    +target.Position.Distance(Player.Position) + extendvalue);
            }
            return new Vector3();
        }

    }
}
