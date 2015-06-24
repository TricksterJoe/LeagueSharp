using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_Irelia
{
    internal class Program
    {
        public const string ChampName = "Irelia";
        public const string Menuname = "Slutty Irelia";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;

        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static Items.Item HealthPotion = new Items.Item(2003);
        public static Items.Item CrystallineFlask = new Items.Item(2041);
        public static Items.Item ManaPotion = new Items.Item(2004);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010);



        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, Orbwalking.GetRealAutoAttackRange(Player));
            E = new Spell(SpellSlot.E, 350);
            R = new Spell(SpellSlot.R, 1000);

            Q.SetTargetted(0f, 2200);
            R.SetSkillshot(0.5f, 120, 1600, false, SkillshotType.SkillshotLine);




            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("sDraw", "Draw Stunnable target").SetValue(true));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQgap", "Q gapcloser jump").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseES", "Use E only to stun").SetValue(false));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useLM", "Minimum Mana for Lane Clear").SetValue(new Slider(50)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useR2L", "Use R to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useW2l", "use W to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useRSlider", "Min minions for R").SetValue(new Slider(3, 1, 20)));

            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useW2KS", "Use W for ks").SetValue(true));

            Config.AddSubMenu(new Menu("Auto Potions", "autoP"));
            Config.SubMenu("autoP").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("autoP")
                .AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion"))
                .SetValue(new Slider(50));
            Config.SubMenu("autoP").AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
            Config.SubMenu("autoP")
                .AddItem(new MenuItem("bSlider", "Minimum %Health for Biscuit"))
                .SetValue(new Slider(50));


            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;
            

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();

            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
            }
            Potion();
            KillSteal();
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Config.Item("qDraw").GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);
            }
            if (Config.Item("eDraw").GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);
            }
            if (Config.Item("rDraw").GetValue<bool>() && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Red);
            }
            if (Config.Item("sDraw").GetValue<bool>() && E.Level > 0)
            {
                foreach (var target in ObjectManager.Get<Obj_AI_Hero>().Where(enemy => Player.Distance(enemy) <= 1000
                                                                                       &&
                                                                                       (Player.HealthPercent <
                                                                                        enemy.HealthPercent)
                                                                                       && enemy.IsValidTarget(1000)))
                {
                    var heroPosition = Drawing.WorldToScreen(target.Position);
                    var textDimension = Drawing.GetTextExtent("Stunnable!");
                    Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                        Color.DarkOrange, "Stunnable");
                }

            }
        }


        private static void Combo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var useQ = Config.Item("UseQ").GetValue<bool>();
            var useW = Config.Item("UseW").GetValue<bool>();
            var useE = Config.Item("UseE").GetValue<bool>();
            var useEs = Config.Item("UseES").GetValue<bool>();
            var useR = Config.Item("UseR").GetValue<bool>();

            if (useQ
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (useW
                && W.IsReady())
            {
                W.Cast();
            }

            if (useEs)
            {
                if (Player.HealthPercent < target.HealthPercent
                    && useE
                    && E.IsReady())
                {
                    E.CastOnUnit(target);
                }
            }
            else
            {
                if (useE
                    && E.IsReady())
                {
                    E.Cast(target);
                }
            }
            if (useR
                && R.IsReady())
            {
                R.Cast(target);
            }
        }


        private static void LaneClear()
        {
            var useQ = Config.Item("useQlc").GetValue<bool>();
            var useW = Config.Item("useW2l").GetValue<bool>();
            var useR = Config.Item("useR2L").GetValue<bool>();
            var useRs = Config.Item("useRSlider").GetValue<Slider>().Value;
            var minM = Config.Item("useLM").GetValue<Slider>().Value;
            var position = R.GetLineFarmLocation(MinionManager.GetMinions(R.Range));
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (Player.ManaPercent < minM)
                return;

            foreach (var minion in minionCount)
            {
                if (useQ
                    && Q.IsReady()
                    && Q.GetDamage(minion) > minion.Health)
                {
                    Q.CastOnUnit(minion);
                }
                if (useW
                    && W.IsReady())
                {
                    W.Cast();
                }
                if (useR
                    && R.IsReady()
                    && minionCount.Count >= useRs)
                {
                    R.Cast(position.Position);
                }
            }
        }

        private static void KillSteal()
        {
            var ks = Config.Item("KS").GetValue<bool>();
            var useQ = Config.Item("useQlc").GetValue<bool>();
            var useE = Config.Item("useW2l").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!ks)
                return;
            if (useQ
                && target.IsValidTarget(Q.Range)
                && target.Health < Q.GetDamage(target))
            {
                Q.CastOnUnit(target);
            }
            if (useE
                && target.IsValidTarget(E.Range)
                && target.Health < E.GetDamage(target))
            {
                E.CastOnUnit(target);
            }
        }

        private static void Potion()
        {
            var autoPotion = Config.Item("autoPO").GetValue<bool>();
            var hPotion = Config.Item("HP").GetValue<bool>();
            var mPotion = Config.Item("MANA").GetValue<bool>();
            var bPotion = Config.Item("Biscuit").GetValue<bool>();
            var fPotion = Config.Item("flask").GetValue<bool>();
            var pSlider = Config.Item("HPSlider").GetValue<Slider>().Value;
            var mSlider = Config.Item("MANASlider").GetValue<Slider>().Value;
            var bSlider = Config.Item("bSlider").GetValue<Slider>().Value;
            var fSlider = Config.Item("fSlider").GetValue<Slider>().Value;
            if (Player.IsRecalling() || Player.InFountain())
            {
                return;
            }
            if (autoPotion
                && hPotion
                && Player.HealthPercent <= pSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !Player.HasBuff("RegenerationPotion")
                && !Player.HasBuff("ItemCrystalFlask"))
            {
                HealthPotion.Cast();
            }

            if (autoPotion
                && mPotion
                && Player.ManaPercent <= mSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !Player.HasBuff("RegenerationPotion")
                && !Player.HasBuff("ItemCrystalFlask"))
            {
                ManaPotion.Cast();
            }

            if (autoPotion
                && bPotion
                && Player.HealthPercent <= bSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !Player.HasBuff("ItemMiniRegenPotion"))
            {
                BiscuitofRejuvenation.Cast();
            }

            if (autoPotion
                && fPotion
                && Player.HealthPercent <= fSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !Player.HasBuff("ItemMiniRegenPotion")
                && !Player.HasBuff("ItemCrystalFlask")
                && !Player.HasBuff("RegenerationPotion")
                && !Player.HasBuff("FlaskOfCrystalWater"))
            {
                CrystallineFlask.Cast();
            }
        }
    }
}
