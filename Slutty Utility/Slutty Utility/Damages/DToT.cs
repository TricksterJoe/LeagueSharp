using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility.MenuConfig;

namespace Slutty_Utility.Damages
{
    class DtoT : Helper
    {

        public static void OnLoad()
        {
            Drawing.OnDraw += OnDraw;
        }

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red);
        private static readonly Color _color = Color.Red;
        private static readonly Color _fillColor = Color.Blue;
        private static Utility.HpBarDamageIndicator.DamageToUnitDelegate _damageToUnit;
        public static bool EnableDrawingDamage { get; set; }
        public static Color DamageFillColor { get; set; }

        public static float GetComboDamage(Obj_AI_Hero enemy)
        {
            var damage = 0d;
            if (Player.Spellbook.GetSpell(SpellSlot.Q).IsReady() &&
            GetBool("damagesmenu.dtop" + DamagesMenu.Slots[0], typeof (bool)))
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (Player.Spellbook.GetSpell(SpellSlot.E).IsReady() &&
                GetBool("damagesmenu.dtop" + DamagesMenu.Slots[1], typeof (bool))) 
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (Player.Spellbook.GetSpell(SpellSlot.W).IsReady() &&
                GetBool("damagesmenu.dtop" + DamagesMenu.Slots[2], typeof (bool))) 
                damage += Player.GetSpellDamage(enemy, SpellSlot.W);

            if (Player.Spellbook.GetSpell(SpellSlot.R).IsReady() &&
                GetBool("damagesmenu.dtop" + DamagesMenu.Slots[3], typeof (bool))) 
                damage += Player.GetSpellDamage(enemy, SpellSlot.R);

            return (float)damage;
        }

        public static Utility.HpBarDamageIndicator.DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += OnDraw;
                }
                _damageToUnit = value;
            }
        }

        private static void OnDraw(EventArgs args)
        {

            var target = TargetSelector.GetSelectedTarget();
            if (target == null)
                return;
            if (!GetBool("dtot.damage", typeof(bool)))
            {
                return;
            }
            var barPos = target.HPBarPosition;
            var damage = DamageToUnit(target);
            var percentHealthAfterDamage = Math.Max(0, target.Health - damage) / target.MaxHealth;
            var yPos = barPos.Y + YOffset;
            var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
            var xPosCurrentHp = barPos.X + XOffset + Width * target.Health / target.MaxHealth;


            Text.X = (int)barPos.X + XOffset;
            Text.Y = (int)barPos.Y + YOffset - 13;
            Text.text = damage > target.Health
                ? "Killable With Combo Rotation " + (target.Health - damage)
                : (target.Health - damage).ToString(CultureInfo.CurrentCulture);
            Text.OnEndScene();

            Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

            var differenceInHp = xPosCurrentHp - xPosDamage;
            var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);
            for (var i = 0; i < differenceInHp; i++)
            {
                Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
            }
        }

    }
}
