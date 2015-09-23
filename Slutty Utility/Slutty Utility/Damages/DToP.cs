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
        public static readonly SpellSlot[] Slots =
        {
            SpellSlot.Q,
            SpellSlot.E,
            SpellSlot.W,
            SpellSlot.R
        };
        public static bool isReadyPerfectly(Spell spell)
        {
            return spell != null && spell.Slot != SpellSlot.Unknown && spell.Instance.State != SpellState.Cooldown &&
                   spell.Instance.State != SpellState.Disabled && spell.Instance.State != SpellState.NoMana &&
                   spell.Instance.State != SpellState.NotLearned && spell.Instance.State != SpellState.Surpressed &&
                   spell.Instance.State != SpellState.Unknown && spell.Instance.State == SpellState.Ready;
        }
       public static void OnLoad()
        {
            Drawing.OnDraw += OnDraw;

            DamageToUnit = ComboCalc;
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
                   Drawing.OnDraw += OnDraw;
               }
               _damageToUnit = value;
           }
       }


        public static float ComboCalc(Obj_AI_Hero Players)
        {
            var damage = 0d;
            var target = TargetSelector.SelectedTarget;
            if (target == null) return 0;

            foreach (var spell in Slots)
            {
                var expires = (target.Spellbook.GetSpell(spell).CooldownExpires);
                var CD =
                    (int)
                        (expires -
                         (Game.Time - 1));

                if (CD <= 0 &&
                    GetBool("damagesmenu.dtop" + spell, typeof (bool)))
                {
                    damage += target.GetSpellDamage(Players, spell);
                }
            }
            return (float)damage;
        }

        private static void OnDraw(EventArgs args)
        {
            if (!GetBool("dtop.selectedtarget", typeof (bool)))
            {
                return;
            }


            var barPos = Player.HPBarPosition;
            var damage = DamageToUnit(Player);
            var percentHealthAfterDamage = Math.Max(0, Player.Health - damage) / Player.MaxHealth;
            var yPos = barPos.X + YOffset;
            var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
            var xPosCurrentHp = barPos.X + XOffset + Width * Player.Health / Player.MaxHealth;

            if (damage > Player.Health)
            {
                Text.X = (int) barPos.X + XOffset;
                Text.Y = (int) barPos.Y + YOffset - 13;
                Text.text = "Killable With Combo Rotation " + (Player.Health - damage);
                Text.OnEndScene();
            }
            Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);

            var differenceInHp = xPosCurrentHp - xPosDamage;
            var pos1 = barPos.X  + 9 +(107 * percentHealthAfterDamage);
            for (var i = 0; i < differenceInHp; i++)
            {
                Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
            }
        }
    }
}

//greets the the random pleb suicidecarl @Leaguesharp
