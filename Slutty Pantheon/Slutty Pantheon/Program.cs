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

namespace Slutty_Pantheon
{
    internal class Program
    {
        public const string ChampName = "Pantheon";
        public const string Menuname = "Slutty Pantheon";
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

            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 440);

            E.SetSkillshot(0.25f, 15f * 2 * (float)Math.PI / 180, 2000f, false, SkillshotType.SkillshotCone);

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

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));


            Config.AddSubMenu(new Menu("Mixed", "Mixed"));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));
            Config.SubMenu("Mixed").AddItem(new MenuItem("useM", "Harras if Mana above").SetValue(new Slider(50)));


            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useEPL", "Minimum mana for lane clear").SetValue(new Slider(50)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useE2L", "Use E to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useESlider", "Min minions for E").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useItems", "Use Hydra/Tiamat").SetValue(true));


            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));

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

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("useW2I", "W to interrupt").SetValue(true));

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnPossibleToInterrupt += OnInterruptableSpell;
        }
        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;


            Potion();
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
            KillSteal();
            Potion();
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
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

        private static void OnInterruptableSpell(Obj_AI_Base target, InterruptableSpell spell)
        {
            var wSpell = Config.Item("useW2I").GetValue<bool>();
            if (wSpell
                && W.IsReady()
                && target.IsValidTarget(W.Range))
            {
                W.CastOnUnit(target);
            }
        }

        private static void Combo()
        {

            if (Player.HasBuff("sound"))
                return;

            var useQ = Config.Item("UseQ").GetValue<bool>();
            var useW = Config.Item("UseW").GetValue<bool>();
            var useE = Config.Item("UseE").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Player.IsChannelingImportantSpell())
                return;

            if (useQ
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id)
                || Items.HasItem(ItemData.Tiamat_Melee_Only.Id))
            {
                Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
            }
            if (useW
                && W.IsReady()
                && target.IsValidTarget(W.Range))
            {
                W.CastOnUnit(target);
            }
            if (useE
                && E.IsReady()
                && target.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
        }

        private static void LaneClear()
        {
            if (Player.HasBuff("sound"))
                return;

            var useI = Config.Item("useItems").GetValue<bool>();
            var useQl = Config.Item("useQlc").GetValue<bool>();
            var useQ = Config.Item("useQ2L").GetValue<bool>();
            var useE = Config.Item("useE2L").GetValue<bool>();
            var minMana = Config.Item("useEPL").GetValue<Slider>().Value;
            var minMinionsE = Config.Item("useESlider").GetValue<Slider>().Value;      
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {
                    if (minionCount.Count > 2
                        && useI
                        &&(Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id) || Items.HasItem(ItemData.Tiamat_Melee_Only.Id)))
                    {
                        Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                        Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
                    }

                    if (Player.ManaPercent < minMana)
                        return;

                    if (useQl
                        && Q.IsReady()
                        && Q.GetDamage(minion) > minion.Health
                        && minion.HealthPercent > 13)
                    {
                        Q.CastOnUnit(minion);
                    }
                    if (useQ
                        && Q.IsReady())
                    {
                        Q.CastOnUnit(minion);
                    }
                        var allMinionsE = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
                        var EFarm = Q.GetCircularFarmLocation(allMinionsE, 200 );
                    if (useE
                        && E.IsReady()
                        && minionCount.Count >= minMinionsE
                        && EFarm.MinionsHit >= minMinionsE)
                    {
                        E.Cast(minion);
                    }
                }
            }
        }

        private static void Mixed()
        {
            var useQ = Config.Item("UseQM").GetValue<bool>();
            var minMana = Config.Item("useM").GetValue<Slider>().Value;
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Player.IsChannelingImportantSpell())
                return;

            if (useQ
                && Q.IsReady()
                && target.IsValidTarget(Q.Range)
                && Player.ManaPercent >= minMana)
            {
                Q.CastOnUnit(target);
            }
        }

        private static void KillSteal()
        {
            var ks = Config.Item("KS").GetValue<bool>();
            var useQ = Config.Item("UseQ2KS").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (!ks)
                return;
            if (useQ
                && Q.IsReady()
                && target.IsValidTarget(Q.Range)
                && Q.GetDamage(target) > target.Health)
            {
                Q.CastOnUnit(target);
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
