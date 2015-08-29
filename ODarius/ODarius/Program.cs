using System;
using LeagueSharp.Common;

namespace ODarius
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Darius.Load;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"The Exception Error Is: " + ex);
            }
        }
    }
}
