using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    internal class Program
    {
        #region onload
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (GlobalManager.GetHero.ChampionName != Champion.ChampName)
                return;

            Champion.Q = new Spell(SpellSlot.Q, 865);
            Champion.Qn = new Spell(SpellSlot.Q, 865);
            Champion.W = new Spell(SpellSlot.W, 585);
            Champion.E = new Spell(SpellSlot.E, 585);
            Champion.R = new Spell(SpellSlot.R);

            Champion.Q.SetSkillshot(0.26f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            Champion.Qn.SetSkillshot(0.26f, 50f, 1700f, false, SkillshotType.SkillshotLine);

            //assign menu from MenuManager to Config
            GlobalManager.Config = MenuManager.GetMenu();
            GlobalManager.Config.AddToMainMenu();

            //Other damge inficators in MenuManager ????
            GlobalManager.DamageToUnit = Champion.GetComboDamage;

            Drawing.OnDraw += DrawManager.Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
#pragma warning disable 618
            Interrupter.OnPossibleToInterrupt += Champion.RyzeInterruptableSpell;
#pragma warning restore 618
            Orbwalking.BeforeAttack += Champion.Orbwalking_BeforeAttack;
            CustomEvents.Unit.OnDash += Champion.Unit_OnDash;
        }
        #endregion

        #region onGameUpdate
        private static void Game_OnUpdate(EventArgs args)
        {
            if (GlobalManager.GetHero.IsDead)
                return;
            MenuManager.Orbwalker.SetAttack(true);

            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);


            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            {
                MenuManager.Orbwalker.SetAttack((target.IsValidTarget() && (GlobalManager.GetHero.Distance(target) > 440) ||
                                     (Champion.Q.IsReady() || Champion.E.IsReady() || Champion.W.IsReady())));
                Champion.AABlock();
                LaneOptions.Combo();
            }

            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                LaneOptions.Mixed();
                MenuManager.Orbwalker.SetAttack(true);
            }

            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (GlobalManager.Config.Item("disablelane").GetValue<KeyBind>().Active)
                    LaneOptions.LaneClear();


                if (GlobalManager.Config.Item("presslane").GetValue<KeyBind>().Active)
                    LaneOptions.LaneClear();


                MenuManager.Orbwalker.SetAttack(true);
                LaneOptions.JungleClear();
            }

            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                LaneOptions.LastHit();


            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                if (GlobalManager.Config.Item("tearS").GetValue<KeyBind>().Active)
                    ItemManager.TearStack();

                if (GlobalManager.Config.Item("autoPassive").GetValue<KeyBind>().Active)
                    Champion.AutoPassive();

                ItemManager.Potion();
                MenuManager.Orbwalker.SetAttack(true);
            }

            if (GlobalManager.Config.Item("UseQauto").GetValue<bool>())
            {
                if (target == null)
                    return;

                if (Champion.Q.IsReady() && target.IsValidTarget(Champion.Q.Range))
                    Champion.Q.Cast(target);
            }


            // Seplane();
            ItemManager.Item();
            Champion.KillSteal();
            ItemManager.Potion();

            if (GlobalManager.Config.Item("level").GetValue<bool>())
            {
                AutoLevelManager.LevelUpSpells();
            }
            if (!GlobalManager.Config.Item("autow").GetValue<bool>() || !target.UnderTurret(true)) return;

            if (target == null)
                return;

            if (!ObjectManager.Get<Obj_AI_Turret>()
                .Any(turret => turret.IsValidTarget(300) && turret.IsAlly && turret.Health > 0)) return;

            Champion.W.CastOnUnit(target);
        }
        #endregion

        /*
        private static void Seplane()
        {
            if (GlobalManager.GetHero.IsValid &&
                GlobalManager.Config.Item("seplane").GetValue<KeyBind>().Active)
            {
                ObjectManager.GlobalManager.GetHero.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                LaneClear();
            }
        }
         */



    }
}
