using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.MenuConfig
{
   internal class DrawingsMenu : Helper
    {
        public static readonly SpellSlot[] Slots =
        {
            SpellSlot.Q,
            SpellSlot.E,
            SpellSlot.W, 
            SpellSlot.R
        };
       public static void DrawingsMenus()
       {

           var drawing = new Menu("Drawing Settings", "Drawing Settings");
           var spellrange = new Menu("Spell Ranges", "Spell Range");
           {
               var spellrangeenemy = new Menu("Draw Enemy Spell Ranges", "Enemy Spell Ranges Draw");
               {
                   foreach (var hero in HeroManager.Enemies)
                   {
                       var spellrangeenemyname = new Menu(hero.ChampionName, hero.ChampionName);
                       spellrangeenemy.AddSubMenu(spellrangeenemyname);
//                       foreach (var herospell in Slots)
//                       {
//                           AddBool(spellrangeenemyname, "Display Range For " + herospell,
//                               "spellrange.spellrangeenemy.spellrangeenemyname" + herospell + hero.ChampionName
//                               , true);
//                       }
                       AddBool(spellrangeenemyname, "Draw Auto Attack Range", "showdrawingsaa" + hero.ChampionName, true);
                       AddBool(spellrangeenemyname, "Show Drawings", "showdrawings" + hero.ChampionName, false);
                   }
                   AddBool(spellrangeenemy, "Display Enemy Ranges", "displayenemyrange", true);
               }
              

               var spellrangeally = new Menu("Draw Ally Spell Ranges", "Ally Spell Ranges Draw");
               {
                   foreach (var hero in HeroManager.Allies)
                   {
                       var spellrangeenemynames = new Menu(hero.ChampionName, hero.ChampionName);
                       spellrangeally.AddSubMenu(spellrangeenemynames);
//                       foreach (var herospell in Slots)
//                       {
//                           AddBool(spellrangeenemynames, "Display Range For " + herospell,
//                               "spellrange.spellrangeenemy.spellrangeallyname" + herospell + hero.ChampionName
//                               , true);
//                       }
                       AddBool(spellrangeenemynames, "Draw Auto Attack Range", "showdrawingsaaa" + hero.ChampionName,
                           true);
                       AddBool(spellrangeenemynames, "Show Drawings", "showdrawingss" + hero.ChampionName, false);
                       AddBool(spellrangeally, "Display Ally Ranges", "displayallyrange", true);
                   }
               }
               spellrange.AddSubMenu(spellrangeenemy);
               spellrange.AddSubMenu(spellrangeally);
           }
           drawing.AddSubMenu(spellrange);
           Config.AddSubMenu(drawing);

       }
    }
}
