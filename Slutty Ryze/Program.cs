using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common.Data;
using SharpDX.Win32;
using Color = System.Drawing.Color;
using LeagueSharp.Common;


namespace Slutty_Ryze
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
            if (Player.ChampionName != "Ryze")
                return;
            Q = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R);
            Q.SetSkillshot(0.25f, 60, 1450, true, SkillshotType.SkillshotLine);
            Menu = new Menu(Player.ChampionName, Player.ChampionName, true);

            Menu orbwalkerMenu = Menu.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));

            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);

            Menu ts = Menu.AddSubMenu(new Menu("Target Selector", "Target Selector"));

            TargetSelector.AddToMenu(ts);

            Menu spellMenu = Menu.AddSubMenu(new Menu("Spells", "Spells"));
            Menu clearMenu = Menu.AddSubMenu(new Menu("LaneClear", "Lane Clear"));
            Menu drawMenu = Menu.AddSubMenu(new Menu("disableDraw", "Drawings"));
            Menu itemMenu = Menu.AddSubMenu(new Menu("items", "Items"));

            spellMenu.AddItem(new MenuItem("useQ", "Use Q").SetValue(true));
            spellMenu.AddItem(new MenuItem("useW", "Use W").SetValue(true));
            spellMenu.AddItem(new MenuItem("useE", "Use E").SetValue(true));
            spellMenu.AddItem(new MenuItem("useR", "Use R").SetValue(true)); ;
            clearMenu.AddItem(new MenuItem("useQlc", "Use Q to last hit in laneclear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useWlc", "Use W to last hit in lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useElc", "Use E to last hit in lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useRlc", "Use R in lane clear").SetValue(true));
            drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("wDraw", "W Drawing").SetValue(true));
            itemMenu.AddItem(new MenuItem("sTear", "Stack Tear").SetValue(true));

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
                Overload();
                Runeprison();
                Spellflux();
                DesperatePower();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Runeprison();
                Spellflux();
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                tearStack();
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
            if (Menu.Item("wDraw").GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);
            }
        }
        private static void Overload()
        {
            // check if the player wants to use E
            if (!Menu.Item("useQ").GetValue<bool>())
                return;


            Obj_AI_Hero target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);


            // check if Q ready
            if (Q.IsReady() && target.IsValidTarget(Q.Range))
                Q.Cast(target);
            if (Q.IsReady())
                Q.Cast(target);
        }

        private static void Runeprison()
        {
            // check if the player wants to use E
            if (!Menu.Item("useW").GetValue<bool>())
                return;

            Obj_AI_Hero target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (W.IsReady())
            {
                if (target.IsValidTarget(W.Range))
                {
                    W.CastOnUnit(target);
                }
            }
        }

        private static void Spellflux()
        {
            if (!Menu.Item("useE").GetValue<bool>())
                return;

            Obj_AI_Hero target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);


            // check if E ready
            if (E.IsReady())
            {
                // check if we found a valid target in range
                if (target.IsValidTarget(E.Range))
                {
                    E.CastOnUnit(target);
                }
                if (E.IsReady())
                {
                    E.CastOnUnit(Player);
                }
            }

        }

        private static void DesperatePower()
        {
            var target = TargetSelector.GetTarget(600, TargetSelector.DamageType.Magical);
            if (target.IsValidTarget(600))
            {
                R.Cast();

            }
        }

        private static void LaneClear()
        {
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (minionCount.Count > 0)
            {
                {
                    foreach (var minion in minionCount)
                    {
                        if (
                            HealthPrediction.GetHealthPrediction(
                                minion, (int)(Q.Delay + (minion.Distance(Player.Position) / Q.Speed))) <
                            Player.GetSpellDamage(minion, SpellSlot.Q) && Menu.Item("useQlc").GetValue<bool>())
                        {
                            Q.Cast(minion);
                        }
                        if (HealthPrediction.GetHealthPrediction(minion,
                            (int)(W.Delay + (minion.Distance(Player.Position) / W.Speed)))
                            < Player.GetSpellDamage(minion, SpellSlot.W) && Menu.Item("useWlc").GetValue<bool>())
                        {
                            W.Cast(minion);
                        }
                        if (
                            HealthPrediction.GetHealthPrediction(minion,
                                (int)(E.Delay + (minion.Distance(Player.Position) / E.Speed))) <
                            Player.GetSpellDamage(minion, SpellSlot.E) && Menu.Item("useElc").GetValue<bool>())
                        {
                            E.Cast(minion);
                        }
                        if (Menu.Item("useRlc").GetValue<bool>())
                        {
                            R.Cast();
                        }
                    }
                }

            }
           
        }

        private static void tearStack()
        {
            Obj_AI_Hero player = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (Menu.Item("sTear").GetValue<bool>() && Q.IsReady() && ObjectManager.Player.Mana > ObjectManager.Player.MaxMana * 0.95)
            {
                Q.Cast(Player.Position);
            }
        }
    }
}
