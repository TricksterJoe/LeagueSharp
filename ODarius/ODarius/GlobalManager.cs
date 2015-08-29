using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace ODarius
{
    internal class GlobalManager : Darius
    {
        // this for later XD
        public static int TickCount(Obj_AI_Hero t)
        {
            var buff = t.Buffs.FirstOrDefault(x => x.Name == "dariushemo");
            return buff != null ? buff.Count : 0;
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
                    Drawing.OnDraw += DrawingManager.Drawing_OnDrawChamp;
                }
                _damageToUnit = value;
            }
        }

        private static float IgniteDamage(Obj_AI_Base target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static float GetComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0d;
            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (R.IsReady() && Player.Mana >= R.Instance.ManaCost)
                damage += Player.GetSpellDamage(enemy, SpellSlot.R); //* RCount();

            if (Ignite.IsReady())
                damage += IgniteDamage(enemy);

            /*
             if (Q.Instance.ManaCost + W.Instance.ManaCost
                 + E.Instance.ManaCost + R.Instance.ManaCost <= Player.Mana)
                 damage += Q.GetDamage(enemy) + W.GetDamage(enemy) + E.GetDamage(enemy) + (R.GetDamage(enemy)*RCount());
             */

            return (float)damage;
        }

    }
}
