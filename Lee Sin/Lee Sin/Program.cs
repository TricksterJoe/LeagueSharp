using LeagueSharp.Common;
using SharpDX;

namespace Lee_Sin
{
    public static class Program
    {
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += LeeSin.Load;
        }

        // HyunMi
        public static Vector3 Move(this Vector3 start, Vector3 end, float distance = 2250f)
        {
            float t = distance / (start.Distance(end));
            return new Vector3(start.X + ((end.X - start.X) * t), start.Y + ((end.Y - start.Y) * t), start.Z + ((end.Z - start.Z) * t));
        }
    }
}
