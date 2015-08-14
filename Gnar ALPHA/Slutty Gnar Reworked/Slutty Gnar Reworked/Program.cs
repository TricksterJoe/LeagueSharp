using LeagueSharp.Common;

namespace Slutty_Gnar_Reworked
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args != null)
            CustomEvents.Game.OnGameLoad += Gnar.OnLoad;
        }
    }
}
