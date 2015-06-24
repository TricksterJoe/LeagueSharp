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

namespace Slutty_Kennen
{
    internal class Program
    {
        public const string ChampName = "Kennen";
        public const string Menuname = "Slutty Kennen";
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
            Config.SubMenu("Drawings").AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseWs", "Use W Only if target is stunnable").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useemin", "Minimum energy for E")).SetValue(new Slider(50, 0, 200));
            Config.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useRc", "Only R when will hit").SetValue(new Slider(3, 1, 5)));

            Config.AddSubMenu(new Menu("Mixed", "Mixed"));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseWM", "Use W").SetValue(true));

            Config.AddSubMenu(new Menu("Harras", "Harras"));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseEH", "Use W").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("useH", "Harras if Mana above").SetValue(new Slider(50)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useE2L", "Use E to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useESlider", "Min minions for E").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useEPL", "Minimum energy for E").SetValue(new Slider(50, 1, 200)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useW2l", "use W to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useWSlider", "Min minions for W").SetValue(new Slider(3, 1, 20)));


            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useW2KS", "Use W for ks").SetValue(true));

            Config.AddSubMenu(new Menu("Auto Potions", "autoP"));
            Config.SubMenu("autoP").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("autoP").AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion")).SetValue(new Slider(50));
            Config.SubMenu("autoP").AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("bSlider", "Minimum %Health for Biscuit")).SetValue(new Slider(50));

            Config.AddItem(new MenuItem("fleekey", "Use Flee Mode")).SetValue(new KeyBind(65, KeyBindType.Press));

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;

        }

        private static void Flee()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wSpell = Config.Item("UseW").GetValue<bool>();
            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            if (!Player.HasBuff("KennenLightningRush"))
            {
                E.Cast();
            }
            if (wSpell
                 && target.IsValidTarget(W.Range)
                 && W.IsReady()
                 && target.Buffs.Any(buff => buff.Name == "kennenmarkofstorm" && buff.Count == 2))
            {
                W.Cast();
            }

        }

        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;
            Potion();

            if (Player.IsValid &&
                Config.Item("fleekey").GetValue<KeyBind>().Active)
            {
                Flee();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
                KillSteal();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Mixed();
                KillSteal();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
                KillSteal();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                KillSteal();
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
            if (Config.Item("wDraw").GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Gold);
            }
        }


        private static void Combo()
        {
            if (Player.HasBuff("KennenLightningRush"))
            {
                return;
            }

            var qSpell = Config.Item("UseQ").GetValue<bool>();
            var eSpell = Config.Item("UseE").GetValue<bool>();
            var wSpell = Config.Item("UseW").GetValue<bool>();
            var rSpell = Config.Item("useR").GetValue<bool>();
            var rCount = Config.Item("useRc").GetValue<Slider>().Value;
            var emSpell = Config.Item("useemin").GetValue<Slider>().Value;
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Player.HasBuff("KennenLightningRush") && Player.Health > target.Health && target.UnderTurret(true))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, target);
            }
            if (eSpell
                && E.IsReady()
                && (Player.Distance(target) < 80
                    && !Q.CanCast(target)
                    || (!Player.HasBuff("KennenLightningRush")
                        && !target.UnderTurret(true)
                        && emSpell < Player.MaxMana)))
            {
                E.Cast();
            }

            if (qSpell
                && target.IsValidTarget(Q.Range)
                && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (wSpell
                && target.IsValidTarget(W.Range)
                && W.IsReady()
                && target.Buffs.Any(buff => buff.Name == "kennenmarkofstorm" && buff.Count == 2))
            {
                W.Cast();
            }
            if (rSpell
                && R.IsReady()
                & target.IsValidTarget(R.Range)
                && target.CountEnemiesInRange(R.Range) >= rCount)
            {
                R.Cast();
            }


        }

        private static void LaneClear()
        {
            if (Player.HasBuff("KennenLightningRush"))
            {
                return;
            }
            var elSpell = Config.Item("useE2L").GetValue<bool>();
            var qlSpell = Config.Item("useQlc").GetValue<bool>();
            var q2LSpell = Config.Item("useQ2L").GetValue<bool>();
            var wlSpell = Config.Item("useW2l").GetValue<bool>();
            var elSlider = Config.Item("useESlider").GetValue<Slider>().Value;
            var emSlider = Config.Item("useEPL").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {

                    if (qlSpell
                        && (Q.GetDamage(minion) >= minion.Health)
                        && Q.IsReady())
                    {
                        Q.Cast(minion);
                    }

                    if (elSpell
                        && E.IsReady()
                        && !minion.UnderTurret(true)
                        && minionCount.Count > elSlider
                        && emSlider < Player.MaxMana)
                    {
                        E.Cast();
                    }

                    if (wlSpell                   
                        && W.IsReady()
                        && minion.Buffs.Any(buff => buff.Name == "kennenmarkofstorm")
                        && minionCount.Count > elSlider)
                    {
                        W.Cast();
                    }

                    if (q2LSpell
                        && Q.IsReady())
                    {
                        Q.Cast(minion);
                    }

                    if (Player.HasBuff("KennenLightningRush"))
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, minionCount.FirstOrDefault());
                    }
                }
            }
        }

        private static void Mixed()
        {
            var qSpell = Config.Item("UseQM").GetValue<bool>();
            var wSpell = Config.Item("UseWM").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (qSpell
                && target.IsValidTarget(Q.Range)
                && Q.IsReady())
            {
                Q.Cast(target);
            }

            if (wSpell
                && target.IsValidTarget(W.Range)
                && W.IsReady()
                && target.Buffs.Any(buff => buff.Name == "kennenmarkofstorm" && buff.Count == 2))
            {
                W.Cast();
            }

        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();
            var wSpell = Config.Item("useW2KS").GetValue<bool>();
            if (qSpell
                && Q.GetDamage(target) > target.Health
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (wSpell
                && W.GetDamage(target) > target.Health
                && target.IsValidTarget(W.Range)
                && target.Buffs.Any(buff => buff.Name == "kennenmarkofstorm"))
            {
                W.Cast();
            }
        }

        private static void Potion()
        {
            var autoPotion = Config.Item("autoPO").GetValue<bool>();
            var hPotion = Config.Item("HP").GetValue<bool>();
            var bPotion = Config.Item("Biscuit").GetValue<bool>();
            var pSlider = Config.Item("HPSlider").GetValue<Slider>().Value;
            var bSlider = Config.Item("bSlider").GetValue<Slider>().Value;
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
                && bPotion
                && Player.HealthPercent <= bSlider
                && Player.CountEnemiesInRange(1000) >= 0
                && HealthPotion.IsReady()
                && !Player.HasBuff("ItemMiniRegenPotion")
                && !Player.HasBuff("RegenerationPotion"))
            {
                BiscuitofRejuvenation.Cast();
            }          
        }
    }
}
