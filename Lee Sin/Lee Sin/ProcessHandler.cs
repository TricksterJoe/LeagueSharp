using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Lee_Sin
{
    class ProcessHandler : LeeSin
    {
        public static void ProcessHandlers()
        {
            if (SelectedAllyAiMinion != null)
            {
                if (SelectedAllyAiMinion.IsDead)
                {
                    SelectedAllyAiMinion = null;
                }
            }
            if (_created)
            {
                Utility.DelayAction.Add(500, () => _created = false);
            }


            if (_processw && Environment.TickCount - _lastprocessw > 500)
            {
                Utility.DelayAction.Add(500, () => _processw = false);
            }

            if (_processroncast && Environment.TickCount - _processroncastr > 500)
            {
                Utility.DelayAction.Add(2500, () => _processroncast = false);
            }

            if (_processW2)
            {
                Utility.DelayAction.Add(2500, () => _processW2 = false);
            }

            if (_processr && Environment.TickCount - _lastprocessr > 100)
            {
                Utility.DelayAction.Add(400, () => _processr = false);
            }

            if (_processr2 && Environment.TickCount - _processr2T > 100)
            {
                Utility.DelayAction.Add(400, () => _processr = false);
            }
        }
    }
}
