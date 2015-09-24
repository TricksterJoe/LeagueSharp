using System;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility.Activator;
using Slutty_Utility.Damages;
using Slutty_Utility.Drawings;
using Slutty_Utility.Enviorment;
using Slutty_Utility.MenuConfig;
using Slutty_Utility.Summoners;
using Slutty_Utility.Tracker;
using Slutty_Utility.Jungle;

/*
 *    _____ _______    _______ _____ _____   ____   ______     _______  
  / ____|__   __|/\|__   __|_   _/ ____| |  _ \ / __ \ \   / / ____| 
 | (___    | |  /  \  | |    | || |      | |_) | |  | \ \_/ / (___   
  \___ \   | | / /\ \ | |    | || |      |  _ <| |  | |\   / \___ \  
  ____) |  | |/ ____ \| |   _| || |____  | |_) | |__| | | |  ____) | 
 |_____/   |_/_/    \_\_|  |_____\_____| |____/ \____/  |_| |_____/  
                                                                     
                                                                     
 */
namespace Slutty_Utility
{
    class Mains : Helper
    {
        internal static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
            Orbwalker = new Orbwalking.Orbwalker(Menu.root);
            ActivatorMenu.LoadActivator();
            DamagesMenu.LoadDamagesMenu();
            EnviormentMenu.LoadEnviormentMenu();
            JungleMenu.LoadJungleMenu();
           DrawingsMenu.DrawingsMenus();
            SummonersMenu.LoadSummonersMenu();
            AutoLevelMenu.OnLoad();
            Config.Item("useautolevel").SetValue(false);
            Config.AddToMainMenu();

            // Activator
            Defensive.OnLoad();
            Offensive.OnLoad();
            Consumables.OnLoad();

            // Summoners
            Ignite.OnLoad();
            Heal.OnLoad();
            Cleanse.OnLoad();
            Barrier.OnLoad();

            //Drawings //todo DtoP
           EnemyRanges.OnLoad();
            AllyRanges.OnLoad();
            Wards.OnLoad();
            TrackerSpell.OnLoad();
            //DtoT.OnLoad();s
            //  DtoP.OnLoad(); 

            //Jungle (yes smite contains everything in jungle) //todo Jungle timers
             Smite.OnLoad();

            //Enviormenet //todo Ult Manager, Inhibs, Turn Around, Turrets
            AntiRengar.OnLoad();
            Auto_Level_Manager.AutoLevel.OnLoad();
            //UltManager.OnLoad();
            //Inhibitors.OnLoad();
            //TurnAround.OnLoad();
            //Turrets.OnLoad();
        }
    }
}
