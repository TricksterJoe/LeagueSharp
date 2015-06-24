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

namespace Slutty_Blitz
{
    internal class Program
    {
        public const string ChampName = "Blitzcrank";
        public const string Menuname = "Slutty Blitz";
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

            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 150f);
            R = new Spell(SpellSlot.R, 550f);

            Q.SetSkillshot(0.25f, 70f, 1800f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.25f, 600, float.MaxValue, false, SkillshotType.SkillshotCircle);

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQm", "Minimum Q distance").SetValue(new Slider(300, 1, 1000)));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRc", "Only R when will hit").SetValue(new Slider(3, 1, 5)));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQmanual", "Semi manual Q").SetValue(new KeyBind(66, KeyBindType.Press)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useLM", "Lane Clear Mana >").SetValue(new Slider(50)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useE2l", "Use E to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useW2l", "use W to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useWSlider", "Min minions for W").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useR2l", "use R to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useRSlider", "Min minions for R").SetValue(new Slider(3, 1, 20)));


            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useR2KS", "Use R for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQR2KS", "Use Q+R for ks").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("qint", "Use Q To Intterupt")).SetValue(true);
            Config.SubMenu("Misc").AddItem(new MenuItem("rint", "Use R To Intterupt")).SetValue(true);

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

            Config.AddItem(new MenuItem("fleekey", "Use Flee Mode")).SetValue(new KeyBind(65, KeyBindType.Press));


            foreach (var obj in ObjectManager.Get<Obj_AI_Hero>().Where(obj => obj.Team != Player.Team))
            {
                Config.AddItem(new MenuItem("dograb" + obj.ChampionName, obj.ChampionName))
                    .SetValue(new StringList(new[] { "Dont Grab ", "Normal Grab "}, 1));
            }

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnPossibleToInterrupt += BlitzInterruptableSpell;
            Orbwalking.BeforeAttack += BeforeAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }
        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }


            KillSteal();
            Potion();


            if (Config.Item("UseQmanual").GetValue<KeyBind>().Active)
            {
                ManualQ();
            }

            if (Config.Item("fleekey").GetValue<KeyBind>().Active)
            {
                Flee();
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
            if (Config.Item("rDraw").GetValue<bool>() && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Gold);
            }
        }

        private static void Combo()
        {
            var qSpell = Config.Item("UseQ").GetValue<bool>();
            var eSpell = Config.Item("UseE").GetValue<bool>();
            var wSpell = Config.Item("UseW").GetValue<bool>();
            var rSpell = Config.Item("UseR").GetValue<bool>();
            var rcSlider = Config.Item("UseRc").GetValue<Slider>().Value;
            var qmSpell = Config.Item("UseQm").GetValue<Slider>().Value;
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(qmSpell)
                && (Config.Item("dograb" + target.ChampionName).GetValue<StringList>().SelectedIndex != 0))
            {
                Q.Cast(target);
            }

            if (eSpell
                && E.IsReady()
                && target.IsValidTarget(120)
                && !target.HasBuffOfType(BuffType.Knockup))
            {
                E.Cast();
            }
            if (wSpell
                && W.IsReady()
                && Player.Distance(target) >= 300)
            {
                W.Cast();
            }
            if (rSpell
                && R.IsReady()
                && target.IsValidTarget(R.Range)
                && Player.CountEnemiesInRange(R.Range) >= rcSlider)
            {
                R.Cast();
            }
        }

        private static void LaneClear()
        {
            var lmSpell = Config.Item("useLM").GetValue<Slider>().Value;
            var rSpell = Config.Item("useR2l").GetValue<bool>();
            var wSpell = Config.Item("useW2l").GetValue<bool>();
            var eSpell = Config.Item("useE2l").GetValue<bool>();
            var qmSpell = Config.Item("useRSlider").GetValue<Slider>().Value;
            var wmSpell = Config.Item("useWSlider").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, R.Range, MinionTypes.All, MinionTeam.NotAlly);
            var minionObj = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.NotAlly,
    MinionOrderTypes.MaxHealth);
            if (Player.ManaPercent <= lmSpell)
                return;
            {
                foreach (var minion in minionCount)
                {
                    if (rSpell
                        && minionObj.Count >= qmSpell
                        && minion.IsValidTarget(R.Range))
                    {
                        R.Cast(minion);
                    }
                    if (wSpell
                        && minionCount.Count >= wmSpell)
                    {
                        W.Cast();
                    }
                    if (eSpell
                        && minion.Health > Player.GetAutoAttackDamage(minion))
                    {
                        E.Cast();
                    }
                }
            }
        }

        private static void KillSteal()
        {
            var ks = Config.Item("KS").GetValue<bool>();
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();
            var rSpell = Config.Item("useR2KS").GetValue<bool>();
            var qrSpell = Config.Item("useQR2KS").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (!ks)
                return;
            if (qSpell
                && Q.IsReady()
                && target.IsValidTarget(Q.Range)
                && target.Health < Q.GetDamage(target))
            {
                Q.Cast(target);
            }
            if (rSpell
                && R.IsReady()
                && target.IsValidTarget(R.Range)
                && target.Health < R.GetDamage(target))
            {
                R.Cast();
            }
            if (qrSpell
                && Q.IsReady() && R.IsReady()
                && target.IsValidTarget(Q.Range)
                && target.Health < (R.GetDamage(target) + Q.GetDamage(target)))
            {
                Q.Cast(target);
                R.Cast(target);
            }
        }

        private static void Flee()
        {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (W.IsReady())
            {

                W.Cast();
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
        private static void BlitzInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Config.Item("qint").GetValue<bool>();
            var rSpell = Config.Item("rint").GetValue<bool>();
            if (qSpell)
            {
                Q.Cast(target);
            }

            if (rSpell)
            {
                if (unit.Distance(Player.ServerPosition, true) <= R.RangeSqr)
                {
                    R.Cast();
                }
            }
        }
        static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target.IsValid<Obj_AI_Hero>()
                && E.IsReady())
            {
                E.Cast();
            }
        }

        private static void ManualQ()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Q.IsReady()
                && (Config.Item("dograb" + target.ChampionName).GetValue<StringList>().SelectedIndex != 0))
            {
                Q.Cast(target);
            }
            
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId)
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
                    }
                }
            }
        }
    }
