using System;
using LeagueSharp;

namespace Slutty_Utility.Activator
{
    class Consumables : Helper
    {
      //  private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static int HpPotion, ManaPotion, Biscuit, Flask, SorcPotion, WarthPotion, RuinPotion, IronPotion;
        public static string HpBuff, ManaBuff, FlaskBuff, BiscuitBuff;
        public static string SorcElixer, WarthElixer, RuinElixer, IronElixer;

        static Consumables()
        {
            HpPotion = 2003;
            ManaPotion = 2004;
            Biscuit = 2010;
            Flask = 2041;
            SorcPotion = 1337;
            WarthPotion = 1337;
            RuinPotion = 1337;
            IronPotion = 1337;
            HpBuff = "RegenerationPotion";
            ManaBuff = "RegenerationPotion";
            FlaskBuff = "FlaskOfCrystalWater";
            BiscuitBuff = "ItemMiniRegenPotion";
            SorcElixer = "sorcbuff";
            WarthElixer = "warthbuff";
            RuinElixer = "ruinbuff";
            IronElixer = "ironbuff";
        }

        public static void OnEnable()
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

            if (HealthCheck("consumables.potions.biscuit")&& !PlayerBuff(ManaBuff) && !PlayerBuff(FlaskBuff))
            {
                PotionCast(Biscuit, BiscuitBuff);
            }
#endregion

           #region Elixers
            ElixerCast(SorcPotion, SorcElixer, "consumables.elixers.sorcery");
            ElixerCast(WarthPotion, WarthElixer, "consumables.elixers.wrath");
            ElixerCast(RuinPotion, RuinElixer, "consumables.elixers.ruin");
            ElixerCast(IronPotion, IronElixer, "consumables.elixers.iron");
#endregion

        }
    }
}
