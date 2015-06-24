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

namespace Slutty_Darius
{
    internal class Program
    {
        public const string ChampName = "Darius";
        public const string Menuname = "Slutty Darius";
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

            Q = new Spell(SpellSlot.Q, 425);
            W = new Spell(SpellSlot.W, 145);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 460);

            E.SetSkillshot(0.1f, 50f * (float)Math.PI / 180, float.MaxValue, false, SkillshotType.SkillshotCone);

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
            Config.SubMenu("Drawings").AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseCItems", "Use Hydra/Tiamat").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRc", "Only R when killable").SetValue(true));

            Config.AddSubMenu(new Menu("Harras", "Harras"));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useEPL", "Minimum Mana for Lane clear").SetValue(new Slider(50)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQSlider", "Min minions for Q").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useW2l", "use W").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useItems", "use Hydra/Tiamat").SetValue(true));

            Config.AddSubMenu(new Menu("misc", "Misc"));
            Config.SubMenu("misc").AddItem(new MenuItem("useE2I", "Use E To intterupt").SetValue(true));

            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useR2KS", "Use R for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQR2KS", "Use Q+R for ks").SetValue(true));

            Config.AddSubMenu(new Menu("Auto Potions", "autoP"));
            Config.SubMenu("autoP").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("autoP").AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion")).SetValue(new Slider(30));
            Config.SubMenu("autoP").AddItem(new MenuItem("MANA", "Auto Mana Potion").SetValue(true));
            Config.SubMenu("autoP")
                .AddItem(new MenuItem("MANASlider", "Minimum %Mana for Potion"))
                .SetValue(new Slider(30));
            Config.SubMenu("autoP").AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
            Config.SubMenu("autoP")
                .AddItem(new MenuItem("bSlider", "Minimum %Health for Biscuit"))
                .SetValue(new Slider(30));
            Config.SubMenu("autoP").AddItem(new MenuItem("flask", "Auto Flask").SetValue(true));
            Config.SubMenu("autoP")
                .AddItem(new MenuItem("fSlider", "Minimum %Health for flask"))
                .SetValue(new Slider(30));


            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnPossibleToInterrupt += OnInterruptableSpell;
            Orbwalking.BeforeAttack += BeforeAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;


        }
        private static void OnInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            
            var eSpell = Config.Item("useE2I").GetValue<bool>();
            if (eSpell
                && E.IsReady()
                && unit.IsValidTarget(E.Range)
                )
            {
                E.Cast(unit);
            }
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Config.Item("useQW2D").GetValue<bool>();

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
                        if (E.IsReady()
                            && args.EndPos.Distance(Player) < E.Range)
                        {
                            if (delay > 0)
                            {
                                Utility.DelayAction.Add(delay * 1000, () => E.Cast(args.EndPos));
                            }
                            else
                            {
                                E.Cast(target);
                            }
                        }
                    }
                }
            }
        }

        static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (args.Target.IsValid<Obj_AI_Hero>()
                && W.IsReady()
                && Player.Distance(target) <= Player.AttackRange)
            {
                W.Cast();
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;

            Potion();
            KillSteal();

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
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
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
            if (Config.Item("eDraw").GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);
            }
            if (Config.Item("rDraw").GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Gold);
            }
        }

        private static void Combo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var qSpell = Config.Item("UseQ").GetValue<bool>();
            var eSpell = Config.Item("UseE").GetValue<bool>();
            var wSpell = Config.Item("UseW").GetValue<bool>();
            var rSpell = Config.Item("UseR").GetValue<bool>();
            var rcSpell = Config.Item("UseRc").GetValue<bool>();
            var items = Config.Item("UseCItems").GetValue<bool>();

            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }
            if (items
                && (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id)
                || Items.HasItem(ItemData.Tiamat_Melee_Only.Id))
                && target.IsValidTarget(Q.Range))
            {
                Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
            }
            if (eSpell
                && E.IsReady()
                && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
            if (wSpell
                && W.IsReady()
                && target.IsValidTarget(W.Range)
                && Player.Distance(target) <= Player.AttackRange)
            {
                W.Cast();
            }
            if (rSpell)
            {
                if (target.HasBuffOfType(BuffType.PhysicalImmunity)
                    || target.HasBuffOfType(BuffType.SpellImmunity)
                    || target.HasBuffOfType(BuffType.Invulnerability))
                    return;

                if (rcSpell
                    && R.GetDamage(target) > target.Health)
                {
                    R.CastOnUnit(target);
                }
                if (!rcSpell)
                {
                    R.CastOnUnit(target);
                }
                    
            }

        }

        private static void LaneClear()
        {
            var qSpell = Config.Item("useQ2L").GetValue<bool>();
            var wSpell = Config.Item("useW2l").GetValue<bool>();
            var items = Config.Item("useItems").GetValue<bool>();
            var minMana = Config.Item("useEPL").GetValue<Slider>().Value;
            var minQ = Config.Item("useQSlider").GetValue<Slider>().Value;

            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if ((minionCount.Count >= 2)
                    && (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id) || Items.HasItem(ItemData.Tiamat_Melee_Only.Id))
                    && items)
                {
                    Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                    Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
                }
                if (Player.ManaPercent < minMana)
                    return;
                if (qSpell
                    && minionCount.Count >= minQ
                    && Q.IsReady())
                {
                    Q.Cast();
                }
                if (wSpell
                    && W.IsReady())
                {
                    W.Cast();
                }
            }
        }

        private static void Mixed()
        {
            var qSpell = Config.Item("UseQM").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast();
            }
        }

        private static void KillSteal()
        {
            var ks = Config.Item("KS").GetValue<bool>();
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();
            var rSpell = Config.Item("useR2KS").GetValue<bool>();
            var qrSpell = Config.Item("useQR2KS").GetValue<bool>();

            if (!ks)
                return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (qrSpell
                && Q.IsReady()
                && R.IsReady()
                && target.IsValidTarget(R.Range)
                && (Q.GetDamage(target) + R.GetDamage(target)) > target.Health)
            {
                Q.Cast();
                R.CastOnUnit(target);
            }
            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(Q.Range)
                && Q.GetDamage(target) > target.Health)
            {
                Q.Cast();
            }
            if (rSpell
                && R.IsReady()
                && target.IsValidTarget(Q.Range)
                && R.GetDamage(target) > target.Health)
            {
                R.CastOnUnit(target);
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
            if (!autoPotion)
            {
                return;
            }

            if (hPotion
                && Player.HealthPercent <= pSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !Player.HasBuff("FlaskOfCrystalWater")
                && !Player.HasBuff("ItemCrystalFlask")
                && !Player.HasBuff("RegenerationPotion"))
            {
                HealthPotion.Cast();
            }

            if (mPotion
                && Player.ManaPercent <= mSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && ManaPotion.IsReady()
                && !Player.HasBuff("RegenerationPotion")
                && !Player.HasBuff("FlaskOfCrystalWater"))
            {
                ManaPotion.Cast();
            }

            if (bPotion
                && Player.HealthPercent <= bSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && BiscuitofRejuvenation.IsReady()
                && !Player.HasBuff("ItemMiniRegenPotion"))
            {
                BiscuitofRejuvenation.Cast();
            }

            if (fPotion
                && Player.HealthPercent <= fSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && CrystallineFlask.IsReady()
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
