using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility.Activator;
using Slutty_Utility.Damages;
using Slutty_Utility.Enviorment;
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
           Defensive.OnLoad();
            Offensive.OnLoad();
//            AntiRengar.OnLoad();
//            UltManager.OnLoad();

            // plzkallen
          //  DtoP.OnLoad();
           // DtoT.OnLoad();
            Consumables.OnEnable();
        }
    }
}
