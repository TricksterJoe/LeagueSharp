using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Jungle
{
    internal class Timer : Helper
    {
        protected float _jungleTick = 0f;

        private void OnCreate(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid) return;
            if(sender.Type != GameObjectType.obj_AI_Minion)return;
            if (sender.Team != GameObjectTeam.Neutral) return;
            foreach (var camp in JungleMonsters.JungleCamps)
            {
                var mob = JungleMonsters.JungleCamps.FirstOrDefault(m => m.Key.ToLower().Contains(sender.Name.ToLower()));
                if(mob.Key == null)continue;

            }
        }
        private void Game_OnUpdate()
        {
            if (_jungleTick > TickCount) return;
            _jungleTick = TickCount + 1000;
            if(JungleMonsters.JungleCamps == null) JungleMonsters.LoadCamps();



        }
    }
}
