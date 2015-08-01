using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    class GlobalManager
    {
        #region Variable Declaration
        private static readonly Obj_AI_Hero PlayerHero = ObjectManager.Player;
        private static DamageToUnitDelegate _damageToUnit;
        private static bool _enableFillDamage = true;
        private static System.Drawing.Color _damageFillColor;
        private static bool _enableDrawingDamage = true;
        private const string _menuName = "Slutty Ryze";

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        //public static Obj_AI_Hero GetHero() => PrivatePlayerHero;
        #endregion
        #region Public Properties
        public static Menu Config { get; set; }

        public static string MenuNAme
        {
            get
            {
                return _menuName;
            }
        }

        public static bool CheckTarget(Obj_AI_Base minion)
        {
            return (minion.IsMinion || minion.MaxHealth > 3 || minion.Armor > 0 || minion.IsTargetable);
        }

        public static Obj_AI_Hero GetHero
        {
            get { return PlayerHero; }
        }

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += DrawManager.Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }

        public static bool CheckMinion(Obj_AI_Base minion)
        {
            return (minion.IsMinion && minion.MaxHealth > 3 && minion.IsTargetable);
        }

        public static int GetPassiveBuff
        {
            get
            {
                var data = GetHero.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                // Does not use C# v6+ T_T
                // return data?.Count ?? 0;
                return data != null ? data.Count : 0;
            }
        }

        public static bool EnableFillDamage
        {
            get { return _enableFillDamage; }

            set { _enableFillDamage = value; }
        }

        public static bool EnableDrawingDamage
        {
            get { return _enableDrawingDamage; }

            set { _enableDrawingDamage = value; }
        }

        public static System.Drawing.Color DamageFillColor
        {
            get { return _damageFillColor; }

            set { _damageFillColor = value; }
        }
        #endregion
    }
}
