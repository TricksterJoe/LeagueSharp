using System.Drawing;
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
        private const string _menuName = "Slutty Ryze";

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);

        #endregion
        #region Public Properties
        #region Auto Properties
        public static Menu Config { get; set; }
        public static bool EnableFillDamage { get; set; }
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }
        #endregion
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
            return minion != null && (minion.IsMinion && minion.MaxHealth > 3 && minion.IsTargetable);
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

        #endregion
    }
}
