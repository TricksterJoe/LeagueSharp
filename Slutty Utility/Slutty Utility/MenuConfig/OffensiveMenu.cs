using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
    class OffensiveMenu : Helper
    {
        internal static void LoadOffensiveMenu()
        {
            var botrk = new Menu("Blade Of The Ruined King/Bilge", "Blade Of The Ruined King/Bilge");
            {
                AddBool(botrk, "Use Botrk/Bilge", "offensive.botrk", true);
                AddValue(botrk, "Use When HP <=", "offensive.botrkvalue", 70);
                AddBool(botrk, "Smart Botrk Usage", "offensive.smartbotrk", true);
            }
            Config.AddSubMenu(botrk);

            var hydra = new Menu("Hydra/Tiamat", "Hydra/Tiamat");
            {
                AddBool(hydra, "Use Hydra/Tiamat Minions", "offensive.hydraminions", true);
                AddValue(hydra, "Use When > Enemies", "offensive.hydraminonss", 3, 1, 10);
                AddBool(hydra, "Use Hydra/Tiamat Combo", "offensive.hydracombo", true);
            }
            Config.AddSubMenu(hydra);

            var muramana = new Menu("Muramana", "Muramana");
            {
                AddBool(muramana, "Use Muramana", "offensive.muramana", true);
            }
            Config.AddSubMenu(muramana);

            var hextech = new Menu("Hextech", "Hextech");
            {
                AddBool(hextech, "Use Hextech", "offensive.hextech", true);
            }
            Config.AddSubMenu(hextech);

            var yom = new Menu("Youmuu's Ghostblade", "Youmuu's Ghostblade");
            {
                AddBool(yom, "Use Yoummuu's", "offensive.yom", true);
            }
        }
    }
}
