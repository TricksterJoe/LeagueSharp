using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Tristana
{
    class Tristana
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        internal static void OnLoad(EventArgs args)
        {
            E = new Spell(SpellSlot.Q, 700);
            R = new Spell(SpellSlot.R, 700);
            E.SetTargetted(0, 1300);
            MenuHandler.OnLoad();
            DamageHandler.DamageToUnit = DamageHandler.EDamage;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += DrawManager.Drawing_OnDraw;
        }

        private static void OnUpdate(EventArgs args)
        {
            foreach (var buff in HeroManager.Enemies)
            { 
                foreach (var buffs in buff.Buffs)
                {
                    if (!buffs.Name.Contains("yasuo") && !buffs.Name.Contains("odin"))
                    Game.PrintChat(buffs.Name);
                }
            }
            ECast();
        }

        private static void ECast()
        {
            var target = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            if (target != null && R.IsReady())
            {
                if (DamageHandler.EDamage(target) + R.GetDamage(target)> target.Health)
                {
                    R.Cast(target);
                }
            }
        }
    }
}
