using System;

using LeagueSharp;
using LeagueSharp.Common;

namespace ChatBlocker
{

    internal class Program
    {
        private static void Main(string[] array)
        {
            CustomEvents.Game.OnGameLoad += delegate
            {
                Game.OnChat += args => args.Process = false;
                Game.OnInput += args => args.Process = false;
            };
        }
    }
}