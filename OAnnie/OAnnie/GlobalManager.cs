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


        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (Q.IsReady() || Player.Mana <= Q.Instance.ManaCost)
                return Q.GetDamage(enemy);

            if (E.IsReady() || Player.Mana <= R.Instance.ManaCost)
                return E.GetDamage(enemy);

            if (W.IsReady() || Player.Mana <= W.Instance.ManaCost )
                return W.GetDamage(enemy);

            return 0;
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
