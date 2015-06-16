#region

using LeagueSharp.Common;

#endregion

namespace Soraka_Modified
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += SorakaModified.OnGameLoad;
        }
    }
}