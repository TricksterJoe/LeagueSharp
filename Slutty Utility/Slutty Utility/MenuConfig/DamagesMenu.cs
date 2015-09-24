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

        public static readonly SpellSlot[] Slots =
        {
            SpellSlot.Q,
            SpellSlot.E,
            SpellSlot.W, 
            SpellSlot.R
        };

        public static void LoadDamagesMenu()
        {
            var Damagesmeny = new Menu("Damage Settings", "Damage Settings");
            {
                var DtoP = new Menu("Damage to Player Settings", "Damage To Player Settings");
                {
                    AddBool(DtoP, "Display Selected Target Damage", "dtop.selectedtarget", false);
                    foreach (var champion in HeroManager.Enemies)
                    {
                        var champions = new Menu(champion.ChampionName, champion.ChampionName);
                        DtoP.AddSubMenu(champions);
                        foreach (var spells in Slots)
                        {
                            AddBool(champions, "Display" + " " + spells + " " + "Damage", "damagesmenu.dtop" + spells,
                                true);
                        }
                    }
                }
                Damagesmeny.AddSubMenu(DtoP);

                var DtoT = new Menu("Damage to Target Settings", "Damage To Target Settings");
                {
                    AddBool(DtoT, "Display Combo Damage on Targets", "dtot.damage", false);
                    foreach (var spells in Slots) // probably wrong, will be checking.
                    {
                        AddBool(DtoT, "Display" + " " + spells + " " + "Damage", "damagesmenu.dtot" + spells, true);
                    }
                }
                Damagesmeny.AddSubMenu(DtoT);
            }
            Config.AddSubMenu(Damagesmeny);
        }

    }
}
