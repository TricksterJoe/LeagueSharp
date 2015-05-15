using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Dynamic;
using System.Linq;
using System.Runtime;
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

namespace Slutty_Darius
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
            if (Player.ChampionName != "Darius")
                return;
            Q = new Spell(SpellSlot.Q, 425);
            W = new Spell(SpellSlot.W, 145);
            E = new Spell(SpellSlot.E, 550);
            R = new Spell(SpellSlot.R, 460);

            Q.SetSkillshot(0.30f, 80, int.MaxValue, false, SkillshotType.SkillshotCone);

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
            spellMenu.AddItem(new MenuItem("useR2LC", "Only R When Killable").SetValue(true));
            drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));
            clearMenu.AddItem(new MenuItem("useQ2L", "Use Q").SetValue(true));
            clearMenu.AddItem(new MenuItem("useW2L", "Use W").SetValue(true));
            clearMenu.AddItem(new MenuItem("useH2L", "Use Hydra/Tiamat").SetValue(true));
            clearMenu.AddItem(new MenuItem("minMana", "Minimum Mana for lane clear").SetValue(new Slider(50, 1)));
            ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            ksMenu.AddItem(new MenuItem("useR2KS", "Use R for ks").SetValue(true));

            Menu.AddToMainMenu();
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
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
            if (Menu.Item("rDraw").GetValue<bool>() && R.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, R.Range, Color.Black);
            }
        }

        private static void Combo()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero targetW = TargetSelector.GetTarget(W.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero targetE = TargetSelector.GetTarget(E.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("useQ").GetValue<bool>() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            if (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id) || Items.HasItem(ItemData.Tiamat_Melee_Only.Id))
            {
                Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
            }
            if (Menu.Item("useW").GetValue<bool>() && targetW.IsValidTarget(W.Range))
            {
                W.Cast();
            }
            if (Menu.Item("useE").GetValue<bool>() && targetE.IsValidTarget(E.Range))
            {
                E.Cast(target);
            }
            if (Menu.Item("useR2LC").GetValue<bool>() && Menu.Item("useR").GetValue<bool>() &&
                targetR.IsValidTarget(R.Range) && targetR.Health < R.GetDamage(targetR))
            {
                R.CastOnUnit(targetR);
            }
            if (Menu.Item("useR").GetValue<bool>() && targetR.IsValidTarget(R.Range) && !Menu.Item("useR2LC").GetValue<bool>())
            {
                R.CastOnUnit(targetR);
            }
        }

        private static void LaneClear()
        {
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                foreach (var minion in minionCount)
                {
                    if ((minionCount.Count > 3) && (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id) || Items.HasItem(ItemData.Tiamat_Melee_Only.Id)) && (Menu.Item("useH2L").GetValue<bool>()))
                    {
                        Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                        Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
                    }
                    if (Player.ManaPercent < (Menu.Item("minMana").GetValue<Slider>().Value))
                        return;
                    if (minionCount.Count > 2 && (Menu.Item("useQ2L").GetValue<bool>()))
                    {
                        Q.Cast();
                    }
                    if (
                        HealthPrediction.GetHealthPrediction(
                            minion, (int) (Q.Delay + (minion.Distance(Player.Position)/Q.Speed))) <
                        Player.GetSpellDamage(minion, SpellSlot.W) && Menu.Item("useW2L").GetValue<bool>())
                    {
                        W.CastOnUnit(minion);
                    }
                }
            }
        }

        private static void Mixed()
        {
            
        }

        private static void KillSteal()
        {
            Obj_AI_Hero targetR = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("useQ2KS").GetValue<bool>() && target.Health < Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.CastOnUnit(target);
            }
            if (Menu.Item("useR2KS").GetValue<bool>() && target.Health < Player.GetSpellDamage(targetR, SpellSlot.R))
            {
                R.Cast(targetR);
            }
        }
    }
}

