using System;
using LeagueSharp.Common;

namespace OAhri
{
   internal class Program
   {

       private static void Main(string[] args)
       {
           try
           {
               CustomEvents.Game.OnGameLoad += Ahri.Load;
           }
           catch (Exception ex)
           {            
               Console.WriteLine(@"The Exception Error Is: " + ex);
           }
       }
    }
}
