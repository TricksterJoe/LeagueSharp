using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_Utility.Activator
{
     class Consumables : Helper
    {
      //  private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static int HpPotion, ManaPotion, Biscuit, Flask, SorcPotion, WarthPotion, RuinPotion, IronPotion;
        public static string HpBuff, ManaBuff, FlaskBuff, BiscuitBuff;
        public static string SorcElixer, WarthElixer, RuinElixer, IronElixer;
        private static bool bought;

        static Consumables()
        {
            HpPotion = 2003;
            ManaPotion = 2004;
            Biscuit = 20;
            Flask = 2041;
            SorcPotion = 2139;
            WarthPotion = 2140;
            RuinPotion = 2137;
            IronPotion = 2138;
            HpBuff = "RegenerationPotion";
            ManaBuff = "RegenerationPotion";
            FlaskBuff = "FlaskOfCrystalWater";
            BiscuitBuff = "ItemMiniRegenPotion";
            SorcElixer = "ElixirOfSorcery";
            WarthElixer = "ElixirOfWrath";
            RuinElixer = "ElixirOfRuin";
            IronElixer = "ElixirOfIron";
        }

        public static void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
        }

        private static void OnUpdate(EventArgs args)
        {        
                
           #region Potions
            if (HealthCheck("consumables.potions.hppotion") && !PlayerBuff(HpBuff))
            {
                PotionCast(HpPotion, HpBuff);
            }

            if (ManaCheck("consumables.potions.manapotion") && !PlayerBuff(ManaBuff))
            {
                PotionCast(ManaPotion , ManaBuff);
            }

            if (HealthCheck("consumables.potions.flask")&& !PlayerBuff(HpBuff) && !PlayerBuff(FlaskBuff))
            {
                PotionCast(Flask, FlaskBuff);
            }

            if (HealthCheck("consumables.potions.biscuit") && !PlayerBuff(BiscuitBuff) && !PlayerBuff(FlaskBuff))
            {
                Items.UseItem(ItemData.Total_Biscuit_of_Rejuvenation2.Id);
            }
#endregion

           #region Elixers
            /*
            int[] id =
            {
                SorcPotion,
                WarthPotion,
                RuinPotion,
                IronPotion
            };

            string[] buffname =
            {
                SorcElixer,
                WarthElixer, 
                RuinElixer,
                IronElixer
            };
            
            for (var i = 0; i >= buffname.Count(); i++)
            {
                hasbuff = PlayerBuff(buffname[i]);
            }
              /*          
            for (var i = 0; i >= id.Count(); i++)
            {
                
                    bought = HasItem(i);
            }
            */
            if (Player.Level >= GetValue("consumables.buy"))
            {
                if (Player.InShop() && Player.Gold >= 400
                    && (!HasItem(SorcPotion) && !HasItem(WarthPotion) && !HasItem(RuinPotion) && !HasItem(RuinPotion))
                    &&
                    (!PlayerBuff(SorcElixer) && !PlayerBuff(IronElixer) && !PlayerBuff(WarthElixer) &&
                     !PlayerBuff(RuinElixer)))
                {
                    if (GetBool("consumables.elixers.sorcery", typeof (bool)))
                    {
                        Player.BuyItem(ItemId.Elixir_of_Sorcery);
                        ElixerCast(SorcPotion, SorcElixer);
                    }

                    if (GetBool("consumables.elixers.wrath", typeof (bool)))
                    {
                        Player.BuyItem(ItemId.Elixir_of_Wrath);
                        ElixerCast(WarthPotion, WarthElixer);
                    }

                    if (GetBool("consumables.elixers.ruin", typeof (bool)))
                    {
                        Player.BuyItem(ItemId.Elixir_of_Ruin);
                        ElixerCast(RuinPotion, RuinElixer);
                    }

                    if (GetBool("consumables.elixers.iron", typeof (bool)))
                    {
                        Player.BuyItem(ItemId.Elixir_of_Iron);
                        ElixerCast(IronPotion, IronElixer);
                    }
                }
            }





            #endregion

        }

        public static bool hasbuff { get; set; }
    }
}
