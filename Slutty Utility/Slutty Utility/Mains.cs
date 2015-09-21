using System;
using LeagueSharp.Common;
using Slutty_Utility.Activator;
using Slutty_Utility.Drawings;
using Slutty_Utility.Enviorment;
using Slutty_Utility.MenuConfig;
using Slutty_Utility.Summoners;

namespace Slutty_Utility
{
    class Mains : Helper
    {
        internal static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            MenuConfig.Activator.LoadActivator();
            DamagesMenu.LoadDamagesMenu();
            EnviormentMenu.LoadEnviormentMenu();
            JungleMenu.LeadJungleMenu();
            DrawingsMenu.DrawingsMenus();
            Config.AddToMainMenu();
            JungleMenu = new JungleMenu();
            Consumables = new Consumables();
            Heal = new Heal();
            
            Consumables.OnLoad(); 
            AntiRengar.OnLoad();
            Defensive.OnLoad();
            EnemyRanges.OnLoad();
            AllyRanges.OnLoad();
            Offensive.OnLoad();
            AntiRengar.OnLoad();
            // UltManager.OnLoad();
            // plzkallen
          //  DtoP.OnLoad();
           // DtoT.OnLoad();
        }

        public static Heal Heal { get; set; }

        public static JungleMenu JungleMenu { get; set; }

        public static Consumables Consumables { get; set; }
    }
}
