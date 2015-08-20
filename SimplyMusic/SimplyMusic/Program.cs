using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplyMusic
{
    class Program
    {
        static void Main(string[] args)
        {
            IOManager.ReadFiles("*.mp3", @"C:\LeagueSharp\Music");
            MusicManager.PlayMusic();
        }
    }
}
