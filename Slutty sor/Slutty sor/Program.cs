
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;

namespace Slutty_sor
{
    internal class Program
    {
        public const string ChampName = "Soraka";
        public const string Menuname = "Slutty Soraka";
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

            Q = new Spell(SpellSlot.Q, 950);
            W = new Spell(SpellSlot.W, 550);
            E = new Spell(SpellSlot.E, 925);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.5f, 300, 1750, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.5f, 70f, 1750, false, SkillshotType.SkillshotCircle);

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("wDraw", "w Drawing").SetValue(true));
            var drawDamageMenu = new MenuItem("RushDrawDamage", "Combo Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            Config.SubMenu("Drawings").AddItem(drawDamageMenu);
            Config.SubMenu("Drawings").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));

            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useE2KS", "Use E for ks").SetValue(true));

            Config.AddSubMenu(new Menu("AutoHeal", "AutoHeal"));
            Config.SubMenu("AutoHeal").AddItem(new MenuItem("UseWh", "Use W").SetValue(true));
            Config.SubMenu("AutoHeal").AddItem(new MenuItem("useWh", "W If Ally %HP >").SetValue(new Slider(50)));
            Config.SubMenu("AutoHeal").AddItem(new MenuItem("duseWh", "dont W If your %HP >").SetValue(new Slider(50)));
            Config.SubMenu("AutoHeal").AddItem(new MenuItem("UseRh", "Use R").SetValue(true));
            Config.SubMenu("AutoHeal").AddItem(new MenuItem("duseRh", "R If Ally %HP >").SetValue(new Slider(50)));

            Config.AddSubMenu(new Menu("Harras", "Harras"));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseEH", "Use E").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("useH", "Harras if Mana above").SetValue(new Slider(50)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("UseQ2l", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQSlider", "Min minions for Q").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQm", "Min mana lane clear").SetValue(new Slider(50)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("UseE2I", "E On interruptable").SetValue(true));


            Config.AddSubMenu(new Menu("Auto Potions", "autoP"));
            Config.SubMenu("autoP").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("autoP").AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion")).SetValue(new Slider(50));
            Config.SubMenu("autoP").AddItem(new MenuItem("MANA", "Auto Mana Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("MANASlider", "Minimum %Mana for Potion")).SetValue(new Slider(50));
            Config.SubMenu("autoP").AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("bSlider", "Minimum %Health for Biscuit")).SetValue(new Slider(50));
            Config.SubMenu("autoP").AddItem(new MenuItem("flask", "Auto Flask").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("fSlider", "Minimum %Health for flask")).SetValue(new Slider(50));

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnPossibleToInterrupt += SorakaInterruptableSpell;

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
                Mixed();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }
            KillSteal();
            AutoHeal();
            Potion();
        }

        static float GetComboDamage(Obj_AI_Base enemy)
        {
                if (Q.IsReady())
                {
                    return Q.GetDamage(enemy);
                }
                if (E.IsReady())
                {
                    return E.GetDamage(enemy);
                }

            return 0;
        }
        private static void SorakaInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var eSpell = Config.Item("UseE2I").GetValue<bool>();
            if (eSpell)
            {
                Q.Cast(target);
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
        private static void Combo()
        {
            var qSpell = Config.Item("UseQ").GetValue<bool>();
            var eSpell = Config.Item("UseE").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (qSpell
                && eSpell
                && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
                Q.Cast(target);
            }

            if (qSpell
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (eSpell
                && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }

        }

        private static void Mixed()
        {
            var qSpell = Config.Item("UseQH").GetValue<bool>();
            var eSpell = Config.Item("UseEH").GetValue<bool>();
            var minm = Config.Item("useH").GetValue<Slider>().Value;

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Player.ManaPercent < minm)
                return;

            if (qSpell
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (eSpell
                && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
        }

        private static void LaneClear()
        {
            var minq = Config.Item("useQSlider").GetValue<Slider>().Value;
            var qSpell = Config.Item("UseQ2l").GetValue<bool>();
            var minm = Config.Item("useQm").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (Player.ManaPercent < minm)
                return;

            {
                foreach (var minion in minionCount)
                {
                    if (qSpell
                        && minionCount.Count >= minq)
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();
            var eSpell = Config.Item("useE2KS").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (qSpell
                && target.IsValidTarget(Q.Range)
                && target.Health < Q.GetDamage(target))
            {
                Q.Cast(target);
            }
            if (eSpell
                && target.IsValidTarget(E.Range)
                && target.Health < E.GetDamage(target))
            {
                E.Cast(target);
            }
        }

        private static void AutoHeal()
        {
            var rSpell = Config.Item("UseRh").GetValue<bool>();
            var wSpell = Config.Item("useWh").GetValue<bool>();
            var minhw = Config.Item("duseWh").GetValue<Slider>().Value;
            var minhr = Config.Item("duseRh").GetValue<Slider>().Value;
            foreach (var friend in
    ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly).Where(x => !x.IsDead).Where(x => !x.IsZombie))
            {
                var allyhealth = friend.HealthPercent;
                if (allyhealth <= minhr
                    && rSpell
                    && !friend.IsRecalling()
                    && !friend.InFountain())
                {
                    R.Cast();
                }
                if (allyhealth <= minhw
                    && wSpell
                    && !friend.IsRecalling()
                    && !friend.InFountain())
                {
                    W.CastOnUnit(friend);
                }
            }
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
            if (Config.Item("wDraw").GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Red);
            }
        }
    }
}
