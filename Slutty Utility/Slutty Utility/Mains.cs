using System;
using LeagueSharp.Common;
using Slutty_Utility.Activator;
using Slutty_Utility.Drawings;
using Slutty_Utility.Enviorment;
using Slutty_Utility.MenuConfig;
using Slutty_Utility.Summoners;
using Slutty_Utility.Tracker;
using Slutty_Utility.Jungle;

namespace Slutty_Utility
{
    class Mains : Helper
    {
        public static Menu menu;
        internal static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
          //  menu = new Menu(Menuname, Menuname, true);
          //  Orbwalker = new Orbwalking.Orbwalker(menu.SubMenu("Orbwalking"));
            ActivatorMenu.LoadActivator();
            DamagesMenu.LoadDamagesMenu();
            EnviormentMenu.LoadEnviormentMenu();
            JungleMenu.LoadJungleMenu();
            DrawingsMenu.DrawingsMenus();
            SummonersMenu.LoadSummonersMenu();
            Config.AddToMainMenu();
         
            Consumables.OnLoad(); 
            AntiRengar.OnLoad();
            Defensive.OnLoad();
            EnemyRanges.OnLoad();
            AllyRanges.OnLoad();
            Offensive.OnLoad();
            Ignite.OnLoad();
            Heal.OnLoad();
            Cleanse.OnLoad();
            Smite.OnLoad();
            Barrier.OnLoad();
          //  Wards.OnLoad();
            TrackerSpell.OnLoad();
            // UltManager.OnLoad();
            // plzkallen
            //  DtoP.OnLoad();
            // DtoT.OnLoad();

        }
    }
}
