using LeagueSharp.Common;

namespace Lee_Sin
{
    class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += LeeSin.Load;
        }
    }
}
