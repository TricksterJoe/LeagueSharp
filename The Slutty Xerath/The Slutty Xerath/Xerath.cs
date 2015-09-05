using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
using ItemData = LeagueSharp.Common.Data.ItemData;

namespace The_Slutty_Xerath
{
    class Xerath : MenuConfigs
    {
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static string ChampName = "Xerath";
        private static int lastr;
        public static SpellSlot Ignite;
        public static int lastrr;

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        private static readonly Render.Text Text = new Render.Text(0, 0, "", 14, SharpDX.Color.Red, "monospace");
        private static readonly Color _color = Color.Red;
        private static readonly Color _fillColor = Color.Blue;
        private static bool hasbought;
        private static int lastbuff;

        internal static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;
            
            LoadMenu();

            // thanks eskor for values

            Q = new Spell(SpellSlot.Q, 1550);
            W = new Spell(SpellSlot.W, 1000);
            E = new Spell(SpellSlot.E, 1150);
            R = new Spell(SpellSlot.R);

            GlobalManager.DamageToUnit = GlobalManager.GetComboDamage;
            Q.SetSkillshot(0.6f, 100f, float.MaxValue, false, SkillshotType.SkillshotLine);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);
            W.SetSkillshot(0.7f, 125f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 120f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            Drawing.OnDraw += Drawing_OnDraw;
            Orbwalking.BeforeAttack += BeforeAttack;
            AntiGapcloser.OnEnemyGapcloser += gapcloser;
            Interrupter2.OnInterruptableTarget += InterruptableTarget;

            Game.OnUpdate += OnUpdate;
        }

