using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common.Data;
using SharpDX.Win32;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    internal class Program
    {
        public const string ChampName = "Ryze";
        public const string Menuname = "Slutty Ryze";
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

            Q = new Spell(SpellSlot.Q, 1050);
            W = new Spell(SpellSlot.W, 800);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R, 500);

            Q.SetSkillshot(0.26f, 50f, 1700f, true, SkillshotType.SkillshotLine);

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
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            Config.SubMenu("Drawings").AddItem(drawDamageMenu);
            Config.SubMenu("Drawings").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;


            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));

            Config.AddSubMenu(new Menu("ComboOptions", "ComboOptions"));
            Config.SubMenu("ComboOptions").AddItem(new MenuItem("AAblock", "Block auto attack in combo").SetValue(true));

            Config.AddSubMenu(new Menu("Mixed", "Mixed"));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseEM", "Use E").SetValue(true));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseWM", "Use W").SetValue(true));


            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useWlc", "Use W to last hit in lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useElc", "Use E to last hit in lane clear").SetValue(true));   
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useW2L", "Use W to lane clear").SetValue(true)); 
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useE2L", "Use E to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useESlider", "Min minions for E").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useEPL", "Minimum mana for lane clear").SetValue(new Slider(50, 1, 200)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useR", "Use R in lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("rMin", "Minimum minions for R").SetValue(new Slider(3, 1, 20)));

            Config.AddSubMenu(new Menu("Items", "Items"));
            Config.SubMenu("Items").AddItem(new MenuItem("tearS", "Stack tear").SetValue(true));
            Config.SubMenu("Items").AddItem(new MenuItem("tearSM", "Min Mana").SetValue(new Slider(95)));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));

            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useW2KS", "Use W for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useE2KS", "Use W for ks").SetValue(true));

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
            Interrupter.OnPossibleToInterrupt += BlitzInterruptableSpell;

        }
        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                AABlock();
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Mixed();
                Orbwalker.SetAttack(true);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
                Orbwalker.SetAttack(true);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                TearStack();
                Orbwalker.SetAttack(true);
            }
            
            Potion();
            KillSteal();
            Orbwalker.SetAttack(true);
        }

        private static void BlitzInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wSpell = Config.Item("useW2I").GetValue<bool>();
            if (wSpell)
            {
                W.CastOnUnit(target);
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
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);
            }
        }

        private static void Combo()
        {
            var qSpell = Config.Item("useQ").GetValue<bool>();
            var eSpell = Config.Item("useE").GetValue<bool>();
            var wSpell = Config.Item("useW").GetValue<bool>();
            var rSpell = Config.Item("useR").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);

            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (wSpell
                && W.IsReady()
                && target.IsValidTarget(W.Range))
            {
                W.CastOnUnit(target);
            }

            if (eSpell
                && E.IsReady()
                && target.IsValidTarget(E.Range))
            {
                E.CastOnUnit(target);
            }

            if (rSpell
                && R.IsReady()
                && target.Health > (Q.GetDamage(target) + E.GetDamage(target))
                && target.IsValidTarget(Q.Range))
            {
                R.Cast();
            }
        }

        private static void LaneClear()
        {
            var qlchSpell = Config.Item("useQlc").GetValue<bool>();
            var elchSpell = Config.Item("useElc").GetValue<bool>();
            var wlchSpell = Config.Item("useWlc").GetValue<bool>();
            var q2LSpell = Config.Item("useQ2L").GetValue<bool>();
            var e2LSpell = Config.Item("useE2L").GetValue<bool>();
            var w2LSpell = Config.Item("useW2L").GetValue<bool>();
            var rSpell = Config.Item("useR").GetValue<bool>();
            var rSlider = Config.Item("rMin").GetValue<Slider>().Value;
            var minMana = Config.Item("useEPL").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if (Player.ManaPercent <= minMana)
                    return;
                foreach (var minion in minionCount)
                {
                    if (qlchSpell
                        && Q.IsReady()
                        && minion.IsValidTarget(Q.Range)
                        && minion.Health > Q.GetDamage(minion))
                    {
                        Q.Cast(minion);
                    }

                    if (wlchSpell
                        && W.IsReady()
                        && minion.IsValidTarget(W.Range)
                        && minion.Health > W.GetDamage(minion))
                    {
                        W.CastOnUnit(minion);
                    }

                    if (elchSpell
                        && E.IsReady()
                        && minion.IsValidTarget(E.Range)
                        && minion.Health > E.GetDamage(minion))
                    {
                        E.CastOnUnit(minion);
                    }

                    if (q2LSpell
                        && Q.IsReady()
                        && minion.IsValidTarget(Q.Range))
                    {
                        Q.Cast(minion);
                    }

                    if (e2LSpell
                        && E.IsReady()
                        && minion.IsValidTarget(E.Range))
                    {
                        E.CastOnUnit(minion);
                    }
                    if (w2LSpell
                        && W.IsReady()
                        && minion.IsValidTarget(W.Range))
                    {
                        W.CastOnUnit(minion);
                    }
                    if (rSpell
                        && R.IsReady()
                        && minion.IsValidTarget(Q.Range)
                        && minionCount.Count >= rSlider)
                    {
                        R.Cast();
                    }
                }
            }
            
        }

        private static void Mixed()
        {
            var qSpell = Config.Item("UseQM").GetValue<bool>();
            var eSpell = Config.Item("UseEM").GetValue<bool>();
            var wSpell = Config.Item("UseWM").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);

            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }

            if (wSpell
                && W.IsReady()
                && target.IsValidTarget(W.Range))
            {
                W.CastOnUnit(target);
            }

            if (eSpell
                && E.IsReady()
                && target.IsValidTarget(E.Range))
            {
                E.CastOnUnit(target);
            }           
        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();
            var wSpell = Config.Item("useW2KS").GetValue<bool>();
            var eSpell = Config.Item("useE2KS").GetValue<bool>();
            if (qSpell
                && Q.GetDamage(target) > target.Health
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (wSpell
                && W.GetDamage(target) > target.Health
                && target.IsValidTarget(W.Range))
            {
                W.CastOnUnit(target);
            }
            if (eSpell
                && E.GetDamage(target) > target.Health
                && target.IsValidTarget(E.Range))
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

        private static void TearStack()
        {
            var tears = Config.Item("tearS").GetValue<bool>();
            var mtears = Config.Item("tearSM").GetValue<Slider>().Value;
            if (ItemData.Tear_of_the_Goddess.Stacks.Equals(750) 
                || Items.HasItem(ItemData.Seraphs_Embrace.Id) 
                || ItemData.Archangels_Staff.Stacks.Equals(750))
                return;


            if (tears
                && Q.IsReady() 
                && Player.ManaPercent >= mtears
                && ((Items.HasItem(ItemData.Tear_of_the_Goddess.Id) || Items.HasItem(ItemData.Archangels_Staff.Id))))
            {
                Q.Cast(Player.Position);
            }

        }

        private static void AABlock()
        {
            var aaBlock = Config.Item("AAblock").GetValue<bool>();
            if (aaBlock)
            {
                Orbwalker.SetAttack(false);
            }
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
            if (W.IsReady())
            {
                return W.GetDamage(enemy);
            }

            return 0;
        }
    }

}
