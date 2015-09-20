using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Mains.OnLoad;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Exception {0} " + ex);
            }
        }
    }
}