        private static void BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            if (Player.HasBuff("xerathascended2onhit"))
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (GlobalManager.RCasted()
                    || Q.IsCharging
                    || (Q.IsReady() || W.IsReady() || E.IsReady()))
                {
                    args.Process = false;
                }
            }
        }

        private static void InterruptableTarget(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            var i = Config.Item("miscMenu.eint").GetValue<bool>();
            if (!i)
                return;
            if (sender.IsAlly || sender.IsMe)
                return;
            if (Player.Distance(sender) <= E.Range)
            {
                E.Cast(sender);
            }
        }

        private static void gapcloser(ActiveGapcloser gapcloser)
        {
            var e = Config.Item("miscMenu.egap").GetValue<bool>();
            if (!e)
                return;

            if (gapcloser.Sender.IsAlly || gapcloser.Sender.IsMe)
                return;

            if (Player.Distance(gapcloser.Sender) <= E.Range)
            {
                E.Cast(gapcloser.Sender);
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Config.Item("FillDamage").GetValue<bool>())
                return;

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = GlobalManager.DamageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;

                if (damage > unit.Health)
                {
                    Text.X = (int)barPos.X + XOffset;
                    Text.Y = (int)barPos.Y + YOffset - 13;
                    Text.text = "Killable With Combo Rotation " + (unit.Health - damage);
                    Text.OnEndScene();
                }
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, _color);
                if (Config.Item("RushDrawWDamageFill").GetValue<bool>())
                {
                    var differenceInHp = xPosCurrentHp - xPosDamage;
                    var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);
                    for (var i = 0; i < differenceInHp; i++)
                    {
                        Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, _fillColor);
                    }
                }
            }
        }

        public static void UltLeveler()
        {
            var level = Config.Item("miscMenu.autolevel").GetValue<bool>();
            if (!level)
                return;

            switch (Player.Level)
            {
                case 6:
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).Level != 1)
                    {
                        Player.Spellbook.LevelSpell(SpellSlot.R);
                    }
                    break;
                case 11:
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).Level != 2)
                    {
                        Player.Spellbook.LevelSpell(SpellSlot.R);
                    }
                    break;
                case 16:
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).Level != 3)
                    {
                        Player.Spellbook.LevelSpell(SpellSlot.R);
                    }
                    break;
            }
        }

        public static void ScrybingOrb()
        {
            var level = Config.Item("miscMenu.scrybebuylevel").GetValue<Slider>().Value;
            var buy = Config.Item("miscMenu.scrybebuy").GetValue<bool>();

            if (!buy)
                return;

            if (hasbought)
                return;

            if (!(Items.HasItem(ItemId.Scrying_Orb_Trinket.ToString()) ||
                  Items.HasItem(ItemId.Farsight_Orb_Trinket.ToString()))
                && Player.InShop() && Player.Level >= level)
            {
                Player.BuyItem(ItemId.Scrying_Orb_Trinket);
                   hasbought = true;
            }
        }

        protected static void Drawing_OnDrawChamp(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var draw = Config.Item("Draw").GetValue<bool>();
            var drawq = Config.Item("qDraw").GetValue<bool>();
            var draww = Config.Item("wDraw").GetValue<bool>();
            var drawe = Config.Item("eDraw").GetValue<bool>();
            if (!draw)
                return;


            if (drawq && Q.Level >= 1 && Q.IsReady())
            {
                Drawing.DrawCircle(Player.Position, Q.Range, Color.GreenYellow);
            }

            if (draww && W.Level >= 1 && W.IsReady())
            {
                Drawing.DrawCircle(Player.Position, W.Range, Color.Aqua);
            }

            if (drawe && E.Level >= 1 && E.IsReady())
            {
                Drawing.DrawCircle(Player.Position, E.Range, Color.Brown);
            }
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;
            {
                var color = Color.FromArgb(255, 255, 10, 255);
                //  Drawing.DrawCircle(Drawing.WorldToMinimap(Player.Position).To3D(), R.Range, Color.Black);
                Drawing.DrawCircle(R.GetPrediction(target).CastPosition, Q.Width, color);
            }

        }
        /*
        public  static int RRange()
        {
            if (Player.GetSpell(SpellSlot.R).Level == 1)
                return 500;

            if (Player.GetSpell(SpellSlot.R).Level == 2)
                return 1000;

            if (Player.GetSpell(SpellSlot.R).Level == 3)
                return 1500;

            return 0;
        }
         */
        private static void OnUpdate(EventArgs args)
        {
            //            foreach (var buff in Player.Buffs)
            //            {
            //                Game.PrintChat(buff.DisplayName);
            //            }
          //  RRange();
            
            R.Range = GlobalManager.RRange;
            ScrybingOrb();
            UltLeveler();
            UseR();
            Orbwalker.SetMovement(true);            
            if (GlobalManager.RCasted() && Config.Item("comboMenu.userblock").GetValue<bool>())
            {
                Orbwalker.SetMovement(false);
            }
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:
                    Laneclear();
                    //   Jungleclear();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                     Harras();
                    break;
            }
        }

        private static void Harras()
        {
            var useq = Config.Item("harassMenu.useq").GetValue<bool>();
            var usew = Config.Item("harassMenu.usew").GetValue<bool>();
            var usee = Config.Item("harassMenu.usee").GetValue<bool>();
            var mana = Config.Item("harassMenu.minmana").GetValue<Slider>().Value;
            AAMinion();
            var qtarget = TargetSelector.GetTarget(Q.ChargedMaxRange, TargetSelector.DamageType.Magical);
            if (Player.ManaPercent <= mana && !Q.IsCharging)
                return;

            if (qtarget == null)
                return;
            if (usee && qtarget.IsValidTarget(E.Range) && E.IsReady() && !Q.IsCharging)
            {
                E.Cast(qtarget);
            }

            if (useq)
            {
                if (Q.IsReady() && !Q.IsCharging)
                {
                    Q.StartCharging();
                }

                else
                {
                    Q.Cast(qtarget);
                }
            }

            if (usew && !Q.IsCharging && qtarget.IsValidTarget(Q.ChargedMaxRange))
            {
                W.Cast(qtarget);
            }
        }

        /*
        private static void Jungleclear()
        {
            var jungle = MinionManager.GetMinions(Player.Position, Q.ChargedMaxRange, MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            if (jungle[0] == null)
                return;

            var useq = Config.Item("jungleMenu.useq").GetValue<bool>();
            var mana = Config.Item("jungleMenu.minmana").GetValue<Slider>().Value;
            var usew = Config.Item("jungleMenu.usew").GetValue<bool>();

            if (Player.ManaPercent <= mana)
                return;


            if (usew && jungle[0].IsValidTarget())
            {
                if (!Q.IsCharging)
                {
                    W.Cast(jungle[0]);
                }
            }


            if (useq && jungle[0].IsValidTarget())
            {
                if (!Q.IsCharging)
                    Q.Cast(jungle[0].Position);

                else
                {
                    Q.Cast(jungle[0]);
                }
            }
        }
         */

        private static void Laneclear()
        {
            var mana = Config.Item("laneMenu.minmana").GetValue<Slider>().Value;

            if (Player.ManaPercent <= mana && !Q.IsCharging)
                return;

            var minion = MinionManager.GetMinions(Player.Position, Q.ChargedMaxRange, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (minion == null)
                return;

            var useq = Config.Item("laneMenu.useq").GetValue<bool>();
            var useqhit = Config.Item("laneMenu.useqhit").GetValue<Slider>().Value;
            var usew = Config.Item("laneMenu.usew").GetValue<bool>();
            var usewhit = Config.Item("laneMenu.usewhit").GetValue<Slider>().Value;
            var minions = MinionManager.GetMinions(Player.Position, W.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            var wfarm = W.GetCircularFarmLocation(minions, Q.Width);

            var qLine = Q.GetLineFarmLocation(
                MinionManager.GetMinionsPredictedPositions(MinionManager.GetMinions(Q.ChargedMaxRange),
                    Q.Delay, Q.Width, Q.Speed,
                    Player.Position, Q.ChargedMaxRange,
                    false, SkillshotType.SkillshotLine), Q.Width);


            foreach (var min in minions)
            {
                if (useq)
                {
                    if (!Q.IsCharging && qLine.MinionsHit >= useqhit)
                        Q.StartCharging();

                    else if (qLine.MinionsHit >= useqhit)
                    {
                        Q.Cast(qLine.Position);
                    }
                }

                if (usew && !Q.IsCharging && W.IsReady())
                {
                    if (wfarm.MinionsHit >= usewhit)
                    {
                        W.Cast(wfarm.Position);
                    }
                }
            }
        }

        public static void scrybeorbuse()
        {
            var enable = Config.Item("miscMenu.scrybe").GetValue<bool>();
            if (!enable)
                return;
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            if (Player.Distance(R.GetPrediction(target).CastPosition) > Player.AttackRange)
            {
                if (Items.HasItem(3342))
                {
                    Items.UseItem(3342, target.Position);
                }
                if (Items.HasItem(3363))
                {
                    Items.UseItem(3363, target.Position);
                }
            }
        }
        public static void AAMinion()
        {
            var enable = Config.Item("miscMenu.aaminion").GetValue<bool>();
            if (!enable)
                return;
            var minion = MinionManager.GetMinions(Player.Position, Player.AttackRange, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.None).FirstOrDefault();
            var qtarget = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (qtarget != null)
                return;

            if (minion == null)
                return;

            if (Q.IsCharging)
                return;

            if (Player.HasBuff("xerathascended2onhit") && Player.ManaPercent < 80)
            {
                havebuff = true;
            }
            if (havebuff)
            {
                Player.IssueOrder(GameObjectOrder.AttackUnit, minion);
                havebuff = false;
            }

        }
        private static void Combo()
        {
            var useq = Config.Item("comboMenu.useq").GetValue<bool>();
            var usew = Config.Item("comboMenu.usew").GetValue<bool>();
            var usee = Config.Item("comboMenu.usee").GetValue<bool>();

            var qtarget = TargetSelector.GetTarget(Q.ChargedMaxRange, TargetSelector.DamageType.Magical);
            AAMinion();

            if (qtarget == null)
                return;



            if (Ignite.IsReady() && qtarget.Health <= (Q.GetDamage(qtarget) + Player.GetAutoAttackDamage(qtarget)))
            {
                Player.Spellbook.CastSpell(Ignite, qtarget);
            }

            if (usee && qtarget.IsValidTarget(E.Range) && E.IsReady() && !Q.IsCharging)
            {
                E.Cast(qtarget);
            }

            if (useq)
            {
                if (Q.IsReady() && !Q.IsCharging)
                {
                    Q.StartCharging();
                }

                else
                {
                    Q.Cast(qtarget);
                }
            }

            if (usew && !Q.IsCharging && qtarget.IsValidTarget(Q.ChargedMaxRange))
            {
                W.Cast(qtarget);
            }
        }

        private static void UseR()
        {
            var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            
            if (Config.Item("comboMenu.usertarget").GetValue<bool>() && TargetSelector.SelectedTarget.IsValidTarget())
            {
                target = TargetSelector.GetSelectedTarget();
            }
             

            var user = Config.Item("comboMenu.user").GetValue<StringList>().SelectedIndex;
            var userdelay = Config.Item("comboMenu.userdelay").GetValue<Slider>().Value;
            var userrtap = Config.Item("comboMenu.usertap").GetValue<KeyBind>().Active;
            var rpred = R.GetPrediction(target);
            switch (user)
            {
                case 1:
                {
                    if (R.IsReady() && target.IsValidTarget(R.Range) && userrtap)
                    {
                        if (rpred.Hitchance >= HitChance.High)
                        {
                            R.Cast(rpred.CastPosition);
                            scrybeorbuse();
                            // R.Cast(target.ServerPosition);
                        }
                    }
                    break;
                }

                case 0:
                {
                    if (R.IsReady() && target.IsValidTarget(R.Range) && userrtap &&
                        Environment.TickCount - lastrr > userdelay)
                    {
                        if (rpred.Hitchance >= HitChance.High)
                        {
                            R.Cast(rpred.CastPosition);

                            lastrr = Environment.TickCount;
                            scrybeorbuse();
                        }
                    }
                    break;
                }
            }
        }

        public static bool havebuff { get; set; }
    }
}
