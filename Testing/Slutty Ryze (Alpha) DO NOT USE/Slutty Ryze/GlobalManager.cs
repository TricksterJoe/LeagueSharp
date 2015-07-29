using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    class GlobalManager
    {
        public static Menu Config { get; set; }

        private static readonly Obj_AI_Hero _playerHero = ObjectManager.Player;
 
        //public static Obj_AI_Hero GetHero() => PrivatePlayerHero;

        public static Obj_AI_Hero GetHero
        {
            get {return _playerHero;}
        }

        public static int GetPassiveBuff
        {
            get
            {
                var data = GetHero.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                // Does not use C# v6+ T_T
                // return data?.Count ?? 0;
                return data != null ? data.Count : 0;
            }
        }

    }
}
