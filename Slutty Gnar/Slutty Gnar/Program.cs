using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using ItemData = LeagueSharp.Common.Data.ItemData;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace Slutty_Gnar
{
    internal class Program
    {
        public const string ChampName = "Gnar";
        public const string Menuname = "Slutty Gnar";
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;

        private static Spell Q
        {
            get { return Gnar_Spells.Q; }
        }

        private static Spell W
        {
            get { return Gnar_Spells.W; }
        }

        private static Spell E
        {
            get { return Gnar_Spells.E; }
        }

        private static Spell R
        {
            get { return Gnar_Spells.R; }
        }

        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static Items.Item HealthPotion = new Items.Item(2003);
        public static Items.Item CrystallineFlask = new Items.Item(2041);
        public static Items.Item ManaPotion = new Items.Item(2004);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010);

        public static void UseItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }

        public static bool CanUseItem(int id)
        {
            return Items.HasItem(id) && Items.CanUseItem(id);
        }

        public static int[] AbilitySequence;
        public static int QOff = 0, WOff = 0, EOff = 0, ROff = 0;

        private static float _lastCheckTick;
        private static float _lastCheckTick2;

        private static SpellSlot Ignite;

        private static Obj_AI_Hero player;


        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {


            if (Player.ChampionName != ChampName)
                return;

            AbilitySequence = new[] {1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 2, 2, 3, 3, 4, 3, 3};

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


            Config.AddSubMenu(new Menu("Both Forms", "bForm"));
            Config.SubMenu("bForm").AddItem(new MenuItem("UseIgnite", "Use Ignite").SetValue(true));
            Config.SubMenu("bForm").AddItem(new MenuItem("Yes", "Use Items").SetValue(true));
            Config.SubMenu("bForm").AddItem(new MenuItem("UseBotrk", "Use Botrk").SetValue(true));
            Config.SubMenu("bForm").AddItem(new MenuItem("UseYumm", "Use Youmuu's ghostblade").SetValue(true));

            Config.AddSubMenu(new Menu("Mini Gnar", "mGnar"));
            Config.SubMenu("mGnar").AddItem(new MenuItem("UseQMini", "Use Q").SetValue(true));
            Config.SubMenu("mGnar").AddItem(new MenuItem("UseQs", "Use Q only when target has 2 W Stacks").SetValue(false));
            Config.SubMenu("mGnar").AddItem(new MenuItem("eGap", "Use E Gap closer when enemy is killable").SetValue(false));
            Config.SubMenu("mGnar").AddItem(new MenuItem("focust", "Focus Target with 2 W Stacks").SetValue(false));

            Config.AddSubMenu(new Menu("Mega Gnar", "megaGnar"));
            Config.SubMenu("megaGnar").AddItem(new MenuItem("UseQMega", "Use Q").SetValue(true));
            Config.SubMenu("megaGnar").AddItem(new MenuItem("UseEMega", "Use E").SetValue(true));
            Config.SubMenu("megaGnar").AddItem(new MenuItem("UseEMini", "Use E Only when about to transform").SetValue(true));
            Config.SubMenu("megaGnar").AddItem(new MenuItem("UseWMega", "Use W").SetValue(true));
            Config.SubMenu("megaGnar").AddItem(new MenuItem("useRMega", "Use R").SetValue(true));
            Config.SubMenu("megaGnar").AddItem(new MenuItem("useRSlider", "Min targets R").SetValue(new Slider(3, 1, 5)));


            Config.AddSubMenu(new Menu("Harras", "Harras"));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQMi", "Use Q Mini Gnar").SetValue(true));
            Config.SubMenu("Harras").AddItem(new MenuItem("UseQMe", "Use Q Mega Gnar").SetValue(true));

            Config.AddSubMenu(new Menu("Lane Clear", "Lane Clear"));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("useQ2c", "Use Q to last hit").SetValue(true));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("useQ2s", "Use Q Only when minion has 2 W stacks").SetValue(false));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("useQPL", "Min Minions for Q Mega Gnar").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("useW2L", "Use W to lane clear").SetValue(true));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("useWSlider", "Min minions for W").SetValue(new Slider(3, 1, 20)));
            Config.SubMenu("Lane Clear").AddItem(new MenuItem("abtT", "Don't Use Spells When about to transform").SetValue(true));

            Config.AddSubMenu(new Menu("JungleClear", "JungleClear"));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("juseQ2L", "Use Q").SetValue(true));
            Config.SubMenu("JungleClear")
                .AddItem(new MenuItem("juseQ2s", "Use Q Only when minion has 2 W stacks").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("juseW2L", "Use W").SetValue(true));
            Config.SubMenu("JungleClear")
                .AddItem(new MenuItem("jabtT", "Don't Use Spells When about to transform").SetValue(true));

            Config.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));

            Config.AddSubMenu(new Menu("Auto Potions", "Auto Potions"));
            Config.SubMenu("Auto Potions").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("Auto Potions").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("Auto Potions")
                .AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion"))
                .SetValue(new Slider(50));
            Config.SubMenu("Auto Potions").AddItem(new MenuItem("Biscuit", "Auto Biscuit").SetValue(true));
            Config.SubMenu("Auto Potions")
                .AddItem(new MenuItem("bSlider", "Minimum %Health for Biscuit"))
                .SetValue(new Slider(50));
            Config.SubMenu("Auto Potions").AddItem(new MenuItem("flask", "Auto Flask").SetValue(true));
            Config.SubMenu("Auto Potions")
                .AddItem(new MenuItem("fSlider", "Minimum %Health for flask"))
                .SetValue(new Slider(50));

            Config.SubMenu("Auto Level").AddItem(new MenuItem("Level", "Auto level up").SetValue(true));

            Config.AddItem(new MenuItem("fleekey", "Use Flee Mode")).SetValue(new KeyBind(65, KeyBindType.Press));

            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnUpdate;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void Game_OnUpdate(EventArgs args)
        {


            if (Player.IsDead)
                return;

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
                JungleClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                KillSteal();
            }
            var autoLevel = Config.Item("Level").GetValue<bool>();

            if (autoLevel)
            {
                SpellsLevel();
            }
            var qSpell = Config.Item("focust").GetValue<bool>();
            if (qSpell)
            {
                var target = HeroManager.Enemies.Find(en => en.IsValidTarget(ObjectManager.Player.AttackRange) 
                    && en.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2));
                if (target != null)
                {
                    Orbwalker.ForceTarget(target);
                    Hud.SelectedUnit = target;
                }
            }
            if (Environment.TickCount - _lastCheckTick < 150)
            {
                return;
            }

            Potion();
            _lastCheckTick = Environment.TickCount;


        }
        static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var useQ = Config.Item("UseQMini").GetValue<bool>();
            var useQm = Config.Item("UseQMega").GetValue<bool>();
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId
                && Player.IsMiniGnar())
            {

                if (useQ 
                    && Q.IsReady() 
                    && args.EndPos.Distance(Player) <= Q.Range)
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
                if (sender.NetworkId == target.NetworkId
                    && Player.IsMegaGnar())
                {

                    if (useQm
                        && Q.IsReady()
                        && args.EndPos.Distance(Player) <= Q.Range)
                    {
                        var delay = (int) (args.EndTick - Game.Time - Q.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay*1000, () => Q.Cast(args.EndPos));
                        }
                        else
                        {
                            Q.Cast(args.EndPos);
                        }
                    }
                }
            }
        }
        static float GetComboDamage(Obj_AI_Base enemy)
        {
            foreach (var Buff in enemy.Buffs)
            {
                if (Buff.Name == "gnarwproc" && Buff.Count == 2)
                {
                    return W.GetDamage(enemy) + (float)Player.GetAutoAttackDamage(enemy, true);
                }
            }

            return 0;
        }

        private static void Combo()
        {
            if (Config.Item("Yes").GetValue<bool>())
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                if (Player.HealthPercent < target.HealthPercent
                    && (CanUseItem(3153) || CanUseItem(3144))
                    && Config.Item("UseBotrk").GetValue<bool>())
                {
                    UseItem(3144, target);

                    UseItem(3153, target);
                }
                if (CanUseItem(3142)
                    && Player.Distance(target.Position) < Player.AttackRange
                    && Config.Item("UseYumm").GetValue<bool>())
                {
                    UseItem(3142);
                }
            }
            if (Player.IsMiniGnar())
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                var qSpell = Config.Item("UseQMini").GetValue<bool>();
                var qsSpell = Config.Item("UseQs").GetValue<bool>();
                var eSpell = Config.Item("eGap").GetValue<bool>();

                if (qSpell
                    && !qsSpell
                    && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }

                if (qSpell
                    && qsSpell
                    && target.IsValidTarget(Q.Range)
                    && target.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                {
                    Q.Cast(target);
                }
                if (eSpell
                    && Player.CountEnemiesInRange(1600) == 1
                    && target.IsValidTarget(Q.Range))
                {
                    var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.All);
                    foreach (var minion in minionCount)
                    {
                        var minionPrediction = E.GetPrediction(minion);
                        var k =
                            ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                          && x.IsMinion
                                                                          && x.Distance(Player) <= E.Range)
                                .OrderBy(x => x.Distance(target))
                                .FirstOrDefault();
                        var edm = Player.ServerPosition.Extend(minionPrediction.CastPosition,
                            Player.ServerPosition.Distance(minionPrediction.CastPosition) + E.Range);
                        if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion != Player.IsMinion
                                                                            && !type.IsDead
                                                                            && type.Distance(edm, true) < 775*775
                                                                            && k.IsValid)
                            && Player.Distance(target) > 300)
                        {
                            E.Cast(k);
                        }
                    }
                }
            }
            if (Player.IsMegaGnar())
            {
                {
                    var rSlider = Config.Item("useRSlider").GetValue<Slider>().Value;
                    Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    var qmSpell = Config.Item("UseQMega").GetValue<bool>();
                    if (R.IsReady()
                        && !Gnar_Spells.HasCastedStun
                        && Config.Item("useRMega").GetValue<bool>())
                    {
                        if (target != null
                            && Player.CountEnemiesInRange(420) >= rSlider)
                        {
                            var prediction = Prediction.GetPrediction(target, R.Delay);
                            if (R.IsInRange(prediction.UnitPosition))
                            {
                                var direction = (Player.ServerPosition - prediction.UnitPosition).Normalized();
                                var maxAngle = 180f;
                                var step = maxAngle/6f;
                                var currentAngle = 0f;
                                var currentStep = 0f;
                                while (true)
                                {

                                    if (currentStep > maxAngle && currentAngle < 0)
                                        break;

                                    if ((currentAngle == 0 || currentAngle < 0) && currentStep != 0)
                                    {
                                        currentAngle = (currentStep)*(float) Math.PI/180;
                                        currentStep += step;
                                    }
                                    else if (currentAngle > 0)
                                        currentAngle = -currentAngle;

                                    Vector3 checkPoint;
                                    if (currentStep == 0)
                                    {
                                        currentStep = step;
                                        checkPoint = prediction.UnitPosition + 500*direction;
                                    }
                                    else
                                        checkPoint = prediction.UnitPosition + 500*direction.Rotated(currentAngle);

                                    if (prediction.UnitPosition.GetFirstWallPoint(checkPoint).HasValue
                                        && !target.IsStunned
                                        && target.Health >= (Q.GetDamage(target) + Player.GetAutoAttackDamage(target)))
                                    {
                                        R.Cast(Player.Position + 500*(checkPoint - prediction.UnitPosition).Normalized());
                                        break;
                                    }
                                }
                            }
                        }
                    }
                        
                    if (Player.IsMegaGnar()
                        && !R.IsReady())
                    {
                        var emSpell = Config.Item("UseEMega").GetValue<bool>();
                        if (E.IsReady()
                            && emSpell
                            && !Config.Item("UseEMini").GetValue<bool>())
                        {
                            if (target != null
                                && target.HealthPercent <= Player.HealthPercent)
                            {
                                E.Cast(target);
                            }
                        }
                        var wSpell = Config.Item("UseWMega").GetValue<bool>();
                        if (wSpell)
                        {
                            var targetw = W.GetTarget();
                            if (targetw != null)
                            {
                                {
                                    W.Cast(targetw);
                                }
                            }
                        }

                        if (qmSpell
                            && target.IsValidTarget(Q.Range))
                        {
                            Q.Cast(target);
                        }
                    }
                }
                if (Config.Item("UseEMini").GetValue<bool>() && (Player.IsMegaGnar() || Player.IsAboutToTransform()))
                {
                    Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                    var targete = E.GetTarget(E.Width / 2);
                    if (targete != null)
                    {
                        var prediction = E.GetPrediction(target);
                        var ed = Player.ServerPosition.Extend(prediction.CastPosition,
                            Player.ServerPosition.Distance(prediction.CastPosition) + E.Range);

                        if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.Team != Player.Team
                                                                            && !type.IsDead
                                                                            && type.Distance(ed, true) < 775 * 775))
                        {
                            E.Cast(prediction.CastPosition);
                        }
                    }
                }
            }
        }

        private static void LaneClear()
        {
            var uSpell = Config.Item("abtT").GetValue<bool>();
            var q2LSpell = Config.Item("useQ2L").GetValue<bool>();
            var q2CSpell = Config.Item("useQ2c").GetValue<bool>();
            var q2SSpell = Config.Item("useQ2s").GetValue<bool>();
            var eqSlider = Config.Item("useQPL").GetValue<Slider>().Value;
            var wlSpell = Config.Item("useW2L").GetValue<bool>();
            var elSlider = Config.Item("useWSlider").GetValue<Slider>().Value;
            var MinionN =
                MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral, MinionOrderTypes.MaxHealth)
                    .FirstOrDefault();
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (uSpell
                && Player.IsAboutToTransform())
                return;
            if (MinionN.IsValidTarget())
            {
                JungleClear();
                return;
            }

            {
                foreach (var minion in minionCount)
                {

                    if (Player.IsMiniGnar()
                        && !Player.IsAboutToTransform())
                    {
                        var prediction = Q.GetPrediction(minion);
                        var qcollision = Q.GetCollision(Player.ServerPosition.To2D(),
                            new List<Vector2> {prediction.CastPosition.To2D()});
                        var minioncol = qcollision.Where(x => !(x is Obj_AI_Hero)).Count(x => x.IsMinion);
                        if (q2LSpell
                            && minioncol >= 1
                            && minionCount.Count >= eqSlider
                            && q2SSpell
                            && minion.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                        {
                            Q.Cast(prediction.CastPosition);
                        }
                        if (q2LSpell
                            && minioncol >= 1
                            && minionCount.Count >= eqSlider
                            && q2CSpell
                            && Q.GetDamage(minion) > minion.Health)
                        {
                            Q.Cast(prediction.CastPosition);
                        }
                        if (q2LSpell
                            && !q2SSpell
                            && minioncol >= 1
                            && minionCount.Count >= eqSlider)
                        {
                            Q.Cast(prediction.CastPosition);
                        }

                    }
                    if (Player.IsMegaGnar())
                    {
                        var minions =
                            MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy,
                                MinionOrderTypes.MaxHealth).Where(m =>
                                    m.Health > player.GetAutoAttackDamage(m)/2 && m.Health < Q.GetDamage(m));
          
                        if (q2LSpell)
                        {
                            var position = Q.GetFarmLocation(MinionTeam.Enemy, minions.ToList());
                            Q.Cast(position.Value.Position);
                        }
                        if (wlSpell)
                        {
                            var positions = W.GetFarmLocation();
                            if (minionCount.Count >= elSlider
                                && minion.Health > (Player.GetAutoAttackDamage(minion) + W.GetDamage(minion)))
                            {

                                {
                                    W.Cast(positions.Value.Position);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void Mixed()
        {
            if (Player.IsMiniGnar())
            {
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                var prediction = Q.GetPrediction(target);
                var qcollision = Q.GetCollision(Player.ServerPosition.To2D(),
                new List<Vector2> {prediction.CastPosition.To2D()});
                var minioncol = qcollision.Where(x => !(x is Obj_AI_Hero)).Count(x => x.IsMinion);
                var qSpell = Config.Item("UseQMi").GetValue<bool>();
                if (qSpell
                    && target.IsValidTarget(Q.Range)
                    && minioncol <= 1)
                {
                    Q.Cast(prediction.CastPosition);
                }
            }

            if (Player.IsMegaGnar())
            {

                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
                var qmSpell = Config.Item("UseQMe").GetValue<bool>();
                if (qmSpell
                    && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
            }
        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            var prediction = Q.GetPrediction(target);
            var qcollision = Q.GetCollision(Player.ServerPosition.To2D(),
            new List<Vector2> {prediction.CastPosition.To2D()});
            var minioncol = qcollision.Where(x => !(x is Obj_AI_Hero)).Count(x => x.IsMinion);
            var ks = Config.Item("KS").GetValue<bool>();
            var qSpell = Config.Item("useQ2KS").GetValue<bool>();
            if (!ks)
                return;
            if (Player.IsMiniGnar()
                && !Player.IsAboutToTransform())
            {
                if (qSpell
                    && minioncol <= 1
                    && Q.GetDamage(target) > target.Health)
                {
                    Q.Cast(prediction.CastPosition);
                }
            }
            if (Player.IsMegaGnar())
            {
                if (qSpell
                    && Q.GetDamage(target) > target.Health)
                {
                    Q.Cast(prediction.CastPosition);
                }
            }
        }

        private static void SpellsLevel()
        {
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = {0, 0, 0, 0};
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[AbilitySequence[i] - 1] = level[AbilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);
            }
        }

        private static void JungleClear()
        {
            var q2LSpell = Config.Item("juseQ2L").GetValue<bool>();
            var q2SSpell = Config.Item("juseQ2s").GetValue<bool>();
            var wlSpell = Config.Item("juseW2L").GetValue<bool>();
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.Neutral);
            foreach (var minion in minionCount)
            {
                {
                    if (minion == null
                        || !minion.IsValidTarget())
                    {
                        LaneClear();
                        return;
                    }
                    var prediction = Q.GetPrediction(minion);
                    if (q2LSpell
                        && q2SSpell
                        && minion.Buffs.Any(buff => buff.Name == "gnarwproc" && buff.Count == 2))
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                    if (q2LSpell
                        && Q.GetDamage(minion) > minion.Health)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                    if (q2LSpell
                        && !q2SSpell)
                    {
                        Q.Cast(prediction.CastPosition);
                    }

                }
                if (Player.IsMegaGnar()
                    || !Player.IsAboutToTransform())
                {
                    var prediction = Q.GetPrediction(minion);
                    if (q2LSpell)
                    {
                        Q.Cast(prediction.CastPosition);
                    }
                    var predictionW = W.GetPrediction(minion);
                    if (wlSpell)
                    {
                        W.Cast(predictionW.CastPosition);
                    }
                }
            }
        }

        private static void Potion()
        {
            var autoPotion = Config.Item("autoPO").GetValue<bool>();
            var hPotion = Config.Item("HP").GetValue<bool>();
            var bPotion = Config.Item("Biscuit").GetValue<bool>();
            var fPotion = Config.Item("flask").GetValue<bool>();
            var pSlider = Config.Item("HPSlider").GetValue<Slider>().Value;
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

        private static void Flee()
        {

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

                var prediction = E.GetPrediction(target);
                var ed = Player.ServerPosition.Extend(prediction.CastPosition,
                    Player.ServerPosition.Distance(prediction.CastPosition) + E.Range);

                if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.Team != Player.Team
                                                                    && !type.IsDead
                                                                    && type.Distance(ed, true) < 775*775))
                {
                    E.Cast(prediction.CastPosition);
                }
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.All);
            foreach (var minion in minionCount)
            {
                var minionPrediction = E.GetPrediction(minion);
                var k =
                    ObjectManager.Get<Obj_AI_Minion>().Where(x => Player.IsFacing(x)
                                                                  && x.IsMinion
                                                                  && x.Distance(Player) <= E.Range)
                        .OrderByDescending(x => x.Distance(Player))
                        .First();

                var edm = Player.ServerPosition.Extend(minionPrediction.CastPosition,
                    Player.ServerPosition.Distance(minionPrediction.CastPosition) + E.Range);
                if (!ObjectManager.Get<Obj_AI_Turret>().Any(type => type.IsMinion != Player.IsMinion
                                                                    && !type.IsDead
                                                                    && type.Distance(edm, true) < 775*775
                                                                    && k.IsValid))
                {
                    E.Cast(k);
                }
            }

        }

        //private static void OrbwalkingAfterAttack(AttackableUnit unit, AttackableUnit target)
        //{

           // if (!(target is Obj_AI_Base) || !unit.IsMe)
           // {
           //     return;
           // }
           // var tg = (Obj_AI_Base)target;
       // }


    }
}
