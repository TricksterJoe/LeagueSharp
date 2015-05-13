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

namespace Slutty_Irelia
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
            if (Player.ChampionName != "Irelia")
                return;
            Q = new Spell(SpellSlot.Q, 650f);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 325);
            R = new Spell(SpellSlot.R, 1000f);

            Q.SetSkillshot(0.25f, 75f, 1500f, false, SkillshotType.SkillshotLine);
            E.SetSkillshot(0.15f, 75f, 1500f, false, SkillshotType.SkillshotCircle);
            R.SetSkillshot(0.15f, 80f, 1500f, false, SkillshotType.SkillshotLine);

            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            Menu clearMenu = Menu.AddSubMenu(new Menu("LaneClear", "Lane Clear"));
            Menu drawMenu = Menu.AddSubMenu(new Menu("Drawings", "disableDraw"));
            Menu ksMenu = Menu.AddSubMenu(new Menu("KillSteal", "KillSteal"));
            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true));
            clearMenu.AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useRlc", "Use R in lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("minMana", "Minimum Mana for lane clear").SetValue(new Slider(50, 1)));
            drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            clearMenu.AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useW2L", "Use W to lane clear").SetValue(true));
            ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            ksMenu.AddItem(new MenuItem("useR2KS", "Use R for ks").SetValue(true));


            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Notifications.AddNotification("Hoe's Ryze assembly :)", 5000);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

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
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            // dont draw stuff while dead
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
            if (!Menu.Item("useQ").GetValue<bool>())
                return;
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero targetE = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (!Menu.Item("useR").GetValue<bool>())
                return;
            if (R.IsReady() && target.IsValidTarget(R.Range))
            {
                R.Cast(targetR);
            }
            if (E.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.CastOnUnit(target);
            }
            if (!Menu.Item("useW").GetValue<bool>())
                return;
            if (W.IsReady() && target.IsValidTarget())
            {
                W.Cast();
            }
            if (!Menu.Item("useE").GetValue<bool>())
                return;
            if (E.IsReady() && target.IsValidTarget(E.Range))
            {
                E.CastOnUnit(targetE);
            }

        }

        private static void Mixed()
        {

        }

        private static void LaneClear()
        {
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if (Player.ManaPercent < (Menu.Item("minMana").GetValue<Slider>().Value))
                    return;
                foreach (var minion in minionCount)
                {
                    if (
                        HealthPrediction.GetHealthPrediction(
                            minion, (int) (Q.Delay + (minion.Distance(Player.Position)/Q.Speed))) <
                        Player.GetSpellDamage(minion, SpellSlot.Q) && Menu.Item("useQlc").GetValue<bool>())
                    {
                        Q.CastOnUnit(minion);
                    }
                    if ( minionCount.Count > 4 && Menu.Item("useRlc").GetValue<bool>() )
                    {
                        R.Cast(minion);
                    }
                }
                if (Menu.Item("useW2L").GetValue<bool>() && minionCount.Count > 1)
                {
                    W.Cast();
                } 
            }
        }

        private static void KillSteal()
        {
            Obj_AI_Hero targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            
            if (Menu.Item("useQ2KS").GetValue<bool>() && target.Health < Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.CastOnUnit(target);
            }
            if (Menu.Item("useR2KS").GetValue<bool>() &&target.Health < Player.GetSpellDamage(targetR, SpellSlot.R))
            {
                R.Cast(targetR);
            }

        }
    }
} 
