using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp;
using LeagueSharp.Common;


namespace DashCounter
{
   public class Database
    {
        public static List<SpellData> Spells = new List<SpellData>();

      static Database()
        {
            Spells.Add(
                new SpellData
                {
                    championName = "Nami",
                    range = 1625,
                    spellDelay = 950,
                    slot = SpellSlot.Q,
                });
        }
    }
}
