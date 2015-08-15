using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Gnar
{
    public static class Gnar_Spells
    {
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell SummonerDot;
        private static float _lastCastedStun;

        static Gnar_Spells()
        {
            QMini = new Spell(SpellSlot.Q, 1100);
            WMini = new Spell(SpellSlot.W);
            EMini = new Spell(SpellSlot.E, 475);
            RMini = new Spell(SpellSlot.R);

            QMega = new Spell(SpellSlot.Q, 1100);
            WMega = new Spell(SpellSlot.W, 525);
            EMega = new Spell(SpellSlot.E, 475);

            RMega = new Spell(SpellSlot.R, 420);

            QMini.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
            EMini.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);

            QMega.SetSkillshot(0.25f, 80, 1200, true, SkillshotType.SkillshotLine);
            WMega.SetSkillshot(0.25f, 80, float.MaxValue, false, SkillshotType.SkillshotLine);
            EMega.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);
            RMega.Delay = 0.25f;

            SummonerDot = new Spell(ObjectManager.Player.GetSpellSlot("SummonerDot"), 550);
            SummonerDot.SetTargetted(0.1f, float.MaxValue);


            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
        }

        public static Spell QMini { get; private set; }
        public static Spell WMini { get; private set; }
        public static Spell EMini { get; private set; }
        public static Spell RMini { get; private set; }
        public static Spell QMega { get; private set; }
        public static Spell WMega { get; private set; }
        public static Spell EMega { get; private set; }
        public static Spell RMega { get; private set; }

        public static Spell Q
        {
            get { return Player.IsMiniGnar() ? QMini : QMega; }
        }

        public static Spell W
        {
            get { return Player.IsMiniGnar() ? WMini : WMega; }
        }

        public static Spell E
        {
            get { return Player.IsMiniGnar() ? EMini : EMega; }
        }

        public static Spell R
        {
            get { return Player.IsMiniGnar() ? RMini : RMega; }
        }

        public static bool HasCastedStun
        {
            get { return Game.Time - _lastCastedStun < 0.25; }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (!sender.Owner.IsMe || !Player.IsMegaGnar())
                return;
            switch (args.Slot)
            {
                case SpellSlot.W:
                case SpellSlot.R:

                    _lastCastedStun = Game.Time;
                    break;
            }
        }

        public static Spell GetSpellFromSlot(SpellSlot slot)
        {
            return slot == SpellSlot.Q
                ? Q
                : slot == SpellSlot.W ? W : slot == SpellSlot.E ? E : slot == SpellSlot.R ? R : null;
        }

        public static bool IsMiniSpell(this Spell spell)
        {
            return
                spell.Equals(QMini) ||
                spell.Equals(WMini) ||
                spell.Equals(EMini) ||
                spell.Equals(RMini);
        }

        public static bool IsAboutToTransform(this Obj_AI_Hero target)
        {
            return target.IsMiniGnar() &&
                   (target.Mana == target.MaxMana
                    && (target.HasBuff("gnartransformsoon")
                        || target.HasBuff("gnartransform")))
                   || target.IsMegaGnar() && target.ManaPercent <= 0.1;
        }

        public static MinionManager.FarmLocation? GetFarmLocation(this Spell spell, MinionTeam team = MinionTeam.Enemy,
            List<Obj_AI_Base> targets = null)
        {
            targets = MinionManager.GetMinions(spell.Range, MinionTypes.All, team, MinionOrderTypes.MaxHealth);
            if (!spell.IsSkillshot || targets.Count == 0)
                return null;
            var positions = MinionManager.GetMinionsPredictedPositions(targets, spell.Delay, spell.Width, spell.Speed,
                spell.From, spell.Range, spell.Collision, spell.Type);
            var farmLocation = MinionManager.GetBestLineFarmLocation(positions, spell.Width, spell.Range);
            if (farmLocation.MinionsHit == 0)
                return null;
            return farmLocation;
        }
    }
}