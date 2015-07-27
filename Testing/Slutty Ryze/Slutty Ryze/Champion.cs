using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    class Champion
    {
        public const string ChampName = "Ryze";
        public const string Menuname = "Slutty Ryze";
        public static Spell Q, W, E, R, Qn;
        private static SpellSlot _ignite;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static float IgniteDamage(Obj_AI_Hero target)
        {
            if (_ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(_ignite) != SpellState.Ready)
                return 0f;
            return (float)Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }

        public static SpellSlot GetIgniteSlot()
        {
            return _ignite;
        }

        public static void SetIgniteSlot(SpellSlot nSpellSlot)
        {
            _ignite = nSpellSlot;
        }

        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (Q.IsReady() || Player.Mana <= Q.Instance.ManaCost * 5)
                return Q.GetDamage(enemy) * 5;

            if (E.IsReady() || Player.Mana <= E.Instance.ManaCost * 5)
                return E.GetDamage(enemy) * 5;

            if (W.IsReady() || Player.Mana <= W.Instance.ManaCost * 3)
                return W.GetDamage(enemy) * 3;

            return 0;
        }
    }
}
