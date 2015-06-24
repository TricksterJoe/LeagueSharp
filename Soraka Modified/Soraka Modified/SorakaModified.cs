#region

using System;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using ItemData = LeagueSharp.Common.Data.ItemData;

#endregion


// this is a modified version of Sophie's soraka which can be found here https://github.com/ChewyMoon/ChewyMoonScripts/tree/master/Sophie's%20Soraka
// modified by "Hoes" Joduskame

namespace Soraka_Modified
{
    internal class SorakaModified
    {
        public static Spell Q;
        public static Spell W;
        public static Spell E;
        public static Spell R;
        public static Menu Menu;
        public static Orbwalking.Orbwalker Orbwalker;
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Items.Item mikael = new Items.Item(3222);

        public static bool Packets
        {
            get { return Menu.Item("packets").GetValue<bool>(); }
        }

        public static void OnGameLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Soraka")
            {
                return;
            }

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.5f, 300, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);

            CreateMenu();


            Interrupter2.OnInterruptableTarget += InterrupterOnOnPossibleToInterrupt;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloserOnOnEnemyGapcloser;
            Game.OnUpdate += GameOnOnGameUpdate;
            Drawing.OnDraw += DrawingOnOnDraw;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Menu.Item("useEdashcloser").GetValue<bool>();

            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId)
            {
                if (qSpell)
                {

                    if (Q.IsReady()
                        && args.EndPos.Distance(Player) < Q.Range)
                    {
                        var delay = (int)(args.EndTick - Game.Time - Q.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay * 1000, () => Q.Cast(args.EndPos));
                        }
                        else
                        {
                            Q.Cast(args.EndPos);
                        }
                        if (Q.IsReady()
                            && args.EndPos.Distance(Player) < Q.Range)
                        {
                            if (delay > 0)
                            {
                                Utility.DelayAction.Add(delay * 1000, () => Q.Cast(args.EndPos));
                            }
                            else
                            {
                                E.Cast(args.EndPos);
                            }
                        }
                    }
                }
            }
        }


        private static
              void DrawingOnOnDraw(EventArgs args)
        {
            var drawQ = Menu.Item("drawQ").GetValue<bool>();
            var drawW = Menu.Item("drawW").GetValue<bool>();
            var drawE = Menu.Item("drawE").GetValue<bool>();

            var p = ObjectManager.Player.Position;

            if (drawQ)
            {
                Render.Circle.DrawCircle(p, Q.Range, Q.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawW)
            {
                Render.Circle.DrawCircle(p, W.Range, W.IsReady() ? Color.Aqua : Color.Red);
            }

            if (drawE)
            {
                Render.Circle.DrawCircle(p, E.Range, E.IsReady() ? Color.Aqua : Color.Red);
            }
        }

        private static void GameOnOnGameUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Mixed:
                    Harass();
                    break;
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
            }
            Mikaels();
            if (Menu.Item("autoW").GetValue<bool>())
            {
                AutoW();
            }

            if (Menu.Item("autoR").GetValue<bool>())
            {
                AutoR();
            }
        }

        private static void Mikaels()
        {
            foreach (var friend in
                from friend in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsMe).Where(friend => W.IsInRange(friend.ServerPosition, W.Range))
                select friend)
            {

                if (UnitIsImmobileUntil(friend) > 0.5f
                    && mikael.IsReady())
                {
                    mikael.Cast(friend);
                }
            }


        }
        static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var time =
                unit.Buffs.Where(buff => buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm
                        || buff.Type == BuffType.Knockup
                        || buff.Type == BuffType.Suppression
                        || buff.Type == BuffType.Stun
                        || buff.Type == BuffType.Snare)).Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (time - Game.Time);
        }
        private static void AutoR()
        {
            if (!R.IsReady())
            {
                return;
            }

            foreach (var friend in
                ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly).Where(x => !x.IsDead).Where(x => !x.IsZombie))
            {
                var friendHealth = (int)friend.Health / friend.MaxHealth * 100;
                var health = Menu.Item("autoRPercent").GetValue<Slider>().Value;

                if (friendHealth <= health)
                {
                    R.Cast(Packets);
                }
            }
        }

        private static void AutoW()
        {
            if (!W.IsReady())
            {
                return;
            }

            /*
            var autoWHealth = Menu.Item("autoWHealth").GetValue<Slider>().Value;
            if (ObjectManager.Player.HealthPercentage() < autoWHealth)
            {
                return;
            }

            foreach (var friend in
                from friend in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsMe)
                        .Where(friend => W.IsInRange(friend.ServerPosition, W.Range))
                let friendHealth = friend.Health / friend.MaxHealth * 100
                let healthPercent = Menu.Item("autoWPercent").GetValue<Slider>().Value
                where friendHealth <= healthPercent
                select friend)
            {
                W.CastOnUnit(friend, Packets);
            }
            */

            if (Player.HealthPercent < Menu.Item("hprest").GetValue<Slider>().Value)
                return;

            foreach (var hero in HeroManager.Allies)
            {
                if (Menu.Item("healop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0
                    && hero.HealthPercent <= Menu.Item("hpsettings" + hero.ChampionName).GetValue<Slider>().Value)
                {
                    W.CastOnUnit(hero);
                }
            }
        }

        private static void Combo()
        {
            var useQ = Menu.Item("useQ").GetValue<bool>();
            var useE = Menu.Item("useE").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(target, Packets);
            }

            if (useE && E.IsReady())
            {
                E.Cast(target, Packets);
            }
        }

        private static void Harass()
        {
            var useQ = Menu.Item("useQHarass").GetValue<bool>();
            var useE = Menu.Item("useEHarass").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (useQ && Q.IsReady())
            {
                Q.Cast(target, Packets);
            }

            if (useE && E.IsReady())
            {
                E.Cast(target, Packets);
            }
        }

        private static void AntiGapcloserOnOnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            var unit = gapcloser.Sender;

            if (Menu.Item("useQGapcloser").GetValue<bool>() && unit.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(unit, Packets);
            }

            if (Menu.Item("useEGapcloser").GetValue<bool>() && unit.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(unit, Packets);
            }
        }

        private static void InterrupterOnOnPossibleToInterrupt(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var unit = sender;
            var spell = args;

            if (Menu.Item("eInterrupt").GetValue<bool>() == false || spell.DangerLevel != Interrupter2.DangerLevel.High)
            {
                return;
            }

            if (!unit.IsValidTarget(E.Range))
            {
                return;
            }
            if (!E.IsReady())
            {
                return;
            }

            E.Cast(unit, Packets);
        }

        private static void CreateMenu()
        {
            Menu = new Menu("Sophies's Soraka", "sSoraka", true);

            // Target Selector
            var tsMenu = new Menu("Target Selector", "ssTS");
            TargetSelector.AddToMenu(tsMenu);
            Menu.AddSubMenu(tsMenu);

            // Orbwalking
            var orbwalkingMenu = new Menu("Orbwalking", "ssOrbwalking");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkingMenu);
            Menu.AddSubMenu(orbwalkingMenu);

            // Combo
            var comboMenu = new Menu("Combo", "ssCombo");
            comboMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            comboMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Menu.AddSubMenu(comboMenu);

            // Harass
            var harassMenu = new Menu("Harass", "ssHarass");
            harassMenu.AddItem(new MenuItem("useQHarass", "Use Q").SetValue(true));
            harassMenu.AddItem(new MenuItem("useEHarass", "Use E").SetValue(true));
            Menu.AddSubMenu(harassMenu);

            // Drawing
            var drawingMenu = new Menu("Drawing", "ssDrawing");
            drawingMenu.AddItem(new MenuItem("drawQ", "Draw Q").SetValue(true));
            drawingMenu.AddItem(new MenuItem("drawW", "Draw W").SetValue(true));
            drawingMenu.AddItem(new MenuItem("drawE", "Draw E").SetValue(true));
            Menu.AddSubMenu(drawingMenu);

            // Misc
            var miscMenu = new Menu("Misc", "ssMisc");
            miscMenu.AddItem(new MenuItem("packets", "Use Packets").SetValue(true));
            miscMenu.AddItem(new MenuItem("useQGapcloser", "Q on Gapcloser").SetValue(true));
            miscMenu.AddItem(new MenuItem("useEGapcloser", "E on Gapcloser").SetValue(true));
            miscMenu.AddItem(new MenuItem("useEdashcloser", "E on Dash").SetValue(true));
            miscMenu.AddItem(new MenuItem("autoW", "Auto use W").SetValue(true));
            miscMenu.AddItem(new MenuItem("autoWPercent", "% Percent").SetValue(new Slider(50, 1)));
            miscMenu.AddItem(new MenuItem("autoWHealth", "My Health Percent").SetValue(new Slider(30, 1)));
            miscMenu.AddItem(new MenuItem("autoR", "Auto use R").SetValue(true));
            miscMenu.AddItem(new MenuItem("autoRPercent", "% Percent").SetValue(new Slider(15, 1)));
            miscMenu.AddItem(new MenuItem("eInterrupt", "Use E to Interrupt").SetValue(true));
            Menu.AddSubMenu(miscMenu);

            foreach (var hero in HeroManager.Allies)
            {
                Menu.AddItem(new MenuItem("healop" + hero.ChampionName, hero.ChampionName))
                    .SetValue(new StringList(new[] { "Heal", "Don't Heal" }, 1));

                Menu.AddItem(new MenuItem("hpsettings" + hero.ChampionName, "% Hp to").SetValue(new Slider(20)));

                Menu.AddItem(new MenuItem("hprest", "Min %HP To heal").SetValue(new Slider(20)));
            }


            Menu.AddToMainMenu();
        }

        public static void PrintChat(string msg)
        {
            Game.PrintChat("<font color='#3492EB'>Sophie's Soraka:</font> <font color='#FFFFFF'>" + msg + "</font>");
        }
    }
}
