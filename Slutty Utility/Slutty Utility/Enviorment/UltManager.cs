using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Utility.Enviorment
{
    class UltManager : Helper
    {
        public static Orbwalking.Orbwalker Orbwalker;
        private static int lastr;
        private static bool _defaultonbutton;
        public UltManager()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            Spellbook.OnCastSpell += OnCastspell;
        }

        private static void OnCastspell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            string[] champions =
            {
                "Fiddlesticks", "Janna",
                "Malzahar", "Katarina",
                "Nunu"
            };

            var ezevade = Menu.GetMenu("EzEvade", "evadeonbutton");

            _defaultonbutton = ezevade.Item("evadeonbutton").GetValue<bool>();

            for (var i = 0; i >= 6; i++)
            {
                if (sender.Owner.Name == champions[i]) continue;

                if (args.Slot == SpellSlot.R)
                {
                    lastr = Environment.TickCount;
                    Orbwalker.SetAttack(false);
                    Orbwalker.SetMovement(false);
                    if (_defaultonbutton)
                    {
                        ezevade.Item("lol").SetValue(false);
                    }

                }

                if (Environment.TickCount - lastr >= Player.Spellbook.GetSpell(SpellSlot.E).SData.SpellCastTime)
                {
                    Orbwalker.SetAttack(true);
                    Orbwalker.SetMovement(true);
                    if (_defaultonbutton)
                    {
                        ezevade.Item("lol").SetValue(true);
                    }
                }
            }
        }
    }
}
