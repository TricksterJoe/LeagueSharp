using LeagueSharp.Common;
using LeagueSharp;

namespace Slutty_ryze
{
    class ItemManager
    {
        private static Items.Item TearoftheGoddess = new Items.Item(id: 3070, range: 0);
        private static Items.Item TearoftheGoddesss = new Items.Item(id: 3072, range: 0);
        private static Items.Item TearoftheGoddessCrystalScar = new Items.Item(id: 3073, range: 0);
        private static Items.Item ArchangelsStaff = new Items.Item(id: 3003, range: 0);
        private static Items.Item ArchangelsStaffCrystalScar = new Items.Item(id: 3007, range: 0);
        private static int pMuramana = 3042;
        private static Items.Item HealthPotion = new Items.Item(id: 2003, range: 0);
        private static Items.Item CrystallineFlask = new Items.Item(id: 2041,range: 0);
        private static Items.Item ManaPotion = new Items.Item(id: 2004);
        private static Items.Item BiscuitofRejuvenation = new Items.Item(id: 2010,range: 0);
        private static Items.Item SeraphsEmbrace = new Items.Item(id: 3040, range: 0);
        private static Items.Item Manamune = new Items.Item(id: 3004, range: 0);
        private static Items.Item ManamuneCrystalScar = new Items.Item(id: 3008, range: 0);

        // public static int Muramana() => pMuramana;
        public static int Muramana()
        {
            get {return pMuramana;}   
        }
        public static void Potion()
        {
            var autoPotion = GlobalManager.Config.Item("autoPO").GetValue<bool>();
            var hPotion = GlobalManager.Config.Item("HP").GetValue<bool>();
            var mPotion = GlobalManager.Config.Item("MANA").GetValue<bool>();
            var bPotion = GlobalManager.Config.Item("Biscuit").GetValue<bool>();
            var fPotion = GlobalManager.Config.Item("flask").GetValue<bool>();
            var pSlider = GlobalManager.Config.Item("HPSlider").GetValue<Slider>().Value;
            var mSlider = GlobalManager.Config.Item("MANASlider").GetValue<Slider>().Value;
            var bSlider = GlobalManager.Config.Item("bSlider").GetValue<Slider>().Value;
            var fSlider = GlobalManager.Config.Item("fSlider").GetValue<Slider>().Value;

            if (GlobalManager.GetHero().IsRecalling() || GlobalManager.GetHero().InFountain()) return;
            if (!autoPotion) return;

            if (hPotion
                && GlobalManager.GetHero().HealthPercent <= pSlider
                && GlobalManager.GetHero().CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !GlobalManager.GetHero().HasBuff("FlaskOfCrystalWater")
                && !GlobalManager.GetHero().HasBuff("ItemCrystalFlask")
                && !GlobalManager.GetHero().HasBuff("RegenerationPotion"))
                HealthPotion.Cast();

            if (mPotion
                && GlobalManager.GetHero().ManaPercent <= mSlider
                && GlobalManager.GetHero().CountEnemiesInRange(1000) >= 0
                && ManaPotion.IsReady()
                && !GlobalManager.GetHero().HasBuff("RegenerationPotion")
                && !GlobalManager.GetHero().HasBuff("FlaskOfCrystalWater"))
                ManaPotion.Cast();

            if (bPotion
                && GlobalManager.GetHero().HealthPercent <= bSlider
                && GlobalManager.GetHero().CountEnemiesInRange(1000) >= 0
                && BiscuitofRejuvenation.IsReady()
                && !GlobalManager.GetHero().HasBuff("ItemMiniRegenPotion"))
                BiscuitofRejuvenation.Cast();

            if (fPotion
                && GlobalManager.GetHero().HealthPercent <= fSlider
                && GlobalManager.GetHero().CountEnemiesInRange(1000) >= 0
                && CrystallineFlask.IsReady()
                && !GlobalManager.GetHero().HasBuff("ItemMiniRegenPotion")
                && !GlobalManager.GetHero().HasBuff("ItemCrystalFlask")
                && !GlobalManager.GetHero().HasBuff("RegenerationPotion")
                && !GlobalManager.GetHero().HasBuff("FlaskOfCrystalWater"))
                CrystallineFlask.Cast();
        }

        public static void TearStack()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero().ServerPosition, Champion.Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (GlobalManager.Config.Item("tearoptions").GetValue<bool>()
                && !GlobalManager.GetHero().InFountain())
                return;

            if (GlobalManager.GetHero().IsRecalling()
                || minions.Count >= 1)
                return;

            var mtears = GlobalManager.Config.Item("tearSM").GetValue<Slider>().Value;

            if (GlobalManager.GetPassiveBuff == 4)
                return;


            if (!Champion.Q.IsReady() ||
                (!TearoftheGoddess.IsOwned(GlobalManager.GetHero()) && !TearoftheGoddessCrystalScar.IsOwned(GlobalManager.GetHero()) &&
                 !ArchangelsStaff.IsOwned(GlobalManager.GetHero()) && !ArchangelsStaffCrystalScar.IsOwned(GlobalManager.GetHero()) &&
                 !Manamune.IsOwned(GlobalManager.GetHero()) && !ManamuneCrystalScar.IsOwned(GlobalManager.GetHero())) || !(GlobalManager.GetHero().ManaPercent >= mtears))
                return;

            Champion.Q.Cast(Game.CursorPos);
        }
    }
}
