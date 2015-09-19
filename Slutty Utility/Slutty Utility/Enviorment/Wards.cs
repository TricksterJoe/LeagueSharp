using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Utility.Enviorment
{
    class Wards
    {
        public Wards()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private void OnLoad(EventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
