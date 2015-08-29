using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace ODarius
{
    internal class Darius : MenuConfig
    {
        public const string ChampName = "Darius";
        public static Spell Q, W, E, R;
        public static SpellSlot Ignite;
        public static SpellSlot FlashSlot;
        public static float FlashRange = 450f;

        internal static void Load(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;

            Q = new Spell(SpellSlot.Q, 420);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 530);
            R = new Spell(SpellSlot.R, 450);

            GlobalManager.DamageToUnit = GlobalManager.GetComboDamage;

            E.SetSkillshot(0.25f, 80, int.MaxValue, false, SkillshotType.SkillshotCone);
            Q.SetSkillshot(0.75f, 42.5f, int.MaxValue, false, SkillshotType.SkillshotCircle);
            LoadMenu();
            Orbwalking.AfterAttack += After_Attack;
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += DrawingManager.Drawing_OnDrawChamp;
            Drawing.OnDraw += DrawingManager.Drawing_OnDraw;
            Obj_AI_Base.OnProcessSpellCast += OnCast;
        }

        private static void After_Attack(AttackableUnit unit, AttackableUnit target)
        {
            var targets = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var usew = Config.Item("comboMenu.usew").GetValue<bool>();
            if (targets == null)
                return;
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear
                || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                
                if (usew && target.IsValidTarget(Player.AttackRange) && W.IsReady())
                {
                    W.Cast();
                }
            }
        }

        private static void OnCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) 
                return;
          //  Game.PrintChat(args.SData.Name);
            if (args.SData.Name == "DariusCleave")
            {
                Orbwalking.ResetAutoAttackTimer();
            }

        }


        private static void Game_OnUpdate(EventArgs args)
        {
            /*
            foreach (var buffs in Player.Buffs)
            {
                if (buffs.Name != "odinplayerbuff"
                    || buffs.Name != "dariushemomarker"
                    || buffs.Name != "dariusaxegrabarmorpen")
                {
                    Game.PrintChat(buffs.Name);
                }
            }
             */
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                   Harass();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    Jungleclear();
                    break;
            }

        }

        private static void Jungleclear()
        {
            var jungle =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral,
                    MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (jungle == null)
                return;

            var useq = Config.Item("jungleMenu.useq").GetValue<bool>();
            var usew = Config.Item("jungleMenu.usew").GetValue<bool>();
            var manaslider = Config.Item("jungleMenu.manaslider").GetValue<Slider>().Value;

            if (Player.ManaPercent < manaslider)
                return;

            if (Q.IsReady() && useq && !Player.IsWindingUp)
            {
                Q.Cast(jungle);
            }

            if (W.IsReady() && usew && jungle.Distance(Player) <= Player.AttackRange)
            {
                W.Cast();
            }
        }

        private static void LaneClear()
        {
            var minion =
    MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
        MinionOrderTypes.MaxHealth).FirstOrDefault();
            var minions =
                MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Enemy,
                    MinionOrderTypes.MaxHealth);

            if (minion == null)
                return;
            var useq = Config.Item("laneMenu.useq").GetValue<bool>();
            var usew = Config.Item("laneMenu.usew").GetValue<bool>();
            var qslider = Config.Item("laneMenu.minminions").GetValue<Slider>().Value;
            var manaslider = Config.Item("laneMenu.manaslider").GetValue<Slider>().Value;

            if (Player.ManaPercent < manaslider)
                return;

            if (Q.IsReady() && useq && !Player.IsWindingUp)
            {
                if (minions.Count >= qslider)
                {
                    Q.Cast(minions[0]);
                }
            }

            if (W.IsReady() && usew && minion.Distance(Player) <= Player.AttackRange)
            {
                W.Cast();
            }
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var useq = Config.Item("harrasMenu.useq").GetValue<bool>();
            var usee = Config.Item("harrasMenu.usee").GetValue<bool>();

            if (target == null)
                return;

            if (useq && target.IsValidTarget(Q.Range)
                && target.Distance(Player) > Player.AttackRange
                && Q.IsReady()
                && !Player.IsWindingUp)
            {
                Q.Cast();
            }

            if (usee && target.IsValidTarget(E.Range) && E.IsReady() && Player.Distance(target) > Player.AttackRange)
            {
                E.Cast(target);
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var useq = Config.Item("comboMenu.useq").GetValue<bool>();
            var usee = Config.Item("comboMenu.usee").GetValue<bool>();
            var user = Config.Item("comboMenu.user").GetValue<StringList>().SelectedIndex;
            if (target == null)
                return;

            if (useq && target.IsValidTarget(Q.Range)
                && target.Distance(Player) > Player.AttackRange
                && Q.IsReady()
                && !Player.IsWindingUp)
            {
                Q.Cast();
            }

            if (usee && target.IsValidTarget(E.Range) && E.IsReady() && Player.Distance(target) >E.Range - Player.AttackRange)
            {
                E.Cast(target);
            }
            

            switch (user)
            {
                case 0:
                {
                    if (R.IsReady() && target.IsValidTarget(R.Range))
                    {
                        R.Cast(target);
                    }
                    break;
                }
                case 1:
                {
                    if (R.IsReady() && target.IsValidTarget(R.Range))
                    {
                        if (target.Health < R.GetDamage(target))
                        {
                            R.Cast(target);
                            
                        }
                    }
                }
                    break;
            }
            if (Player.HasBuff("dariusexecutemulticast"))
            {
                lastr = Environment.TickCount;
            }

            if (Environment.TickCount - lastr >= 18000
                && Player.HasBuff("dariusexecutemulticast"))
            {
                R.Cast(target);
            }
            
        }


        public static int lastr { get; set; }
    }
}
