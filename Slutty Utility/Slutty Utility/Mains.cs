using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility.MenuConfig;

namespace Slutty_Utility
{
    class Mains : Helper
    {
        internal static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
            MenuConfig.Activator.LoadActivator();
            DamagesMenu.LoadDamagesMenu();
            EnviormentMenu.LoadEnviormentMenu();
            JungleMenu.LeadJungleMenu();
            Config.AddToMainMenu();
        }
    }
}
