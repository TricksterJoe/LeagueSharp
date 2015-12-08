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
        public static void OnLoaded()
        {
            //    if (Player.ChampionName != "LeeSin") return;
            MenuConfig.OnLoad();
            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 375);

            _flashSlot = ObjectManager.Player.GetSpellSlot("summonerflash");

            foreach (var spell in Player.Spellbook.Spells.Where(spell => spell.Name.ToLower().Contains("smite")))
            {
                Smite = spell.Slot;
            }

            Notifciations.Messages();
            Misc.VersionCheck.UpdateCheck();

            Q.SetSkillshot(0.25f, 65f, 1800f, true, SkillshotType.SkillshotLine);

            Game.OnUpdate += OnUpdate.OnUpdated;
            Drawing.OnDraw += OnInsec.OnDraw;
            Drawing.OnDraw += OnJungle.OnCamps;
            Drawing.OnDraw += OnChamp.OnSpells;
            GameObject.OnCreate += EventHandler.OnCreate;
            Obj_AI_Base.OnProcessSpellCast += EventHandler.OnSpellcast;
            Spellbook.OnCastSpell += EventHandler.OnSpell;
            Game.OnWndProc += EventHandler.OnWndProc;
        }
    }
}
