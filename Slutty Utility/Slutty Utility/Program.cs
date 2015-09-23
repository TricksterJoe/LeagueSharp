using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

/*
 *    _____ _______    _______ _____ _____   ____   ______     _______  
  / ____|__   __|/\|__   __|_   _/ ____| |  _ \ / __ \ \   / / ____| 
 | (___    | |  /  \  | |    | || |      | |_) | |  | \ \_/ / (___   
  \___ \   | | / /\ \ | |    | || |      |  _ <| |  | |\   / \___ \  
  ____) |  | |/ ____ \| |   _| || |____  | |_) | |__| | | |  ____) | 
 |_____/   |_/_/    \_\_|  |_____\_____| |____/ \____/  |_| |_____/  
                                                                     
                                                                     
 */
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
