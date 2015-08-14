using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Gnar_Reworked
{
    public static class GnarSpells
    {

        private static Obj_AI_Hero player = ObjectManager.Player;
        public static Spell QMega,QnMega, WMega, EMega, RMega;
        public static Spell QMini, QnMini, WMini, EMini, RMini;
        public static Spell SummonerDot;

        private static float lastCastedStun;

        public static bool HasCastedStun
        {
            get { return Game.Time - lastCastedStun < 0.25; }
        }

        static GnarSpells()
        {
            QMini = new Spell(SpellSlot.Q, 1050);
            QnMini = new Spell(SpellSlot.Q, 1050);
            WMini = new Spell(SpellSlot.W);
            EMini = new Spell(SpellSlot.E, 475);
            RMini = new Spell(SpellSlot.R);


            QMega = new Spell(SpellSlot.Q, 1050);
            QnMega = new Spell(SpellSlot.Q, 1050);
            WMega = new Spell(SpellSlot.W, 500);
            EMega = new Spell(SpellSlot.E, 475);

            RMega = new Spell(SpellSlot.R, 420);

            QMini.SetSkillshot(0.25f, 60, 1200, true, SkillshotType.SkillshotLine);
            QnMini.SetSkillshot(0.25f, 60, 1200, false, SkillshotType.SkillshotLine);
            EMini.SetSkillshot(0.5f, 150, float.MaxValue, false, SkillshotType.SkillshotCircle);

            QMega.SetSkillshot(0.25f, 80, 1200, true, SkillshotType.SkillshotLine);
            QnMega.SetSkillshot(0.25f, 80, 1200, false, SkillshotType.SkillshotLine);
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
        public static bool IsMiniGnar(this Obj_AI_Hero target)
        {
            return target.CharData.BaseSkinName == "Gnar";
        }

        public static bool IsMegaGnar(this Obj_AI_Hero target)
        {
            return target.CharData.BaseSkinName == "gnarbig";
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
