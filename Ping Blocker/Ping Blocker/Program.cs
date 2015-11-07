using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Ping_Blocker
{
    internal class Program : Helper
    {

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        public static readonly PingCategory[] PingCategorys =
        {
            PingCategory.AssistMe,
            PingCategory.Danger,
            PingCategory.EnemyMissing,
            PingCategory.Fallback,
            PingCategory.Normal,
            PingCategory.OnMyWay,
        };

        private static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            var allies = new Menu("Allies Settings", "Allies Settings");
            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
            {
                var ally = new Menu(hero.ChampionName, "heronames");
                foreach (var  type in PingCategorys)
                {
                    AddBool(ally, "Use On " + type, "useon" + type + hero.ChampionName, false);
                    AddBool(ally, "Disable All Pings", "disableall" + hero.ChampionName, false);
                }
                allies.AddSubMenu(ally);
            }

            Config.AddSubMenu(allies);
            Config.AddToMainMenu();
            Game.OnPing += OnPing;
        }

        private static void OnPing(GamePingEventArgs args)
        {
            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
            {
                if (args.Source.NetworkId != Player.NetworkId) // most likely redundant, keeping it anyways
                {
                    if (args.Source.NetworkId == hero.NetworkId)
                    {
                        if (GetBool("disableall" + hero.ChampionName, typeof (bool)))
                        {
                            args.Process = false;
                        }
                    }
                }
            }

            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
            {
                if (args.Source.NetworkId != Player.NetworkId) // most likely redundant, keeping it anyways
                {
                  
                    foreach (var types in PingCategorys)
                    {
                        if (GetBool("useon" + types + hero.ChampionName, typeof (bool)) &&
                            args.Source.NetworkId == hero.NetworkId && args.PingType == types)
                        {                          
                            args.Process = false;
                        }
                    }
                }
            }
        }
    }
}
