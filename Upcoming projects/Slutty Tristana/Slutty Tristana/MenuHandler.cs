using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Tristana
{
    class MenuHandler
    {

        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public const string Menuname = "OAhri";
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static void OnLoad()
        {
            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddToMainMenu();
        }
    }
}
