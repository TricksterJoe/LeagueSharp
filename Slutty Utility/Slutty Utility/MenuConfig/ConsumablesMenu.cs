using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility.Activator;

namespace Slutty_Utility
{
    class ConsumablesMenu : Helper
    {
        public static string[] Marksman =
        {
            "Kalista", "Jinx", "Lucian", "Quinn", "Draven", "Varus", "Graves", "Vayne",
            "Caitlyn", "Urgot", "Ezreal", "KogMaw", "Ashe", "MissFortune", "Tristana", "Teemo",
            "Sivir", "Twitch", "Corki"
        };

        public static void LoadConsumablesMenu()
        {
            var consumables = new Menu("Consumables", "Consumables");
            {
                var potions = new Menu("Potions", "Potions");
                {
                    AddValue(potions, "Hp Potion", "consumables.potions.hppotion", 30);
                    AddValue(potions, "Mana Potion", "consumables.potions.manapotion", 30);
                    AddValue(potions, "Biscuit", "consumables.potions.biscuit", 30);
                    AddValue(potions, "Flask", "consumables.potions.flask", 30);
                }
                consumables.AddSubMenu(potions);

                var elixers = new Menu("elixers", "elixers");
                {
                    AddValue(elixers, "Elixir of Iron", "consumables.elixers.iron", 30);
                    AddValue(elixers, "Elixir of Ruin", "consumables.elixers.ruin", 30);
                    AddValue(elixers, "Elixir of Sorcery", "consumables.elixers.sorcery", 30);
                    AddValue(elixers, "Elixir of Wrath", "consumables.elixers.wrath", 30);
                }
                consumables.AddSubMenu(elixers);
            }
            Config.AddSubMenu(consumables);
        }
    }
}
