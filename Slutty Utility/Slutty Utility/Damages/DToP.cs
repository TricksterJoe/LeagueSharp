using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Damages
{
    class DtoP : Helper
    {
        public DtoP()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }
        public static float RComboCalc(Obj_AI_Hero enemy)
        {
            var damage = 0d;

            if (enemy.Spellbook.GetSpell(SpellSlot.Q).IsReady() &&
                enemy.Spellbook.GetSpell(SpellSlot.Q).ManaCost <= Player.Mana)
                damage += enemy.GetSpellDamage(enemy, SpellSlot.Q);

            if (enemy.Spellbook.GetSpell(SpellSlot.E).IsReady() &&
                enemy.Spellbook.GetSpell(SpellSlot.E).ManaCost <= Player.Mana)
                damage += enemy.GetSpellDamage(enemy, SpellSlot.E);

            if (enemy.Spellbook.GetSpell(SpellSlot.W).IsReady() &&
                enemy.Spellbook.GetSpell(SpellSlot.W).ManaCost <= Player.Mana)
                damage += enemy.GetSpellDamage(enemy, SpellSlot.W);

            if (enemy.Spellbook.GetSpell(SpellSlot.R).IsReady() &&
                enemy.Spellbook.GetSpell(SpellSlot.R).ManaCost <= Player.Mana)
                damage += enemy.GetSpellDamage(enemy, SpellSlot.R);

            return (float)damage;
        }
        private static void OnLoad(EventArgs args)
        {
            Drawing.OnDraw += OnDraw;
        }

        private static void OnDraw(EventArgs args)
        {
            var target = TargetSelector.GetSelectedTarget();
            if (target == null)
                return;
        }
    }
}
