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
            if (Created)
            {
                Utility.DelayAction.Add(500, () => Created = false);
            }


            if (Processw && Environment.TickCount - Lastprocessw > 500)
            {
                Utility.DelayAction.Add(500, () => Processw = false);
            }

            if (Processroncast && Environment.TickCount - Processroncastr > 500)
            {
                Utility.DelayAction.Add(2500, () => Processroncast = false);
            }

            if (ProcessW2)
            {
                Utility.DelayAction.Add(2500, () => ProcessW2 = false);
            }

            if (Processr && Environment.TickCount - Lastprocessr > 100)
            {
                Utility.DelayAction.Add(400, () => Processr = false);
            }

            if (Processr2 && Environment.TickCount - Processr2T > 100)
            {
                Utility.DelayAction.Add(400, () => Processr = false);
            }
        }
    }
}
