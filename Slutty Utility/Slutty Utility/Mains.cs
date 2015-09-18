using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using Slutty_Utility.MenuConfig;

namespace Slutty_Utility
{
    class Mains
    {
        internal static void OnLoad(EventArgs args)
        {
            ConsumablesMenu.LoadConsumablesMenu();
            DefensiveMenu.LoadDefensiveMenu();
            OffensiveMenu.LoadOffensiveMenu();
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {
            Activator.Consumables.Consumable();
            Activator.Defensive.Defensives();
        }
    }
}
