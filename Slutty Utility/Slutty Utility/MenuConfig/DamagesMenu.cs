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
                        var Damagesmeny = new Menu("Damage Settingssss", "Damage Settsssings");
            {
                var DtoP = new Menu("Damage To Player Settings", "Damage To Player Settings");
                {
                    AddBool(DtoP, "Display Selected Target Damage", "dtop.selectedtarget", true);
                    foreach (var champion in HeroManager.Enemies.Where(x => !x.IsMe && x.IsValidTarget()))
                    {
                        var champions = new Menu(champion.ChampionName, champion.ChampionName);
                        DtoP.AddSubMenu(champions);
                        foreach (var spells in Slots) // probably wrong, will be checking.
                        {
                            AddBool(champions, "Display" + " " + spells + " " + "Damage", "damagesmenu.dtop" + spells,
                                true);
                        }
                    }
                }
                Damagesmeny.AddSubMenu(DtoP);

                var DtoT = new Menu("Damage To Target Settings", "Damage To Target Settings");
                {
                    AddBool(DtoT, "Display Combo Damage On Targets", "dtot.damage", true);
                    foreach (var spells in Slots) // probably wrong, will be checking.
                    {
                        AddBool(DtoT, "Display" + " " + spells + " " + "Damage", "damagesmenu.dtop" + spells, true);
                    }
                }
                Damagesmeny.AddSubMenu(DtoT);
            }
            Config.AddSubMenu(Damagesmeny);
        }
         
    }
}
