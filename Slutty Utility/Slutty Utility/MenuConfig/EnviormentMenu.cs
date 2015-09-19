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
            var antirengar = new Menu("Anti Rengar", "Anti Rengar");
            {
                AddBool(antirengar, "Use Anti Rengar", "enviorment.antirengar", true);
            }
            Config.AddSubMenu(antirengar);

            var ultmanager = new Menu("Important Spells Manager", "Important Spells Manager");
            {
                AddBool(ultmanager, "Block Movemenet", "enviorment.blockmove", true);
                AddBool(ultmanager, "Smart Block", "enviorment.smartblockmove", true);
            }
            Config.AddSubMenu(ultmanager);

            var wardtrack = new Menu("Ward Tracker", "Ward Tracker");
            {
                AddBool(wardtrack, "Track Wards", "enviorment.wards", true);
                AddBool(wardtrack, "Ping On Ward Placement", "enviorment.wardsplace", true);
                AddBool(wardtrack, "Ping On Ward Expire", "enviorment.wardsexpire", true);
                AddBool(wardtrack, "Notify Teamates On Ward Placement", "enviorment.wardsteam", true);
            }
            Config.AddSubMenu(wardtrack);

        }
    }
}
