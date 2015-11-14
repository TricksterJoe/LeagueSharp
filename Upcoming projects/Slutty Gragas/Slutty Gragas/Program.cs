using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Gragas
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Gragas.OnLoad;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("An error has occured {0}", ex);
            }
        }
    }
}
