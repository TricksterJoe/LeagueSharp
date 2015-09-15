using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;


namespace Slutty_Thresh
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null) return;
            try
            {
                CustomEvents.Game.OnGameLoad += SluttyThresh.OnLoad;
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }
    }
}