using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace OAnnie
{
    class GlobalManager : Annie
    {
        private static DamageToUnitDelegate _damageToUnit;
        public static bool EnableDrawingDamage { get; set; }
        public static System.Drawing.Color DamageFillColor { get; set; }
        public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);


        public static float GetComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0d;
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            if (W.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);
            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);

            return (float)damage;
        }



        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
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
    }
}
