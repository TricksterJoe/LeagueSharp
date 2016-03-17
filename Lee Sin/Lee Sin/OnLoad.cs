using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Lee_Sin.Drawings;
using Lee_Sin.Misc;

namespace Lee_Sin
{
    class OnLoad : LeeSin
    {
        public static SebbyLib.Prediction.PredictionInput PredictionRnormal;
        public static void OnLoaded()
        {
               if (Player.ChampionName != "LeeSin") return;
            MenuConfig.OnLoad();
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 375);
            Rnormal = new Spell(SpellSlot.R, 700);
            FlashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");
            Rnormal.SetSkillshot(0f, 70f, 1500f, false,(LeagueSharp.Common.SkillshotType) SkillshotType.SkillshotLine);
            PredictionRnormal = new SebbyLib.Prediction.PredictionInput
            {
                Aoe = true,
                Collision = false,
                Speed = Rnormal.Speed,
                Delay = Rnormal.Delay,
                Range = Rnormal.Range,
                Radius = Rnormal.Width,
                Type = SebbyLib.Prediction.SkillshotType.SkillshotLine
            };
            foreach (var spell in Player.Spellbook.Spells.Where(spell => spell.Name.ToLower().Contains("smite")))
            {
                Smite = spell.Slot;
            }

            Notifciations.Messages();
            Misc.VersionCheck.UpdateCheck();

            Q.SetSkillshot(0.25f, 58f, 1800f, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
            Game.OnUpdate += OnUpdate.OnUpdated;
            Drawing.OnDraw += OnInsec.OnDraw;
            Drawing.OnDraw += OnJungle.OnCamps;
            Drawing.OnDraw += OnChamp.OnSpells;
            GameObject.OnCreate += EventHandler.OnCreate;
            Obj_AI_Base.OnProcessSpellCast += EventHandler.OnSpellcast;
            Spellbook.OnCastSpell += EventHandler.OnSpell;
            Game.OnWndProc += EventHandler.OnWndProc;
           // Obj_AI_Base.OnDoCast += EventHandler.OnDoCast;
        }
    }
}
