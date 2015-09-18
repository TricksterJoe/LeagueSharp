using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Activator
{
     class Defensive : Helper
    {
        public static int ZhonyaId, Omen, Seraphs, QSS, Mikaels, Locket, Mountain;

        static Defensive()
        {
            ZhonyaId = 2003;
            Omen = 3143;
            Seraphs = 3040;
            QSS = 3140;
            Mikaels = 3222;
            Locket = 3190;
            Mountain = 3401;
        }


         public Defensive()
         {
             CustomEvents.Game.OnGameLoad += OnLoad;
         }

         private static void OnLoad(EventArgs args)
         {
             Obj_AI_Base.OnProcessSpellCast += Processspell;
         }
         private static void Processspell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
         {
             if (sender.IsAlly || sender.IsMe)
                 return;

             string[] spells =
                {
                    "Add Dangerous Spells"
                };

             for (var i = 0; i <= 1; i++)
             {
                 if (args.Target.IsMe
                     && (args.SData.TargettingType == SpellDataTargetType.Unit
                         || args.SData.TargettingType == SpellDataTargetType.SelfAndUnit)
                     && args.SData.Name == spells[i])
                 {
                     SelfCast(ZhonyaId);
                 }
             }

         }

         public static void Defensives()
         {
             #region Omen

             if (ItemReady(Omen) && HasItem(Omen))
             {
                 if (Player.CountEnemiesInRange(500) >= GetValue("defensive.omencount"))
                 {
                     SelfCast(Omen);
                 }
             }
                      #endregion

             #region Locket

             if (ItemReady(Locket) && HasItem(Locket))
             {
                 foreach (var hero in
                     HeroManager.Allies.Where(x => !x.IsMe))
                 {
                     if (GetStringValue("locketop") == 0
                         && hero.HealthPercent <= Config.Item("lockethp" + hero.ChampionName).GetValue<Slider>().Value
                         && Player.CountEnemiesInRange(1500) >= 2)
                         SelfCast(Locket);
                 }
             }

             #endregion

             #region Seraphs

             if (ItemReady(Seraphs) && HasItem(Seraphs)
                 && GetBool("defensive.seraphmenu")
                 && Player.HealthPercent <= GetValue("defensive.value"))
             {
                 SelfCast(Seraphs);
             }

             #endregion

             #region Mikaels

             if (ItemReady(Mikaels) && HasItem(Mikaels))
             {
                 foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe && x.Distance(Player) <= 800))
                 {
                     if (Config.Item("mikaels" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                     {
                         if (hero.HasBuffOfType(BuffType.Blind) || hero.HasBuffOfType(BuffType.Charm) ||
                             hero.HasBuffOfType(BuffType.Flee) || hero.HasBuffOfType(BuffType.Silence) ||
                             hero.HasBuffOfType(BuffType.Stun) || hero.HasBuffOfType(BuffType.Taunt) ||
                             hero.HasBuffOfType(BuffType.Suppression) || hero.HasBuffOfType(BuffType.Sleep))
                         {
                             UseUnitItem(Mikaels, hero);
                         }
                     }
                 }
             }

             #endregion

             #region Mountain

             if (ItemReady(Mountain) && HasItem(Mountain))
             {
                 foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe && x.Distance(Player) <= 700))
                 {
                     if (GetStringValue("Mountain") == 0
                         && hero.HealthPercent <= Config.Item("facehp" + hero.ChampionName).GetValue<Slider>().Value
                         && Player.CountEnemiesInRange(1500) >= 2)
                     {
                         UseUnitItem(Mountain, hero);
                     }
                 }
             }

             #endregion

             #region QSS

             if (ItemReady(QSS) && HasItem(QSS))
             {
                 if (GetBool("defensive.qss"))
                 {
                     if (Player.HasBuffOfType(BuffType.Blind) || Player.HasBuffOfType(BuffType.Charm) ||
                         Player.HasBuffOfType(BuffType.Flee) || Player.HasBuffOfType(BuffType.Silence) ||
                         Player.HasBuffOfType(BuffType.Stun) || Player.HasBuffOfType(BuffType.Taunt) ||
                         Player.HasBuffOfType(BuffType.Suppression) || Player.HasBuffOfType(BuffType.Sleep))
                     {
                         SelfCast(QSS);
                     }
                 }
             }

             #endregion

         }
    }
}
