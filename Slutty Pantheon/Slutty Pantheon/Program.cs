
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
            if (Player.ChampionName != "Pantheon")
                return;
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 440);

            E.SetSkillshot(0.25f, 15f*2*(float) Math.PI/180, 2000f, false, SkillshotType.SkillshotCone);

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
            clearMenu.AddItem(new MenuItem("useQlc", "Use Q last hit").SetValue(true));
            drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            clearMenu.AddItem(new MenuItem("useQ2L", "Use Q to lane clear").SetValue(true));
            clearMenu.AddItem(new MenuItem("useE2L", "Use E to lane clear").SetValue(true));
            ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q for ks").SetValue(true));
            clearMenu.AddItem(new MenuItem("useH2L", "Use Hydra/Tiamat").SetValue(true));
            clearMenu.AddItem(new MenuItem("minMana", "Minimum Mana for lane clear").SetValue(new Slider(50, 1)));


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
                Orbwalker.SetMovement(true);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Mixed();
                Orbwalker.SetMovement(true);
            }

            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                LaneClear();
                Orbwalker.SetMovement(true);
            }
            if (Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                KillSteal();
                Orbwalker.SetMovement(true);
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
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);
            if (Menu.Item("useQ").GetValue<bool>() && Q.IsReady())
            {
                Q.CastOnUnit(target);
            }
            if (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id) || Items.HasItem(ItemData.Tiamat_Melee_Only.Id))
            {
                Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
            }
            if (Menu.Item("useW").GetValue<bool>() && Q.IsReady())
            {
                W.CastOnUnit(target);
            }
            if (Menu.Item("useE").GetValue<bool>() && E.IsReady())
            {
                E.Cast(target);
                Orbwalker.SetMovement(false);
            }

        }

        private static void Mixed()
        {

        }

        private static void LaneClear()
        {
            if (Player.IsChannelingImportantSpell())
                return;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if (minionCount.Count > 2 && Menu.Item("useH2L").GetValue<bool>() && (Items.HasItem(ItemData.Ravenous_Hydra_Melee_Only.Id) || Items.HasItem(ItemData.Tiamat_Melee_Only.Id)))
                {
                    Items.UseItem(ItemData.Ravenous_Hydra_Melee_Only.Id);
                    Items.UseItem(ItemData.Tiamat_Melee_Only.Id);
                }
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
                    if (Menu.Item("useQ2L").GetValue<bool>())
                    {
                        Q.CastOnUnit(minion);
                    }
                    if (minionCount.Count > 1 && Menu.Item("useE2L").GetValue<bool>())
                    {
                        E.Cast(minion);
                    }
                }

            }

        }

        private static void KillSteal()
        {
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Physical);

            if (Menu.Item("useQ2KS").GetValue<bool>() && target.Health < Player.GetSpellDamage(target, SpellSlot.Q))
            {
                Q.CastOnUnit(target);
            }
        }
    }
}
