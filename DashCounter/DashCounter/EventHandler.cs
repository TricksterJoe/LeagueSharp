using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;

namespace DashCounter
{
    class EventHandler
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu Config;
        public const string Menuname = "Dash Counter";
        public static void AddBool(Menu menu, string displayName, string name, bool value = true)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(value));
        }

        internal static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
            AddBool(Config, "Enable", "enable");

            Game.OnUpdate += OnUpdate;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            CustomEvents.Unit.OnDash += Ondash;
        }

        public static void ReadyCast(float range, SpellSlot slot, Vector3 position = new Vector3(), bool targetted = false)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            if (!spell.IsReady()) return;

            spellbook.CastSpell(slot, false);
        }

        public static void Ondash(Obj_AI_Base sender, Dash.DashItem args)
        {
            foreach (var spellData in Database.Spells)
            {
                if (Player.ChampionName == spellData.championName)
                {
                    if (args.EndPos.Distance(Player.Position) <= spellData.range)
                    {
                        ReadyCast(spellData.range, spellData.slot, new Vector3(args.EndPos.X, args.EndPos.Y, 0), false);
                    }
                }
            }
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //
        }

        private static void OnUpdate(EventArgs args)
        {
           //
        }
    }
}
