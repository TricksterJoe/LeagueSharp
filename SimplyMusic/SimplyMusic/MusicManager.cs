using System;
using System.Collections.Generic;

namespace SimplyMusic
{
    class MusicManager
    {
        static List<string> _musicList;

        public static void LoadMusic(List<string> musicList)
        {
            _musicList = musicList;
        }

        public static void PlayMusic()
        {
            for (var i = 0; i < _musicList.Count; i++)
            {
                if (NAudioManager.PlayAudio(_musicList[i])) break;
            }
        }
    }
}
