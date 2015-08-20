using System;
using System.Collections.Generic;
using System.IO;
namespace SimplyMusic
{
    class IOManager
    {
        public static void ReadFiles(string mask, string source)
        {
            var list = new List<string>();
            var files = Directory.GetFiles(source, mask, SearchOption.AllDirectories);
            list.AddRange(files);
            MusicManager.LoadMusic(list);
        }
    }
}
