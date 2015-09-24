using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_Utility.Activator
{
     class Defensive : Helper
    {
        public static int ZhonyaId, Omen, Seraphs, QSS, Mikaels, Locket, Mountain, Merc;
        public static readonly BuffType[] Bufftype =
        {
            BuffType.Snare, 
            BuffType.Blind, 
            BuffType.Charm, 
            BuffType.Stun,
            BuffType.Fear, 
            BuffType.Slow,
            BuffType.Taunt, 
            BuffType.Suppression
        };

        static Defensive()
        {
            ZhonyaId = 2003;
            Omen = 3143;
            Seraphs = 3040;
            QSS = 3140;
            Merc = 3139;
            Mikaels = 3222;
            Locket = 3190;
            Mountain = 3401;
        }
         

         public static void OnLoad()
         {
            // Obj_AI_Base.OnProcessSpellCast += Processspell;
             Game.OnUpdate += OnUpdate;
         }

         private static void OnUpdate(EventArgs args)
         {
            // Console.WriteLine(ItemData.Total_Biscuit_of_Rejuvenation2.Id);
            // Console.WriteLine("seraphs" + ItemData.Seraphs_Embrace.Id + " " + "QSS" + ItemData.Quicksilver_Sash.Id + ItemData.Mercurial_Scimitar.Id);
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
                     HeroManager.Allies)
                 {
                     if (GetStringValue("locketop" + hero.ChampionName) == 0
                         && hero.Distance(Player) <= 1000
                         && hero.HealthPercent <= Config.Item("lockethp" + hero.ChampionName).GetValue<Slider>().Value
                         && Player.CountEnemiesInRange(1500) >= 1)
                         SelfCast(Locket);
                 }
             }

             #endregion

             #region Seraphs

             if (ItemReady(Seraphs) && HasItem(Seraphs)
                 && GetBool("defensive.seraphmenu", typeof(bool))
                 && Player.HealthPercent <= GetValue("defensive.value"))
             {
                 SelfCast(Seraphs);
             }

             #endregion

             #region Mikaels

             if (ItemReady(Mikaels) && HasItem(Mikaels))
             {
                 foreach (
                     var hero in HeroManager.Allies)
                 {
                     foreach (var buff in Bufftype)
                     {
                         if (!GetBool("defensive.mikaels", typeof(bool)) ||
                             !GetBool("usemikaels" + hero.ChampionName, typeof(bool)))
                             return;
                         if (hero.HasBuffOfType(buff))
                         {
                             if (GetBool("mikalesuse" + buff, typeof (bool)))
                             {
                                 UseUnitItem(Mikaels, hero);
                             }
                         }
                     }
                 }
             }
         

         #endregion

             #region Mountain

             if (ItemReady(Mountain) && HasItem(Mountain))
             {
                 foreach (var hero in HeroManager.Allies.Where(x => x.Distance(Player) <= 700))
                 {
                     if (GetStringValue("Mountain" + hero.ChampionName) == 0
                         && hero.HealthPercent <= Config.Item("facehp" + hero.ChampionName).GetValue<Slider>().Value
                         && Player.CountEnemiesInRange(1500) >= 2)
                     {
                         UseUnitItem(Mountain, hero);
                     }
                 }
             }

             #endregion

             #region QSS

             if ((ItemReady(Merc) && HasItem(Merc))|| (ItemReady(QSS) && HasItem(QSS)))
             {
                 foreach (var buff in Bufftype)
                 {
                     if (GetBool("defensive.qss" + buff, typeof(bool)))
                     {
                         if (Player.HasBuffOfType(buff) && HasItem(QSS))
                         {
                             Utility.DelayAction.Add(GetValue("qssdelay"), () =>  SelfCast(QSS));
                         }
                         else if (Player.HasBuffOfType(buff))
                         {
                             Utility.DelayAction.Add(GetValue("qssdelay"), () => SelfCast(Merc));
                         }
                     }
                 }
             }

             #endregion
         }
         /*
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
          */
    }
}
