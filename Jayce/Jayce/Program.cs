using LeagueSharp.Common;

namespace Jayce
{
    class Program 
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Jayce.OnLoad;
        }
    }
}
