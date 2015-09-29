using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
    class EnviormentMenu : Helper
    {
        public static void LoadEnviormentMenu()
        {
            var antirengar = new Menu("Anti-Rengar", "Anti Rengar");
            {
                AddBool(antirengar, "Use Anti-Rengar", "enviorment.antirengar", true);
                if (Player.ChampionName == "Vayne")
                {
                    AddBool(antirengar, "Use R in Anti-Rengar", "userantirengar", true);
                }
            }
            Config.AddSubMenu(antirengar);

            var ultmanager = new Menu("Important Spells Manager", "Important Spells Manager");
            {
                AddBool(ultmanager, "Block Movement", "enviorment.blockmove", true);
                AddBool(ultmanager, "Smart Block", "enviorment.smartblockmove", true);
            }
            Config.AddSubMenu(ultmanager);

            var wardtrack = new Menu("Ward Tracker", "Ward Tracker");
            {
                AddBool(wardtrack, "Track Wards", "enviorment.wards", true);
                AddBool(wardtrack, "Ping on Ward Placement", "enviorment.wardsplace", true);
                AddBool(wardtrack, "Ping on Ward Expire", "enviorment.wardsexpire", true);
                AddBool(wardtrack, "Notify Teamates on Ward Placement", "enviorment.wardsteam", true);
            }
            Config.AddSubMenu(wardtrack);

            var spelltracker = new Menu("Spell Tracker", "Spell Tracker");
            {
                AddBool(spelltracker, "Track Spells", "spelltracker", true);
            }
            Config.AddSubMenu(spelltracker);


        }
    }
}
