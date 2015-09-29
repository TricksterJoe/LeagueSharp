using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
    class AutoLevelMenu
    {
        public static void OnLoad()
        {
            var autolevel =  new Menu("Auto-Leveler", "Auto Levelers");
            Helper.AddBool(autolevel, "Use Auto-Level", "useautolevel", true);
            autolevel.AddItem(new MenuItem("autolevelmode", "Auto-Level Mode:"))
                .SetValue(new StringList(new[] {"QWE", "WQE", "EQW"}));
            Helper.Config.AddSubMenu(autolevel);
        }
    }
}
