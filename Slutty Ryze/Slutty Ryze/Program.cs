using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Dynamic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common.Data;
using SharpDX.Win32;
using ItemData = LeagueSharp.Common.Data.ItemData;
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
        public static Spell Q, W, E, R, Qn;
        private static SpellSlot Ignite;

        private static readonly Obj_AI_Hero Player = ObjectManager.Player;

        public static int[] abilitySequence;
        public static int qOff = 0, wOff = 0, eOff = 0, rOff = 0;

        public static int Muramana = 3042;
        public static Items.Item HealthPotion = new Items.Item(2003);
        public static Items.Item CrystallineFlask = new Items.Item(2041);
        public static Items.Item ManaPotion = new Items.Item(2004);
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010);
        public static Items.Item SeraphsEmbrace = new Items.Item(3040, 0);

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;

            Q = new Spell(SpellSlot.Q, 865);
            Qn = new Spell(SpellSlot.Q, 865);
            W = new Spell(SpellSlot.W, 585);
            E = new Spell(SpellSlot.E, 585);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.26f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            Qn.SetSkillshot(0.26f, 50f, 1700f, false, SkillshotType.SkillshotLine);

            abilitySequence = new int[] {1, 2, 3, 1, 1, 4, 1, 2, 1, 2, 4, 3, 2, 2, 3, 4, 3, 3};

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
            Config.SubMenu("Drawings").AddItem(new MenuItem("notdraw", "Float Text").SetValue(true));
            var drawDamageMenu = new MenuItem("RushDrawEDamage", "Combo damage").SetValue(true);
            var drawFill =
                new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(new Circle(true, Color.SeaGreen));
            Config.SubMenu("Drawings").AddItem(drawDamageMenu);
            Config.SubMenu("Drawings").AddItem(drawFill);

            DamageIndicator.DamageToUnit = GetComboDamage;
            DamageIndicator.Enabled = drawDamageMenu.GetValue<bool>();
            DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
            DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;


            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo")
                .AddItem(
                    new MenuItem("combooptions", "Combo Mode").SetValue(new StringList(new[] {"Stable", "Beta Combo"}, 0)));
            Config.SubMenu("Combo").AddItem(new MenuItem("useQ", "Use Q (Over Load)").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useW", "Use W (Rune Prison)").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useE", "Use E (Spell Flux)").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R (Desperate Power)").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("useRww", "Only R if Target Is Rooted").SetValue(true));

            Config.AddSubMenu(new Menu("Combo Options", "ComboOptions"));
            Config.SubMenu("ComboOptions")
                .AddItem(new MenuItem("AAblock", "Block auto attack in combo").SetValue(false));

            Config.AddSubMenu(new Menu("Mixed", "Mixed"));

            Config.SubMenu("Mixed")
                .AddItem(new MenuItem("mMin", "Minimum Mana For Spells").SetValue(new Slider(40)));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseQM", "Use Q").SetValue(true));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseQMl", "Use Q last hit minion").SetValue(true));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseEM", "Use E").SetValue(false));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseWM", "Use W").SetValue(false));
            Config.SubMenu("Mixed").AddItem(new MenuItem("UseQauto", "Auto Q").SetValue(false));


            Config.AddSubMenu(new Menu("Lane Clear", "LaneClear"));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("disablelane", "Lane Clear Toggle").SetValue(new KeyBind('T', KeyBindType.Toggle)));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("presslane", "Press Lane Clear").SetValue(new KeyBind('H', KeyBindType.Press)));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("useEPL", "Minimum %Mana For Lane Clear").SetValue(new Slider(50)));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("passiveproc", "Don't Use Spells If Passive Will Proc").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQlc", "Use Q Last Hit").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useWlc", "Use W Last Hit").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useElc", "Use E Last Hit").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useQ2L", "Use Q To Lane Clear").SetValue(true));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useW2L", "Use W To Lane Clear").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useE2L", "Use E To Lane Clear").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("useRl", "Use R In Lane Clear").SetValue(false));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("rMin", "Minimum Minions For R").SetValue(new Slider(3, 1, 20)));

            Config.AddSubMenu(new Menu("Last Hit", "LastHit"));
            Config.SubMenu("LastHit").AddItem(new MenuItem("useQl2h", "Use Q Last Hit").SetValue(true));
            Config.SubMenu("LastHit").AddItem(new MenuItem("useWl2h", "Use W Last Hit").SetValue(false));
            Config.SubMenu("LastHit").AddItem(new MenuItem("useEl2h", "Use E Last Hit").SetValue(false));

            // Config.SubMenu("LaneClear").AddItem(new MenuItem("seplane", "Seperate Lane Clear Key").SetValue(new KeyBind('V', KeyBindType.Press)));

            Config.AddSubMenu(new Menu("Jungle Clear", "JungleClear"));
            Config.SubMenu("JungleClear")
                .AddItem(new MenuItem("useJM", "Minimum Mana For Jungle Clear").SetValue(new Slider(50)));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("useQj", "Use Q").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("useWj", "Use W").SetValue(true));
            Config.SubMenu("JungleClear").AddItem(new MenuItem("useEj", "Use E").SetValue(true));


            Config.AddSubMenu(new Menu("Items", "Items"));
            Config.SubMenu("Items")
                .AddItem(new MenuItem("tearS", "Stack Tear").SetValue(new KeyBind('G', KeyBindType.Toggle)));
            Config.SubMenu("Items").AddItem(new MenuItem("tearoptions", "Stack Tear Only in Fountain").SetValue(false));
            Config.SubMenu("Items").AddItem(new MenuItem("tearSM", "Min Mana").SetValue(new Slider(95)));
            Config.SubMenu("Items").AddItem(new MenuItem("staff", "Use Seraphs Embrace").SetValue(true));
            Config.SubMenu("Items").AddItem(new MenuItem("staffhp", "Seraph's when %HP >").SetValue(new Slider(30)));
            Config.SubMenu("Items").AddItem(new MenuItem("muramana", "Muramana").SetValue(true));

            Config.AddSubMenu(new Menu("Misc", "Misc"));
            Config.SubMenu("Misc").AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("useQW2D", "W/Q On Dashing").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("level", "Auto Skill Level Up").SetValue(true));
            Config.SubMenu("Misc").AddItem(new MenuItem("autow", "Auto W enemy under turret").SetValue(true));


            Config.AddSubMenu(new Menu("Kill Steal", "KillSteal"));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useW2KS", "Use W for ks").SetValue(true));
            Config.SubMenu("KillSteal").AddItem(new MenuItem("useE2KS", "Use E for ks").SetValue(true));

            Config.AddSubMenu(new Menu("Auto Potions", "autoP"));
            Config.SubMenu("autoP").AddItem(new MenuItem("autoPO", "Auto Health Potion").SetValue(true));
            Config.SubMenu("autoP").AddItem(new MenuItem("HP", "Health Potions")).SetValue(true);
            Config.SubMenu("autoP")
                .AddItem(new MenuItem("HPSlider", "Minimum %Health for Potion"))
                .SetValue(new Slider(30));
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

            Config.AddSubMenu(new Menu("Passive Stack", "autoPassive"));
            Config.SubMenu("autoPasive")
                .AddItem(new MenuItem("ManapSlider", "Minimum %Mana"))
                .SetValue(new Slider(30));
            Config.SubMenu("autoPassive")
                .AddItem(new MenuItem("autoPassive", "Stack Passive").SetValue(new KeyBind('Z', KeyBindType.Toggle)));
            Config.SubMenu("autoPassive")
                .AddItem(new MenuItem("stackSlider", "Keep passive count at"))
                .SetValue(new Slider(3, 1, 4));
            Config.SubMenu("autoPassive").AddItem(new MenuItem("stackMana", "Minimum %Mana")).SetValue(new Slider(50));

            Config.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnPossibleToInterrupt += RyzeInterruptableSpell;
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;

        }

        private static void Game_OnUpdate(EventArgs args)
        {

            if (Player.IsDead)
                return;
            Orbwalker.SetAttack(true);

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);


            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                if (((Player.Distance(target) > 440)
                     || (Q.IsReady() || E.IsReady() || W.IsReady()))
                    && target.Health > (Player.GetAutoAttackDamage(target)*3))
                {
                    Orbwalker.SetAttack(false);
                }
                else
                {
                    Orbwalker.SetAttack(true);
                }

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
                if (Config.Item("disablelane").GetValue<KeyBind>().Active)
                {
                    LaneClear();
                }

                if (Config.Item("presslane").GetValue<KeyBind>().Active)
                {
                    LaneClear();
                }

                Orbwalker.SetAttack(true);
                JungleClear();

            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
            {
                LastHit();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                if (Config.Item("tearS").GetValue<KeyBind>().Active)
                {
                    TearStack();
                }
                if (Config.Item("autoPassive").GetValue<KeyBind>().Active)
                {
                    AutoPassive();
                }

                Potion();
                Orbwalker.SetAttack(true);
            }

            if (Config.Item("UseQauto").GetValue<bool>())
            {
                if (target == null)
                    return;

                if (Q.IsReady()
                    && target.IsValidTarget(Q.Range))
                {
                    Q.Cast(target);
                }
            }


            // Seplane();
            Item();
            KillSteal();
            Potion();

            if (Config.Item("level").GetValue<bool>())
            {
                LevelUpSpells();
            }
            if (Config.Item("autow").GetValue<bool>()
                && target.UnderTurret(true))
            {
                if (target == null)
                    return;

                if (ObjectManager.Get<Obj_AI_Turret>()
                    .Any(turret => turret.IsValidTarget(300) && turret.IsAlly && turret.Health > 0))
                {
                    W.CastOnUnit(target);
                }
            }
        }

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
                return 0f;
            return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }


        /*
        private static void Seplane()
        {
            if (Player.IsValid &&
                Config.Item("seplane").GetValue<KeyBind>().Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                LaneClear();
            }
        }
         */

        private static void LevelUpSpells()
        {
            int qL = Player.Spellbook.GetSpell(SpellSlot.Q).Level + qOff;
            int wL = Player.Spellbook.GetSpell(SpellSlot.W).Level + wOff;
            int eL = Player.Spellbook.GetSpell(SpellSlot.E).Level + eOff;
            int rL = Player.Spellbook.GetSpell(SpellSlot.R).Level + rOff;
            if (qL + wL + eL + rL < ObjectManager.Player.Level)
            {
                int[] level = {0, 0, 0, 0};
                for (int i = 0; i < ObjectManager.Player.Level; i++)
                {
                    level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                }
                if (qL < level[0]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.Q);
                if (wL < level[1]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.W);
                if (eL < level[2]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.E);
                if (rL < level[3]) ObjectManager.Player.Spellbook.LevelSpell(SpellSlot.R);

            }
        }

        private static void RyzeInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var wSpell = Config.Item("useW2I").GetValue<bool>();
            if (wSpell)
            {
                W.CastOnUnit(target);
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
                        var delay = (int) (args.EndTick - Game.Time - Q.Delay - 0.1f);
                        if (delay > 0)
                        {
                            Utility.DelayAction.Add(delay*1000, () => Q.Cast(args.EndPos));
                        }
                        else
                        {
                            Q.Cast(args.EndPos);
                        }
                        if (Q.IsReady()
                            && args.EndPos.Distance(Player) < Q.Range)
                        {
                            if (delay > 0)
                            {
                                Utility.DelayAction.Add(delay*1000, () => Q.Cast(args.EndPos));
                            }
                            else
                            {
                                W.CastOnUnit(target);
                            }
                        }
                    }
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
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);
            }
            var tears = Config.Item("tearS").GetValue<KeyBind>().Active;
            var passive = Config.Item("autoPassive").GetValue<KeyBind>().Active;
            var laneclear = Config.Item("disablelane").GetValue<KeyBind>().Active;

            if (Config.Item("notdraw").GetValue<bool>())
            {
                if (tears)
                {
                    var heroPosition = Drawing.WorldToScreen(Player.Position);
                    var textDimension = Drawing.GetTextExtent("Stunnable!");
                    Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                        Color.DarkGreen, "Tear Stack: On");
                }

                if (passive)
                {
                    var heroPosition = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(heroPosition.X - 150, heroPosition.Y - 30,
                        Color.DarkGreen, "Passive Stack: On");
                }

                if (laneclear)
                {
                    var heroPosition = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30,
                        Color.DarkGreen, "Lane Clear: On");
                }

                if (!tears)
                {
                    var heroPosition = Drawing.WorldToScreen(Player.Position);
                    var textDimension = Drawing.GetTextExtent("Stunnable!");
                    Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                        Color.Red, "Tear Stack: Off");
                }

                if (!passive)
                {
                    var heroPosition = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(heroPosition.X - 150, heroPosition.Y - 30,
                        Color.Red, "Passive Stack: Off");
                }

                if (!laneclear)
                {
                    var heroPosition = Drawing.WorldToScreen(Player.Position);
                    Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30,
                        Color.Red, "Lane Clear: Off");
                }
            }


        }

        private static void Combo()
        {

            Ignite = Player.GetSpellSlot("summonerdot");
            var qSpell = Config.Item("useQ").GetValue<bool>();
            var eSpell = Config.Item("useE").GetValue<bool>();
            var wSpell = Config.Item("useW").GetValue<bool>();
            var rSpell = Config.Item("useR").GetValue<bool>();
            var rwwSpell = Config.Item("useRww").GetValue<bool>();
            Obj_AI_Hero target = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(Q.Range))
            {
                return;
            }


            if (target.IsValidTarget(W.Range)
                && (target.Health < IgniteDamage(target) + W.GetDamage(target)))
            {
                Player.Spellbook.CastSpell(Ignite, target);
            }

            switch (Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                case 1:
                    if (R.IsReady())
                    {
                        if (GetPassiveBuff == 1
                            || !Player.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Q.Range)
                                && qSpell
                                && Q.IsReady())
                            {
                                Q.Cast(target);
                            }

                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(E.Range)
                                && eSpell
                                && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }

                            if (rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                        if (GetPassiveBuff == 2)
                        {
                            if (target.IsValidTarget(Q.Range)
                                && qSpell
                                && Q.IsReady())
                            {
                                Q.Cast(target);
                            }

                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(E.Range)
                                && eSpell
                                && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }

                            if (rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                        if (GetPassiveBuff == 3)
                        {
                            if (Q.IsReady()
                                && target.IsValidTarget(Q.Range))
                            {
                                {
                                    Qn.Cast(target);
                                }
                            }
                            if (E.IsReady()
                                && target.IsValidTarget(E.Range))
                            {
                                {
                                    E.CastOnUnit(target);
                                }
                            }
                            if (W.IsReady()
                                && target.IsValidTarget(W.Range))
                            {
                                {
                                    W.CastOnUnit(target);
                                }
                            }
                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW")
                                        && (Q.IsReady() || W.IsReady() || E.IsReady()))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell
                                        && (Q.IsReady() || W.IsReady() || E.IsReady()))
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                        if (GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }
                            if (target.IsValidTarget(Qn.Range)
                                && Q.IsReady()
                                && qSpell)
                            {
                                Qn.Cast(target);
                            }
                            if (target.IsValidTarget(E.Range)
                                && E.IsReady()
                                && eSpell)
                            {
                                E.CastOnUnit(target);
                            }

                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                    if (!Q.IsReady() && !W.IsReady() && !E.IsReady())
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                        if (Player.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }

                            if (wSpell
                                && W.IsReady()
                                && target.IsValidTarget(W.Range))
                            {
                                W.CastOnUnit(target);
                            }

                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }

                            if (eSpell
                                && E.IsReady()
                                && target.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(target);
                            }

                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }

                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                    if (!E.IsReady() && !Q.IsReady() && !W.IsReady())
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }
                    }

                    if (!R.IsReady())
                    {
                        if (GetPassiveBuff == 1
                            || !Player.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(E.Range)
                                && eSpell
                                && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }
                        }

                        if (GetPassiveBuff == 2)
                        {
                            if (target.IsValidTarget(Q.Range)
                                && qSpell
                                && Q.IsReady())
                            {
                                Q.Cast(target);
                            }

                            if (target.IsValidTarget(E.Range)
                                && eSpell
                                && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }

                            if (rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                        if (GetPassiveBuff == 3)
                        {
                            if (Q.IsReady()
                                && target.IsValidTarget(Q.Range))
                            {
                                {
                                    Qn.Cast(target);
                                }
                            }
                            if (E.IsReady()
                                && target.IsValidTarget(E.Range))
                            {
                                {
                                    E.CastOnUnit(target);
                                }
                            }
                            if (W.IsReady()
                                && target.IsValidTarget(W.Range))
                            {
                                {
                                    W.CastOnUnit(target);
                                }
                            }
                        }

                        if (GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(E.Range)
                                && E.IsReady()
                                && eSpell)
                            {
                                E.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }
                            if (target.IsValidTarget(Qn.Range)
                                && Q.IsReady()
                                && qSpell)
                            {
                                Qn.Cast(target);
                            }
                        }

                        if (Player.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }

                            if (wSpell
                                && W.IsReady()
                                && target.IsValidTarget(W.Range))
                            {
                                W.CastOnUnit(target);
                            }

                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }

                            if (eSpell
                                && E.IsReady()
                                && target.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(target);
                            }

                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }
                        }
                    }
                    break;
           


            case 0:

                    if (target.IsValidTarget(Q.Range))
                    {
                        if (GetPassiveBuff <= 2
                            || !Player.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Q.Range)
                                && qSpell
                                && Q.IsReady())
                            {
                                Q.Cast(target);
                            }

                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }

                            if (target.IsValidTarget(E.Range)
                                && eSpell
                                && E.IsReady())
                            {
                                E.CastOnUnit(target);
                            }

                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }


                        if (GetPassiveBuff == 3)
                        {
                            if (Q.IsReady()
                                && target.IsValidTarget(Q.Range))
                            {
                                {
                                    Qn.Cast(target);
                                }
                            }
                            if (E.IsReady()
                                && target.IsValidTarget(E.Range))
                            {
                                {
                                    E.CastOnUnit(target);
                                }
                            }
                            if (W.IsReady()
                                && target.IsValidTarget(W.Range))
                            {
                                {
                                    W.CastOnUnit(target);
                                }
                            }
                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                }
                            }

                        }

                        if (GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(W.Range)
                                && wSpell
                                && W.IsReady())
                            {
                                W.CastOnUnit(target);
                            }
                            if (target.IsValidTarget(Qn.Range)
                                && Q.IsReady()
                                && qSpell)
                            {
                                Qn.Cast(target);
                            }
                            if (target.IsValidTarget(E.Range)
                                && E.IsReady()
                                && eSpell)
                            {
                                E.CastOnUnit(target);
                            }

                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                        if (Player.HasBuff("ryzepassivecharged"))
                        {
                            if (wSpell
                                && W.IsReady()
                                && target.IsValidTarget(W.Range))
                            {
                                W.CastOnUnit(target);
                            }

                            if (qSpell
                                && Qn.IsReady()
                                && target.IsValidTarget(Qn.Range))
                            {
                                Qn.Cast(target);
                            }

                            if (eSpell
                                && E.IsReady()
                                && target.IsValidTarget(E.Range))
                            {
                                E.CastOnUnit(target);
                            }

                            if (R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(W.Range)
                                    && target.Health > (Q.GetDamage(target) + E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                    {
                                        R.Cast();
                                    }
                                    if (!rwwSpell)
                                    {
                                        R.Cast();
                                    }
                                    if (!E.IsReady() && !Q.IsReady() && !W.IsReady())
                                    {
                                        R.Cast();
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (wSpell
                            && W.IsReady()
                            && target.IsValidTarget(W.Range))
                        {
                            W.CastOnUnit(target);
                        }

                        if (qSpell
                            && Qn.IsReady()
                            && target.IsValidTarget(Qn.Range))
                        {
                            Qn.Cast(target);
                        }

                        if (eSpell
                            && E.IsReady()
                            && target.IsValidTarget(E.Range))
                        {
                            E.CastOnUnit(target);
                        }

                    }
                    break;
            }

if (R.IsReady()
&& GetPassiveBuff == 4
&& rSpell)
{
if (!Q.IsReady()
&& !W.IsReady()
&& !E.IsReady())
{
R.Cast();
}
}
        }

        private static void LaneClear()
        {

            if (GetPassiveBuff == 4
                && !Player.HasBuff("RyzeR")
                && Config.Item("passiveproc").GetValue<bool>())
                return;

            var qlchSpell = Config.Item("useQlc").GetValue<bool>();
            var elchSpell = Config.Item("useElc").GetValue<bool>();
            var wlchSpell = Config.Item("useWlc").GetValue<bool>();
            var q2LSpell = Config.Item("useQ2L").GetValue<bool>();
            var e2LSpell = Config.Item("useE2L").GetValue<bool>();
            var w2LSpell = Config.Item("useW2L").GetValue<bool>();
            var rSpell = Config.Item("useRl").GetValue<bool>();
            var rSlider = Config.Item("rMin").GetValue<Slider>().Value;
            var minMana = Config.Item("useEPL").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (Player.ManaPercent <= minMana)
                return;

            foreach (var minion in minionCount)
            {
                if (qlchSpell
                    && Q.IsReady()
                    && minion.IsValidTarget(Q.Range)
                    && minion.Health <= Q.GetDamage(minion))
                {
                    Q.Cast(minion);
                }

                if (wlchSpell
                    && W.IsReady()
                    && minion.IsValidTarget(W.Range)
                    && minion.Health <= W.GetDamage(minion))
                {
                    W.CastOnUnit(minion);
                }

                if (elchSpell
                    && E.IsReady()
                    && minion.IsValidTarget(E.Range)
                    && minion.Health <= E.GetDamage(minion))
                {
                    E.CastOnUnit(minion);
                }

                if (q2LSpell
                    && Q.IsReady()
                    && minion.IsValidTarget(Q.Range)
                    && minion.Health >= (Player.GetAutoAttackDamage(minion)*1.3))
                {
                    Q.Cast(minion);
                }

                if (e2LSpell
                    && E.IsReady()
                    && minion.IsValidTarget(E.Range)
                    && minion.Health >= (Player.GetAutoAttackDamage(minion) * 1.3))
                {
                    E.CastOnUnit(minion);
                }
                if (w2LSpell
                    && W.IsReady()
                    && minion.IsValidTarget(W.Range)
                    && minion.Health >= (Player.GetAutoAttackDamage(minion) * 1.3))
                {
                    W.CastOnUnit(minion);
                }
                if (rSpell
                    && R.IsReady()
                    && minion.IsValidTarget(Q.Range)
                    && minionCount.Count > rSlider)
                {
                    R.Cast();
                }
            }
        }


        private static void JungleClear()
        {
            var qSpell = Config.Item("useQj").GetValue<bool>();
            var eSpell = Config.Item("useEj").GetValue<bool>();
            var wSpell = Config.Item("useWj").GetValue<bool>();
            var mSlider = Config.Item("useJM").GetValue<Slider>().Value;



            if (Player.ManaPercent < mSlider)
            {
                return;
            }


            var jungle = MinionManager.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth);

            if (jungle.Count > 0)
                foreach (var jungleMinion in jungle)
            {
                if (!jungleMinion.IsValidTarget())
                {
                    if (Config.Item("disablelane").GetValue<KeyBind>().Active)
                    {
                        LaneClear();
                    }
                    return;
                }
                if (GetPassiveBuff == 4
                    || Player.HasBuff("RyzePassiveStack"))
                {
                    if (eSpell
                        && E.IsReady())
                    {
                        E.CastOnUnit(jungleMinion);
                    }
                    if (qSpell
                        && Q.IsReady())
                    {
                        Q.Cast(jungleMinion);
                    }

                    if (wSpell
                        && W.IsReady())
                    {
                        W.CastOnUnit(jungleMinion);
                    }
                }
            }
        }

        private static void LastHit()
        {
            var qlchSpell = Config.Item("useQl2h").GetValue<bool>();
            var elchSpell = Config.Item("useEl2h").GetValue<bool>();
            var wlchSpell = Config.Item("useWl2h").GetValue<bool>();

            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            foreach (var minion in minionCount)
            {
                if (qlchSpell
                    && Q.IsReady()
                    && minion.IsValidTarget(Q.Range - 20)
                    && minion.Health < Q.GetDamage(minion))
                {
                    Q.Cast(minion);
                }

                if (wlchSpell
                    && W.IsReady()
                    && minion.IsValidTarget(W.Range - 10)
                    && minion.Health < W.GetDamage(minion))
                {
                    W.CastOnUnit(minion);
                }

                if (elchSpell
                    && E.IsReady()
                    && minion.IsValidTarget(E.Range - 10)
                    && minion.Health < E.GetDamage(minion))
                {
                    E.CastOnUnit(minion);
                }
            }

        }

        private static void Mixed()
        {
            /*
            foreach (var JOE_HAS_NO_PENIS in Player.Buffs)
            {
                Console.WriteLine(JOE_HAS_NO_PENIS.Name.ToString(), 1337);
            }
             */

            var qSpell = Config.Item("UseQM").GetValue<bool>();
            var qlSpell = Config.Item("UseQMl").GetValue<bool>();
            var eSpell = Config.Item("UseEM").GetValue<bool>();
            var wSpell = Config.Item("UseWM").GetValue<bool>();
            var minMana = Config.Item("useEPL").GetValue<Slider>().Value;

            if (Player.ManaPercent < Config.Item("mMin").GetValue<Slider>().Value)
                return;
            
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
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if (Player.ManaPercent <= minMana)
                    return;
                foreach (var minion in minionCount)
                {
                    if (qlSpell
                        && Q.IsReady()
                        && minion.Health < Q.GetDamage(minion))
                    {
                        Q.Cast(minion);
                    }
                }
            }
        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget() || target.IsInvulnerable)
                return;

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

        private static int GetPassiveBuff
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                return data != null ? data.Count : 0;
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

        private static void TearStack()
        {
            var minions = MinionManager.GetMinions(
   ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy,
   MinionOrderTypes.MaxHealth);


            if (Config.Item("tearoptions").GetValue<bool>()
                && !Player.InFountain())
            {
                return;
            }

            if (Player.IsRecalling()
                || minions.Count >= 1)
                return;

            var mtears = Config.Item("tearSM").GetValue<Slider>().Value;
            if (GetPassiveBuff == 4
                && (!Items.HasItem(3070)))
            {
                return;
            }
             

            if (Q.IsReady()
                && Player.ManaPercent >= mtears
                && ((Items.HasItem(ItemData.Tear_of_the_Goddess.Id)
                     || Items.HasItem(ItemData.Archangels_Staff.Id))))
            {
                Q.Cast(Game.CursorPos);
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

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (Q.IsReady() || Player.Mana <= Q.Instance.ManaCost * 5)
            {
                return Q.GetDamage(enemy) * 5;
            }

            if (E.IsReady() || Player.Mana <= E.Instance.ManaCost * 5)
            {
                return E.GetDamage(enemy) * 5;
            }

            if (W.IsReady() || Player.Mana <= W.Instance.ManaCost * 3)
            {
                return W.GetDamage(enemy) * 3;
            }
            return 0;
        }

        private static void Item()
        {

            var staff = Config.Item("staff").GetValue<bool>();
            var staffhp = Config.Item("staffhp").GetValue<Slider>().Value;
            if (staff
                && Items.HasItem(ItemData.Seraphs_Embrace.Id)
                && Player.HealthPercent <= staffhp)
            {
                Items.UseItem(ItemData.Seraphs_Embrace.Id);
            }
        }

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var mura = Config.Item("muramana").GetValue<bool>();
            if (mura)
            {
                var muramanai = Items.HasItem(Muramana) ? 3042 : 3043;
                if (args.Target.IsValid<Obj_AI_Hero>()
                    && args.Target.IsEnemy
                    && Items.HasItem(muramanai)
                    && Items.CanUseItem(muramanai))
                {
                    if (!ObjectManager.Player.HasBuff("Muramana"))
                        Items.UseItem(muramanai);
                }
            }
        }

        private static void AutoPassive()
        {

            var minions = MinionManager.GetMinions(
ObjectManager.Player.ServerPosition, Q.Range, MinionTypes.All, MinionTeam.Enemy,
MinionOrderTypes.MaxHealth);
            if (Player.Mana < Config.Item("ManapSlider").GetValue<Slider>().Value)
                return;

            if (Player.IsRecalling()
                || minions.Count >= 1)
                return;

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                return;
            }

            var stackSliders = Config.Item("stackSlider").GetValue<Slider>().Value;
            if (Player.IsRecalling() || Player.InFountain())
            {
                return;
            }

            if (GetPassiveBuff >= stackSliders)
            {
                return;
            }
            if (Environment.TickCount - Q.LastCastAttemptT >= 9000
                && Q.IsReady())
            {
                Q.Cast(Game.CursorPos);
            }

        }
    }
}
