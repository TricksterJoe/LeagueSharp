using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
    class DamagesMenu : Helper
    {

        private static readonly SpellSlot[] Slots =
        {
            SpellSlot.R, SpellSlot.E,
            SpellSlot.W, SpellSlot.Q
        };
        public static void LoadDamagesMenu()
        {
            var DtoP = new Menu("Damage To Player Settings", "Damage To Player Settings");
            {
                foreach (var champion in HeroManager.Enemies.Where(x => !x.IsMe && x.IsValidTarget()))
                {
                    var champions = new Menu(champion.ChampionName, champion.ChampionName);
                    foreach (var spells in Slots) // probably wrong, will be checking.
                    {
                        AddBool(champions, "Display" + " " + spells + " " + "Damage", "damagesmenu.dtop" + spells, true);
                    }
                }
            }
            Config.AddSubMenu(DtoP);
        }
    }
}
