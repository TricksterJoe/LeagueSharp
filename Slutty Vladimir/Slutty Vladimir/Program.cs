using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common.Data;
using SharpDX.Win32;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using System.Drawing;

namespace Slutty_Vladimir
{
    internal class Program
    {
        private static Obj_AI_Hero Player
        {
            get { return ObjectManager.Player; }
        }

        private static Orbwalking.Orbwalker Orbwalker;
        private static Spell Q, W, E, R;
        private static Orbwalking.Orbwalker _orbwalker;
        private static Menu Menu;




        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.ChampionName != "Vladimir")
                return;
            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 610);
            R = new Spell(SpellSlot.R, 700);

            R.SetSkillshot(0.25f, 175, 700, false, SkillshotType.SkillshotCircle);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);   
            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            Menu clearMenu = Menu.AddSubMenu(new Menu("LaneClear", "Lane Clear"));
            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "disableDraw"));
            Menu ksMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            Menu poolMenu = Menu.AddSubMenu(new Menu("Auto Pool", "aPool"));
            Menu autoMenu = Menu.AddSubMenu(new Menu("autoEStack", "Auto E Stack"));

            spellMenu.AddItem(new MenuItem("useQ", "Use Q1").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useEP", "Use E If HP% is above").SetValue(new Slider(50, 1)));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            spellMenu.AddItem(new MenuItem("useRc", "Only R when targets").SetValue(new Slider(3, 1, 5)));
            clearMenu.AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useE2L", "Use E to lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useESlider", "Min minions for E").SetValue(new Slider(3, 1, 20)));
            clearMenu.AddItem(new MenuItem("useEPL", "Use E If HP% is above").SetValue(new Slider(50, 1)));
            drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "R Drawing").SetValue(true));
            clearMenu.AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            ksMenu.AddItem(new MenuItem("useE2KS", "Use E for ks").SetValue(true));
            poolMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            poolMenu.AddItem(new MenuItem("useWHP", "Hp for W").SetValue(new Slider(50, 1)));
            poolMenu.AddItem(new MenuItem("useWGapCloser", "Auto W when Gap Closer")).GetValue<bool>();
            autoMenu.AddItem(new MenuItem("AutoEStack", "Automatic stack E", true).SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
            autoMenu.AddItem(new MenuItem("MinHPEStack", "Minimum automatic stack HP")).SetValue(new Slider(20));


            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                Combo();
                AutoPool();
                KillSteal();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Mixed();
                AutoPool();
                KillSteal();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
                AutoPool();
                KillSteal();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                AutoPool();
                KillSteal();
            }
            var autoStack = Menu.Item("AutoEStack", true).GetValue<KeyBind>().Active;
            if (autoStack)
            {
                AutoE();
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (Menu.Item("qDraw").GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);
            }
            if (Menu.Item("eDraw").GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);
            }
        }
        private static void Combo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("useQ").GetValue<bool>()
                && Q.IsReady()
                && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (Menu.Item("useE").GetValue<bool>()
                && E.IsReady()
                && target.IsValidTarget(E.Range)
                && Player.HealthPercent > Menu.Item("useEP").GetValue<Slider>().Value)
            {
                E.Cast();
            }
            if (Menu.Item("useR").GetValue<bool>()          
                && R.IsReady()
                && target.IsValidTarget(R.Range)
                && (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E)) <
                target.Health)
            {
                R.Cast(target);
            }

        }

        private static void LaneClear()
        {
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {
                    if (Menu.Item("useE2L").GetValue<bool>() 
                        && E.IsReady()
                        && minionCount.Count >= Menu.Item("useESlider").GetValue<Slider>().Value
                        && Player.HealthPercent > Menu.Item("useEPL").GetValue<Slider>().Value)
                    {
                        E.Cast();
                    }
                    if (Menu.Item("useQlc").GetValue<bool>() 
                        && (Q.GetDamage(minion) > minion.Health) 
                        && Q.IsReady())
                    {
                        Q.CastOnUnit(minion);
                    }
                    if (Menu.Item("useQ2c").GetValue<bool>()
                        && Q.IsReady()
                        && Menu.Item("useQlc").GetValue<bool>())
                    {
                        Q.CastOnUnit(minion);
                    }

                }
            }
        }

        private static void Mixed()
        {
            
        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Menu.Item("useQ2KS").GetValue<bool>();
            var eSpell = Menu.Item("useE2KS").GetValue<bool>();
            if (qSpell
                && Q.GetDamage(target) > target.Health 
                && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (eSpell 
                && E.GetDamage(target) > target.Health 
                && target.IsValidTarget(E.Range))
            {
                E.Cast();
            }  
        }

        private static void AutoPool()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("useW").GetValue<bool>()
                && Player.HealthPercent < (Menu.Item("useWHP").GetValue<Slider>().Value)
                && target.IsValidTarget(Q.Range))
            {
                W.Cast();
            }      
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Menu.Item("useWGapCloser").GetValue<bool>()
                && W.IsReady() 
                && gapcloser.Sender.Distance(Player) < W.Range 
                && Player.CountEnemiesInRange(E.Range) >= 1)
            {
                W.Cast(Player);
            }
        }

        private static void AutoE()
        {
            if (Player.IsRecalling() || Player.InFountain())
                return;
            var stackHp = Menu.Item("MinHPEStack").GetValue<Slider>().Value;

            if (Environment.TickCount - E.LastCastAttemptT >= 9900
                && E.IsReady()
                && (Player.Health/Player.MaxHealth)*100 >= stackHp)
            {
                E.Cast();
            }
                
        }

    }
}