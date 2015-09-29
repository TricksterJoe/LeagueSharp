using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Kindred
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Kindred.OnLoad;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error Has Occured" + "{0}" + ex);
            }
        }
    }
}
