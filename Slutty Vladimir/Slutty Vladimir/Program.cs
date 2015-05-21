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

namespace Slutty_Pantheon
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
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
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
            poolMenu.AddItem(new MenuItem("useWHP", "Hp for W").SetValue(new Slider(50, 1)));
            poolMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));



            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Notifications.AddNotification("Hoe's Ryze assembly :)", 5000);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;
            AutoPool();
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
            var qSpell = Menu.Item("useQ").GetValue<bool>();
            var eSpell = Menu.Item("useE").GetValue<bool>();
            var rSpell = Menu.Item("useR").GetValue<bool>();
            var EPSpell = Menu.Item("useEP").GetValue<Slider>().Value;
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (qSpell && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (eSpell && target.IsValidTarget(E.Range) && (Player.HealthPercent > EPSpell))
            {
                E.Cast();
            }
            if (rSpell && (Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.E)) < target.Health)
            {
                R.CastOnUnit(target);
            }
        }

        private static void LaneClear()
        {
            var qSpell = Menu.Item("useQlc").GetValue<bool>();
            var eSpell = Menu.Item("useE2L").GetValue<bool>();
            var qlSpell = Menu.Item("useQ2L").GetValue<bool>();
            var elcslider = Menu.Item("useESlider").GetValue<Slider>().Value;
            var elcpslider = Menu.Item("useEPL").GetValue<Slider>().Value;
           var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            foreach (var minion in minionCount)
            {
                if (eSpell && E.IsReady() && minionCount.Count >= elcslider && Player.HealthPercent > elcpslider)
                {
                    E.Cast();
                }
                if (qSpell && Q.GetDamage(minion) > minion.Health && Q.IsReady())
                {
                    Q.CastOnUnit(minion);
                }
                if (qlSpell && Q.IsReady())
                {
                    Q.CastOnUnit(minion);
                }

            }
        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = Menu.Item("useQ2KS").GetValue<bool>();
            var eSpell = Menu.Item("useE2KS").GetValue<bool>();
            if (qSpell && Q.GetDamage(target) > target.Health && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (eSpell && E.GetDamage(target) > target.Health && target.IsValidTarget(E.Range))
            {
                E.Cast();
            }
            
        }

        private static void Mixed()
        {

        }

        private static void AutoPool()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (Menu.Item("useW").GetValue<bool>() &&
       Player.HealthPercent < (Menu.Item("useWHP").GetValue<Slider>().Value) && target.IsValidTarget(Q.Range))
            {
                W.Cast();
            }   
        }
        }
    }
