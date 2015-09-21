using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_ryze
{
    class ItemManager
    {
        #region Variable Declaration
        private static Items.Item _tearoftheGoddess = new Items.Item(3070, 0);
        private static Items.Item _tearoftheGoddesss = new Items.Item(3072, 0);
        private static Items.Item _tearoftheGoddessCrystalScar = new Items.Item(3073, 0);
        private static Items.Item _archangelsStaff = new Items.Item(3003, 0);
        private static Items.Item _archangelsStaffCrystalScar = new Items.Item(3007, 0);
        private static int _pMuramana = 3042;
        private static Items.Item _healthPotion = new Items.Item(2003, 0);
        private static Items.Item _crystallineFlask = new Items.Item(2041, 0);
        private static Items.Item _manaPotion = new Items.Item(2004);
        private static Items.Item _biscuitofRejuvenation = new Items.Item(2010, 0);
        private static Items.Item _seraphsEmbrace = new Items.Item(3040, 0);
        private static Items.Item _manamune = new Items.Item(3004, 0);
        private static Items.Item _manamuneCrystalScar = new Items.Item(3008, 0);
        #endregion
        #region Public Properties
        // public static int Muramana() => pMuramana;
        public static int Muramana
        {
            get {return _pMuramana;}   
        }
        #endregion
        #region Public Functions
        public static void Item()
        {
            var staff = GlobalManager.Config.Item("staff").GetValue<bool>();
            var staffhp = GlobalManager.Config.Item("staffhp").GetValue<Slider>().Value;

            if (!staff || !Items.HasItem(ItemData.Seraphs_Embrace.Id) || !(GlobalManager.GetHero.HealthPercent <= staffhp)) return;

            Items.UseItem(ItemData.Seraphs_Embrace.Id);
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

            if (GlobalManager.GetHero.IsRecalling() || GlobalManager.GetHero.InFountain()) return;
            if (!autoPotion) return;

            if (hPotion
                && GlobalManager.GetHero.HealthPercent <= pSlider
                && GlobalManager.GetHero.CountEnemiesInRange(1000) >= 0
                && _healthPotion.IsReady()
                && !GlobalManager.GetHero.HasBuff("FlaskOfCrystalWater")
                && !GlobalManager.GetHero.HasBuff("ItemCrystalFlask")
                && !GlobalManager.GetHero.HasBuff("RegenerationPotion"))
                _healthPotion.Cast();

            if (mPotion
                && GlobalManager.GetHero.ManaPercent <= mSlider
                && GlobalManager.GetHero.CountEnemiesInRange(1000) >= 0
                && _manaPotion.IsReady()
                && !GlobalManager.GetHero.HasBuff("RegenerationPotion")
                && !GlobalManager.GetHero.HasBuff("FlaskOfCrystalWater"))
                _manaPotion.Cast();

            if (bPotion
                && GlobalManager.GetHero.HealthPercent <= bSlider
                && GlobalManager.GetHero.CountEnemiesInRange(1000) >= 0
                && _biscuitofRejuvenation.IsReady()
                && !GlobalManager.GetHero.HasBuff("ItemMiniRegenPotion"))
                _biscuitofRejuvenation.Cast();

            if (fPotion
                && GlobalManager.GetHero.HealthPercent <= fSlider
                && GlobalManager.GetHero.CountEnemiesInRange(1000) >= 0
                && _crystallineFlask.IsReady()
                && !GlobalManager.GetHero.HasBuff("ItemMiniRegenPotion")
                && !GlobalManager.GetHero.HasBuff("ItemCrystalFlask")
                && !GlobalManager.GetHero.HasBuff("RegenerationPotion")
                && !GlobalManager.GetHero.HasBuff("FlaskOfCrystalWater"))
                _crystallineFlask.Cast();
        }

        public static void TearStack()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero.ServerPosition, Champion.Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (GlobalManager.Config.Item("tearoptions").GetValue<bool>()
                && !GlobalManager.GetHero.InFountain())
                return;

            if (GlobalManager.GetHero.IsRecalling()
                || minions.Count >= 1)
                return;

            var mtears = GlobalManager.Config.Item("tearSM").GetValue<Slider>().Value;

            if (GlobalManager.GetPassiveBuff == 4)
                return;


            if (!Champion.Q.IsReady() ||
                (!_tearoftheGoddess.IsOwned(GlobalManager.GetHero) && !_tearoftheGoddessCrystalScar.IsOwned(GlobalManager.GetHero) &&
                 !_archangelsStaff.IsOwned(GlobalManager.GetHero) && !_archangelsStaffCrystalScar.IsOwned(GlobalManager.GetHero) &&
                 !_manamune.IsOwned(GlobalManager.GetHero) && !_manamuneCrystalScar.IsOwned(GlobalManager.GetHero)) || !(GlobalManager.GetHero.ManaPercent >= mtears))
                return;

            if (!Game.CursorPos.IsZero)
                Champion.Q.Cast(Game.CursorPos);
            else
                Champion.Q.Cast();
        }
        #endregion
    }
}
