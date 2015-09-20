using System;
using System.Drawing;
using System.Globalization;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_Utility.MenuConfig;

namespace Slutty_Utility.Damages
{
    internal class DtoP : Helper
    {
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
        
       public static void OnLoad()
        {
            Drawing.OnDraw += OnDraw;
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

        public static float ComboCalc()
        {
            var damage = 0d;
            var target = TargetSelector.GetSelectedTarget();
            if (target != null)
            {
                    if (target.Spellbook.GetSpell(SpellSlot.Q).IsReady() &&
                        GetBool("damagesmenu.dtop" + DamagesMenu.Slots[0], typeof(bool)) &&
                        target.Spellbook.GetSpell(SpellSlot.Q).ManaCost <= Player.Mana)
                        damage += target.GetSpellDamage(Player, SpellSlot.E);

                    if (target.Spellbook.GetSpell(SpellSlot.E).IsReady() &&
                        GetBool("damagesmenu.dtop" + DamagesMenu.Slots[1], typeof(bool)) &&
                        target.Spellbook.GetSpell(SpellSlot.E).ManaCost <= Player.Mana)
                        damage += target.GetSpellDamage(Player, SpellSlot.E);

                    if (target.Spellbook.GetSpell(SpellSlot.W).IsReady() &&
                        GetBool("damagesmenu.dtop" + DamagesMenu.Slots[2], typeof(bool)) &&
                        target.Spellbook.GetSpell(SpellSlot.W).ManaCost <= Player.Mana)
                        damage += target.GetSpellDamage(Player, SpellSlot.W);

                    if (target.Spellbook.GetSpell(SpellSlot.R).IsReady() &&
                        GetBool("damagesmenu.dtop" + DamagesMenu.Slots[3], typeof(bool)) &&
                        target.Spellbook.GetSpell(SpellSlot.R).ManaCost <= Player.Mana)
                        damage += target.GetSpellDamage(Player, SpellSlot.R);

                    return (float) damage;
            }
            return 0;
        }

        private static void OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetSelectedTarget();
            if (target == null)
                return;
            if (!GetBool("dtop.selectedtarget", typeof (bool)))
            {
                return;
            }
            var barPos = Player.HPBarPosition;
            var damage = DamageToUnit(Player);
            var percentHealthAfterDamage = Math.Max(0, Player.Health - damage) / Player.MaxHealth;
            var yPos = barPos.Y + YOffset;
            var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
            var xPosCurrentHp = barPos.X + XOffset + Width * Player.Health / Player.MaxHealth;


                Text.X = (int)barPos.X + XOffset;
                Text.Y = (int)barPos.Y + YOffset - 13;
                Text.text = damage > Player.Health
                    ? "Killable With Combo Rotation " + (Player.Health - damage)
                    : (Player.Health - damage).ToString(CultureInfo.CurrentCulture);               
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
