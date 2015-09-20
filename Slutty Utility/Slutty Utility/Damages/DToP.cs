using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
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
            _damageToUnit = ComboCalc;
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

        public static float ComboCalc(Obj_AI_Hero targets)
        {
            var damage = 0d;
            if (targets.Spellbook.GetSpell(SpellSlot.Q).IsReady())
            {
//&&
                // GetBool("damagesmenu.dtop" + DamagesMenu.Slots[0], typeof(bool)))
                damage += Player.GetSpellDamage(targets, SpellSlot.Q);
                
            }

            if (targets.Spellbook.GetSpell(SpellSlot.E).IsReady())
            {
//&&
                // GetBool("damagesmenu.dtop" + DamagesMenu.Slots[1], typeof(bool)))
                damage += Player.GetSpellDamage(targets, SpellSlot.E);
            }

            if (targets.Spellbook.GetSpell(SpellSlot.W).IsReady())
            {
            // &&
                // GetBool("damagesmenu.dtop" + DamagesMenu.Slots[2], typeof(bool)))
                damage += Player.GetSpellDamage(targets, SpellSlot.W);
            }

            if (targets.Spellbook.GetSpell(SpellSlot.R).IsReady()) // &&
            {
                //GetBool("damagesmenu.dtop" + DamagesMenu.Slots[3], typeof(bool)))
                damage += Player.GetSpellDamage(targets, SpellSlot.R);
            }

            return (float) damage;
        }

        private static void OnDraw(EventArgs args)
        {
//            if (!GetBool("dtop.selectedtarget", typeof (bool)))
//            {
//                return;
//            }
                var target = TargetSelector.SelectedTarget;
                if (target != null)
                {
                    var barPos = Player.HPBarPosition;
                    var damage = DamageToUnit(Player);
                    var percentHealthAfterDamage = Math.Max(0, Player.Health - damage)/Player.MaxHealth;
                    var yPos = barPos.Y + YOffset;
                    var xPosDamage = barPos.X + XOffset + Width*percentHealthAfterDamage;
                    var xPosCurrentHp = barPos.X + XOffset + Width*Player.Health/Player.MaxHealth;

                    if (damage > Player.Health)
                    {
                        Text.X = (int) barPos.X + XOffset;
                        Text.Y = (int) barPos.Y + YOffset - 13;
                        Text.text = "Killable With Combo Rotation " + (Player.Health - damage);
                        Text.OnEndScene();
                    }
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
}
