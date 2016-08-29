using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;

namespace DashCounter
{
    public class SpellData
    {
        public string championName;
        public int range;
        public int spellDelay;
        public SpellSlot slot;

        public SpellData() { }

        public SpellData(string ChampionName,
        int Range,
        int SpellDelay,
        SpellSlot Slot)
        {
            championName = ChampionName;
            range = Range;
            spellDelay = SpellDelay;
            slot = Slot;
        }
    }
}
