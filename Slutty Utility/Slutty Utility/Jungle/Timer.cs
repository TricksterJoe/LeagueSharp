using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    internal class Timer : Helper
    {
        private static float JungleTick = 0;

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid) return;
            if(sender.Type != GameObjectType.obj_AI_Minion)return;
            if (sender.Team != GameObjectTeam.Neutral) return;
            if (!GetBool("jungle.options.drawing.timers", typeof(bool))) return;
            for (var index = 0; index < JungleMonsters.JungleCamps.Count; index++)
            {
                var camp = JungleMonsters.JungleCamps[index];
                for (var index2 = 0; index2 < JungleMonsters.JungleCamps[index].Monsters.Count; index2++)
                {
                    var mob = JungleMonsters.JungleCamps[index].Monsters[index2];
                    if (!String.Equals(mob.Name, sender.Name, StringComparison.CurrentCultureIgnoreCase)) continue;
                    mob.IsDead = false;
                    camp.IsDead = false;
                    JungleMonsters.JungleCamps[index] = camp;
                    JungleMonsters.JungleCamps[index].Monsters[index2] = mob;
                }
            }
        }

        public static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid) return;
            if (sender.Type != GameObjectType.obj_AI_Minion) return;
            if (sender.Team != GameObjectTeam.Neutral) return;
            if (!GetBool("jungle.options.drawing.timers", typeof(bool))) return;
            for (var index = 0; index < JungleMonsters.JungleCamps.Count; index++)
            {
                var camp = JungleMonsters.JungleCamps[index];
                for (var index2 = 0; index2 < JungleMonsters.JungleCamps[index].Monsters.Count; index2++)
                {
                    var mob = JungleMonsters.JungleCamps[index].Monsters[index2];
                    if (!String.Equals(mob.Name, sender.Name, StringComparison.CurrentCultureIgnoreCase)) continue;
                    mob.IsDead = true;
                    camp.IsDead = camp.Monsters.All(m => m.IsDead);
                    JungleMonsters.JungleCamps[index] = camp;
                    JungleMonsters.JungleCamps[index].Monsters[index2] = mob;
                    if(!camp.IsDead)continue;
                    camp.RespawnTime = Game.Time + camp.RespawnTime;

                }
            }

        }
        public static void OnUpdate(EventArgs args)
        {
            if (JungleTick > TickCount) return;
            JungleTick = TickCount + 1000;
            if(JungleMonsters.JungleCamps == null) JungleMonsters.LoadCamps();
            if (!GetBool("jungle.options.drawing.timers", typeof(bool))) return;

            for (var index = 0; index < JungleMonsters.JungleCamps.Count; index++)
            {
                var camp = JungleMonsters.JungleCamps[index];
                if (camp.IsDead)
                    if (camp.RespawnTime - Game.Time <= 0)
                    {
                        camp.IsDead = false;
                        continue;
                    }
                var loc = Drawing.WorldToMinimap(camp.Location.To3D());
                Drawing.DrawText(loc.X,loc.Y, Color.LightGray, string.Format("{mm:ss}", (camp.RespawnTime - Game.Time)));
            }

        }
    }
}
