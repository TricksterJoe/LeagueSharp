using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace The_Slutty_Xerath
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Xerath.OnLoad;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("A Message has Occured: " + ex);
            }
        }
    }
}
