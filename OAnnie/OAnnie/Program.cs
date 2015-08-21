using LeagueSharp.Common;

namespace OAnnie
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Annie.Load;
            CustomEvents.Game.OnGameLoad += Tibbers.OnLoad;
        }
    }
}