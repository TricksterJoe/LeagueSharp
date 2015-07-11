using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;
using System.Drawing;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_Caitlyn
{
    internal class Program
    {
        public const string ChampName = "Caitlyn";
        public const string Menuname = "Slutty Caitlyn";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        public float QMana, EMana;

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

            Q = new Spell(SpellSlot.Q, 1280f);
            W = new Spell(SpellSlot.W, 800f);
            E = new Spell(SpellSlot.E, 980f);
            R = new Spell(SpellSlot.R, 3000f);


            Q.SetSkillshot(0.65f, 90f, 2200f, false, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.5f, 1f, 1750f, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 80f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 200f, 1500f, false, SkillshotType.SkillshotCircle);

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
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "W Damage").SetValue(true);
            var drawFill = new MenuItem("RushDrawWDamageFill", "W Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            Config.SubMenu("Drawings").AddItem(drawDamageMenu);
            Config.SubMenu("Drawings").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;

            drawDamageMenu.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
            };

            drawFill.ValueChanged +=
            delegate(object sender, OnValueChangeEventArgs eventArgs)
            {
                DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
            };

            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseQr", "Reduce Use Q Usage").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("UseRM", "Semi Manual R").SetValue(new KeyBind(68, KeyBindType.Press)));
            

            Config.AddSubMenu(new Menu("Harras", "Harras"));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQH", "Use Q").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQrh", "Reduce Use Q Usage").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("useH", "Harras if %Mana >").SetValue(new Slider(50)));

            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQSlider", "Min minions for Q").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useMSlider", "Lane Clear if %Mana >").SetValue(new Slider(50)));


            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useR2KS", "Use R").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("UseW", "Auto W").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("UseQa", "Auto Q On stunned").SetValue(true));


            Config.AddSubMenu(new Menu("Auto Potions", "autoP"));
            Config.SubMenu("autoP").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("autoP").AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion")).SetValue(new Slider(50));
            Config.SubMenu("autoP").AddItem(new MenuItem("MANA", "Auto Mana Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("MANASlider", "Minimum %Mana for Potion")).SetValue(new Slider(50));

            Config.AddItem(new MenuItem("dashte", "Dash EQ to on target")).SetValue(new KeyBind(67, KeyBindType.Press));

            Config.AddItem(new MenuItem("fleekey", "Use Flee Mode")).SetValue(new KeyBind(65, KeyBindType.Press));

         //   Config.AddItem(new MenuItem("dasht", "Dash E to mouse")).SetValue(new KeyBind(66, KeyBindType.Press));


            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Config.Item("fleekey").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (E.IsReady())
                {
                    Utility.DelayAction.Add(300, () => E.Cast(Game.CursorPos.Extend(Player.Position, 5000)));
                }
            }

            /*
            if (Config.Item("dasht").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                if (E.IsReady())
                {
                    E.Cast(Game.CursorPos.Extend(Player.Position, 5000));
                }
            }
             */

            if (Config.Item("dashte").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (E.IsReady() && Q.IsReady()
                    && Player.Mana > (Q.Instance.ManaCost + E.Instance.ManaCost))
                {
                    E.Cast(target.Position);
                    Q.Cast(target.Position);
                }
            }

            if (Config.Item("UseRM").GetValue<KeyBind>().Active
                && R.IsReady())
            {
                ManualR();
            }

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
                KillSteal();
            }
            AutoQ();
            Potion();
            AutoW();

        }

        private static void AutoQ()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            var qSpell = Config.Item("UseQa").GetValue<bool>();

            if (qSpell)
            {
                if ((target.HasBuffOfType(BuffType.Slow)
                    || target.HasBuffOfType(BuffType.Charm)
                    || target.HasBuffOfType(BuffType.Stun)
                    || target.HasBuffOfType(BuffType.Snare)
                    || target.HasBuffOfType(BuffType.Knockup)
                    || target.HasBuffOfType(BuffType.Suppression))
                    && !target.IsZombie)
                {
                    Q.Cast(target);
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
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.BlueViolet);
            }
        }

        private static void Combo()
        {
            var qSpell = Config.Item("UseQ").GetValue<bool>();
            var qrSpell = Config.Item("UseQr").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var minionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var qHit = Q.GetLineFarmLocation(minionsQ, 100);

            if ((target != null)
                && !target.IsZombie)
            {

                if (qSpell
                    && !qrSpell
                    && Q.IsReady()
                    && qHit.MinionsHit <= 4
                    && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (qSpell
                    && qrSpell
                    && Q.IsReady()
                    && Player.Distance(target) >= 500
                    && Player.Mana >= Q.Instance.ManaCost*2f
                    && Player.CountEnemiesInRange(1000) <= 3
                    && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void LaneClear()
        {
            var q2LSpell = Config.Item("useQ2L").GetValue<bool>();
            var qSlider = Config.Item("useQSlider").GetValue<Slider>().Value;
            var mSlider = Config.Item("useMSlider").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (Player.ManaPercent < mSlider)
                return;
            {
                    var mfarm = Q.GetLineFarmLocation(minionCount);
                    if (q2LSpell
                        && minionCount.Count >= qSlider
                        && mfarm.MinionsHit >= qSlider)
                    {
                        Q.Cast(mfarm.Position);
                    }
            }
        }

        private static void Mixed()
        {
            var qSpell = Config.Item("UseQH").GetValue<bool>();
            var qrSpell = Config.Item("UseQrh").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var minionsQ = MinionManager.GetMinions(ObjectManager.Player.ServerPosition, Q.Range);
            var qHit = Q.GetLineFarmLocation(minionsQ, 100);
            if (qSpell
                && !qrSpell
                && Q.IsReady()
                && qHit.MinionsHit <= 4
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (qSpell
                && qrSpell
                && Q.IsReady()
                && Player.Distance(target) > 800
                && qHit.MinionsHit <= 4
                && target.HealthPercent < Player.HealthPercent
                && target.IsFacing(Player)
                && Player.CountEnemiesInRange(1000) == 1
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
        }

        private static void KillSteal()
        {
            var ks = Config.Item("KS").GetValue<bool>();
            if (!ks)
                return;
            
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            var prediction = R.GetPrediction(target);
            var qcollision = R.GetCollision(Player.ServerPosition.To2D(),
            new List<Vector2> { prediction.CastPosition.To2D() });
            var playerncol = qcollision.Where(x => !(x is Obj_AI_Hero)).Count(x => x.IsTargetable);
            var rSpell = Config.Item("useR2KS").GetValue<bool>();
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();

            if (rSpell
                && R.IsReady()
                && target.IsValidTarget(R.Range)
                && Player.CountEnemiesInRange(2000) <= 3
                && playerncol == 0
                && target.Health < R.GetDamage(target)*0.6f)
            {
                R.CastOnUnit(target);
            }

            if (qSpell
                && Q.IsReady()
                && target.Health < Q.GetDamage(target)
                && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
        }

        private static void Potion()
        {
            var autoPotion = Config.Item("autoPO").GetValue<bool>();
            var hPotion = Config.Item("HP").GetValue<bool>();
            var mPotion = Config.Item("MANA").GetValue<bool>();
            var pSlider = Config.Item("HPSlider").GetValue<Slider>().Value;
            var mSlider = Config.Item("MANASlider").GetValue<Slider>().Value;
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
        }

        // NOT MINE, THIS IS SEBBY'S
        static double UnitIsImmobileUntil(Obj_AI_Base unit)
        {
            var time =
                unit.Buffs.Where(buff =>buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm
                        || buff.Type == BuffType.Knockup
                        || buff.Type == BuffType.Suppression 
                        || buff.Type == BuffType.Stun
                        || buff.Type == BuffType.Snare)).Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (time - Game.Time);
        }

        private static void AutoW()
        {
            foreach (
                var Object in
                    ObjectManager.Get<Obj_AI_Base>().Where(enemy => enemy.Distance(Player.ServerPosition) < W.Range
                                                                    && enemy.Team
                                                                    != Player.Team
                                                                    && (enemy.HasBuff("teleport_target", true))))
            {
                W.Cast(Object.Position, true);
            }

            var wSpell = Config.Item("UseW").GetValue<bool>();
            var qSpell = Config.Item("UseQa").GetValue<bool>();
            if (wSpell)
            {
                foreach (Obj_AI_Hero target in HeroManager.Enemies.Where(x => x.IsValidTarget(W.Range)))
                {
                    if (target != null)
                    {
                        if (UnitIsImmobileUntil(target) >= W.Delay - 0.5
                            && W.IsReady()
                            && target.IsValidTarget(W.Range)) 
                        W.Cast(target);
                    }
                    if (target != null
                        && qSpell)
                    {
                        if (UnitIsImmobileUntil(target) >= Q.Delay
                            && Q.IsReady()
                            && target.IsValidTarget(Q.Range))
                            Q.Cast(target);
                    }
                }

            }

        }
        static float GetComboDamage(Obj_AI_Base enemy)
        {
            foreach (var Buff in Player.Buffs)
            {
                if (Buff.Name == "caitlynheadshot")
                {
                    return ((float)Player.GetAutoAttackDamage(enemy, true))*0.5f;
                }
                if (Q.IsReady())
                {
                    return Q.GetDamage(enemy);
                }
            }

            return 0;
        }

        private static void ManualR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (target == null)
                return;

            if (target.IsValidTarget()
                && !target.IsZombie)
            {
                R.CastOnUnit(target);
            }
        }

    }
}
