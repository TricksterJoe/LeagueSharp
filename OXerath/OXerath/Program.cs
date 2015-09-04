using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;

namespace OXerath
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Xerath.OnLoad;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An Error has Occured" + ex);
            }
            
        }
    }
}
