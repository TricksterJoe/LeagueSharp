using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;

namespace Slutty_Tristana
{
    class DamageHandler
    {
        public static int[] abilitySequence;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static int GetPassiveBuff
        {
            get
            {
                var data = Player.Buffs.FirstOrDefault(b => b.DisplayName == "TristanaECharge");
                return data != null ? data.Count : 0;
            }
        }
        public static float EDamage(Obj_AI_Hero unit)
        {
            float damage = 0;
            if (unit == null) return 0;
            var data = Player.Buffs.FirstOrDefault(b => b.DisplayName == "TristanaECharge");
                var i = data != null ? data.Count : 0;
            
            var buff = unit.GetBuff("TristanaECharge").Count;
            if (!unit.HasBuff("tristanaechargesound"))
                return 0;
            if (i != 0)
            {
                damage += (float) (Tristana.E.GetDamage(unit)*((0.3*(i + 1)))
                                   + (Player.TotalMagicalDamage*0.5));
                damage += BonusDamagePerStack(unit)*(i + 1);
            }

            return damage;
        }


        public static float BonusDamagePerStack(Obj_AI_Hero unit)
        {
            if (unit == null) return 0;
            return (float) Player.CalcDamage(unit, Damage.DamageType.Physical,
                new[] {18, 21, 24, 27, 30}[Tristana.E.Level - 1] +
                new[]
                {
                    Player.BaseAttackDamage*0.15, Player.BaseAttackDamage*0.195, Player.BaseAttackDamage*0.24,
                    Player.BaseAttackDamage*0.285, Player.BaseAttackDamage*0.33
                }[Tristana.E.Level] + Player.FlatMagicDamageMod*0.15);
        }

        private static DamageToUnitDelegate _damageToUnit;
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }

        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);



        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += DrawManager.Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

    }
}
