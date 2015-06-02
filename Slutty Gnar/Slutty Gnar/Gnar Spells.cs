using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;

namespace Slutty_Gnar
{
    public static class Gnar_Spells
    {

        private static Obj_AI_Hero player = ObjectManager.Player;
        public static Spell QMini { get; private set; }
        public static Spell WMini { get; private set; }
        public static Spell EMini { get; private set; }
        public static Spell RMini { get; private set; }

        public static Spell QMega { get; private set; }
        public static Spell WMega { get; private set; }
        public static Spell EMega { get; private set; }
        public static Spell RMega { get; private set; }
        public static Spell SummonerDot;

        public static Spell Q
        {
            get { return player.IsMiniGnar() ? QMini : QMega; }
        }

        public static Spell W
        {
            get { return player.IsMiniGnar() ? WMini : WMega; }
        }

        public static Spell E
        {
            get { return player.IsMiniGnar() ? EMini : EMega; }
        }

        public static Spell R
        {
            get { return player.IsMiniGnar() ? RMini : RMega; }
        }

        private static float lastCastedStun = 0;

        public static bool HasCastedStun
        {
            get { return Game.Time - lastCastedStun < 0.25; }
        }

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

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (sender.Owner.IsMe && player.IsMegaGnar())
            {
                switch (args.Slot)
                {
                    case SpellSlot.W:
                    case SpellSlot.R:

                        lastCastedStun = Game.Time;
                        break;
                }
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
    }
}
