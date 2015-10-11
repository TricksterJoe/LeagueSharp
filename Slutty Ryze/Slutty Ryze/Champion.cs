using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    class Champion
    {
        #region Variable Declaration
        private static SpellSlot _ignite;
        public static readonly Obj_AI_Hero Player = ObjectManager.Player;
        private const string _champName = "Ryze";
        //private static Spell _q, _w, _e, _r, _qn;
        // Does not work as a property o-o
        public static Spell Q, W, E, R, Qn;
        public static bool casted;
        private static int lastprocess;
        #endregion
        #region Public Properties
        //public static Spell Q
        //{
        //    get { return _q; }
        //    set { _q = value; }
        //}

        //public static Spell QN
        //{
        //    get { return _qn; }
        //    set { _qn = value; }
        //}
        //public static Spell W
        //{
        //    get { return _w; }
        //    set { _w = value; }
        //}
        //public static Spell WE
        //{
        //    get { return _e; }
        //    set { _e = value; }
        //}
        //public static Spell R
        //{
        //    get { return _r; }
        //    set { _r = value; }
        //}
        public static string ChampName
        {
            get
           {
                return _champName;
           }
        }
        #endregion
        #region Public Functions
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


        public static void AutoPassive()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (GlobalManager.GetHero.Mana < GlobalManager.Config.Item("ManapSlider").GetValue<Slider>().Value) return;

            //Maybe check if any minons can be killed?
            //foreach(var minion in minions)
            //if(minion.Headth < Champion.Q.GetDamage)
            //break;
            
           if (GlobalManager.GetHero.IsRecalling()) return;

           if (minions.Count >= 1) return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target != null) return;

            var stackSliders = GlobalManager.Config.Item("stackSlider").GetValue<Slider>().Value;
            if (GlobalManager.GetHero.InFountain()) return;

            if (GlobalManager.GetPassiveBuff >= stackSliders)
                return;

            if (Utils.TickCount - Q.LastCastAttemptT >=
                GlobalManager.Config.Item("autoPassiveTimer").GetValue<Slider>().Value * 1000 - (100 + (Game.Ping/2)) &&
                Q.IsReady())
            {
                if (!Game.CursorPos.IsZero)
                    Q.Cast(Game.CursorPos);
                else
                    Q.Cast();
            }
            Console.WriteLine(Game.Ping);

        }

        public static void RyzeInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wSpell = GlobalManager.Config.Item("useW2I").GetValue<bool>();
            if (!wSpell) return;
            W.CastOnUnit(target);
        }

//        public static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
//        {
//            if (!sender.IsEnemy) return;
//
//            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);
//            var qSpell = GlobalManager.Config.Item("useQW2D").GetValue<bool>();
//
//            if (sender.NetworkId != target.NetworkId) return;
//            if (!qSpell) return;
//            if (!Champion.Q.IsReady() || !(args.EndPos.Distance(GlobalManager.GetHero) < Champion.Q.Range)) return;
//            var delay = (int)(args.EndTick - Game.Time - Champion.Q.Delay - 0.1f);
//
//            if (delay > 0)
//                Utility.DelayAction.Add(delay * 1000, () => Champion.Q.Cast(args.EndPos));
//            else
//                Champion.Q.Cast(args.EndPos);
//
//            if (!Champion.Q.IsReady() || !(args.EndPos.Distance(GlobalManager.GetHero) < Champion.Q.Range)) return;
//
//            if (delay > 0)
//                Utility.DelayAction.Add(delay * 1000, () => Champion.Q.Cast(args.EndPos));
//            else
//                Champion.W.CastOnUnit(target);
//        }
        public static void AABlock()
        {
                MenuManager.Orbwalker.SetAttack(!GlobalManager.Config.Item("AAblock").GetValue<bool>());
        }

        public static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget() || target.IsInvulnerable)
                return;

            var qSpell = GlobalManager.Config.Item("useQ2KS").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("useW2KS").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("useE2KS").GetValue<bool>();
            if (qSpell
                && Q.GetDamage(target) > target.Health
                && target.IsValidTarget(Q.Range))
                Q.Cast(target);

            if (wSpell
                && W.GetDamage(target) > target.Health
                && target.IsValidTarget(W.Range))
                W.CastOnUnit(target);

            if (eSpell
                && E.GetDamage(target) > target.Health
                && target.IsValidTarget(E.Range))
                E.CastOnUnit(target);
        }

        public static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (W.IsReady() && W.Level > 0 && MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                args.Process = false;
            }

            var mura = GlobalManager.Config.Item("muramana").GetValue<bool>();

            if (!mura) return;

            var muramanai = Items.HasItem(ItemManager.Muramana) ? 3042 : 3043;

            if (!args.Target.IsValid<Obj_AI_Hero>() || !args.Target.IsEnemy || !Items.HasItem(muramanai) ||
                !Items.CanUseItem(muramanai))
                return;

            if (!GlobalManager.GetHero.HasBuff("Muramana"))
                Items.UseItem(muramanai);
        }

        #endregion


        internal static void OnProcess(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
          //  if (sender.SpellWasCast)
//            {
//                if (args.Slot == SpellSlot.W
//                    || args.Slot == SpellSlot.Q
//                    || args.Slot == SpellSlot.E)
//                {
//                    casted = true;
//                    MenuManager.Orbwalker.SetMovement(false);
//                    MenuManager.Orbwalker.SetAttack(false);
//                }
//                if (casted)
//                {
//                    Utility.DelayAction.Add(10000, () => MenuManager.Orbwalker.SetMovement(true));
//                    Utility.DelayAction.Add(400, () => MenuManager.Orbwalker.SetMovement(true));
//                    casted = false;
//                }
//            }
        }
        /*
        internal static void OnOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (casted)
            {
                if (args.Order == GameObjectOrder.AttackUnit
                    && Environment.TickCount - lastprocess >= 1000 + Game.Ping)
                {
                    args.Process = false;
                    lastprocess = Environment.TickCount;
                }
            }
        }
         */
    }
}
