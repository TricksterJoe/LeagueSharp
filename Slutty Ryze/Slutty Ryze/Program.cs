using System;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using Slutty_ryze.Properties;

namespace Slutty_ryze
{
    internal class Program
    {
        readonly static Random Seeder = new Random();
        private static bool _casted;
        private static int _lastw;
        #region onload
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            // So you can test if it in VS wihout crahses
#if !DEBUG
            CustomEvents.Game.OnGameLoad += OnLoad;
#endif
        }

        private static void OnLoad(EventArgs args)
        {
            if (GlobalManager.GetHero.ChampionName != Champion.ChampName)
                return;

            Console.WriteLine(@"Loading Your Slutty Ryze");

            Humanizer.AddAction("generalDelay",35.0f);

            Champion.Q = new Spell(SpellSlot.Q, 865);
            Champion.Qn = new Spell(SpellSlot.Q, 865);
            Champion.W = new Spell(SpellSlot.W, 585);
            Champion.E = new Spell(SpellSlot.E, 585);
            Champion.R = new Spell(SpellSlot.R);

            Champion.Q.SetSkillshot(0.26f, 50f, 1700f, true, SkillshotType.SkillshotLine);
            Champion.Qn.SetSkillshot(0.26f, 50f, 1700f, false, SkillshotType.SkillshotLine);

            //assign menu from MenuManager to Config
            Console.WriteLine(@"Loading Your Slutty Menu...");
            GlobalManager.Config = MenuManager.GetMenu();
            GlobalManager.Config.AddToMainMenu();
            Printmsg("Ryze Assembly Loaded");
            Printmsg1("Current Version: " + typeof(Program).Assembly.GetName().Version);
            Printmsg2("Don't Forget To " + "<font color='#00ff00'>[Upvote]</font> <font color='#FFFFFF'>" + "The Assembly In The Databse" + "</font>");
            //Other damge inficators in MenuManager ????
            GlobalManager.DamageToUnit = Champion.GetComboDamage;

            Drawing.OnDraw += DrawManager.Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
#pragma warning disable 618
            Interrupter.OnPossibleToInterrupt += Champion.RyzeInterruptableSpell;
            Spellbook.OnCastSpell += Champion.OnProcess;
#pragma warning restore 618
            Orbwalking.BeforeAttack += Champion.Orbwalking_BeforeAttack;
            //CustomEvents.Unit.OnDash += Champion;
            ShowDisplayMessage();

        }

        #endregion
        private static void Printmsg(string message)
        {
            Game.PrintChat(
                "<font color='#6f00ff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg1(string message)
        {
            Game.PrintChat(
                "<font color='#ff00ff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }

        private static void Printmsg2(string message)
        {
            Game.PrintChat(
                "<font color='#00abff'>[Slutty Ryze]:</font> <font color='#FFFFFF'>" + message + "</font>");
        }
        #region onGameUpdate

        private static void ShowDisplayMessage()
        {
            var r = new Random();
      
            var txt = Resources.display.Split('\n');
            switch (r.Next(1, 3))
            {
                case 2:
                    txt = Resources.display2.Split('\n');
                    break;
                case 3:
                    txt = Resources.display3.Split('\n');
                    break;
            }

            foreach (var s in txt)
                Console.WriteLine(s);
            #region L# does not allow D:
            //try
            //{
            //    var sr = new System.IO.StreamReader(System.Net.WebRequest.Create(string.Format("http://www.fiikus.net/asciiart/pokemon/{0}{1}{2}.txt", r.Next(0, 1), r.Next(0, 3), r.Next(0, 9))).GetResponse().GetResponseStream());
            //    string line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        Console.WriteLine(line);
            //    }
            //}

            //catch
            //{
            //    // ignored
            //}
            #endregion
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            try // lazy
            {
                if (GlobalManager.Config.Item("test").GetValue<KeyBind>().Active)
                {
                    GlobalManager.GetHero.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var targets = TargetSelector.GetTarget(Champion.W.Range, TargetSelector.DamageType.Magical);
                    if (targets == null)
                        return;
                    if (Champion.W.IsReady())
                    {
                        LaneOptions.CastW(targets);
                        {
                            _lastw = Environment.TickCount;
                        }
                    }

                    if (Environment.TickCount - _lastw >= 700 - Game.Ping)
                    {
                        if (Champion.Q.IsReady())
                        {
                            LaneOptions.CastQn(targets);
                            _casted = true;
                        }
                    }

                    if (_casted)
                    {
                        LaneOptions.CastE(targets);
                        LaneOptions.CastQn(targets);
                        _casted = false;
                    }
                }

                if (GlobalManager.Config.Item("chase").GetValue<KeyBind>().Active)
                {
                    GlobalManager.GetHero.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    var targets = TargetSelector.GetTarget(Champion.W.Range + 200, TargetSelector.DamageType.Magical);
                    if (targets == null)
                        return;

                    if (GlobalManager.Config.Item("usewchase").GetValue<bool>())
                        LaneOptions.CastW(targets);

                    if (GlobalManager.Config.Item("chaser").GetValue<bool>() &&
                        targets.Distance(GlobalManager.GetHero) > Champion.W.Range + 200)
                        Champion.R.Cast();
                }
           
                if (GlobalManager.GetHero.IsDead)
                    return;
                if (GlobalManager.GetHero.IsRecalling())
                    return;

                if (Champion.casted == false)
                {
                    MenuManager.Orbwalker.SetAttack(true);
                }

                var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);

                if (GlobalManager.Config.Item("doHuman").GetValue<bool>())
                {
                    if (!Humanizer.CheckDelay("generalDelay")) // Wait for delay for all other events
                    {
                        Console.WriteLine(@"Waiting on Human delay");
                        return;
                    }
                    //Console.WriteLine("Seeding Human Delay");
                    var nDelay = Seeder.Next(GlobalManager.Config.Item("minDelay").GetValue<Slider>().Value, GlobalManager.Config.Item("maxDelay").GetValue<Slider>().Value); // set a new random delay :D
                    Humanizer.ChangeDelay("generalDelay", nDelay);
                }

                if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
//
//                    if (target.IsValidTarget() 
//                        &&  GlobalManager.GetHero.Distance(target) > 400 && (Champion.Q.IsReady() && Champion.W.IsReady() && Champion.E.IsReady()))
//                    {
//                        MenuManager.Orbwalker.SetAttack(false);
//                    }
//
//                    if (target.IsValidTarget() && GlobalManager.GetHero.Distance(target) > 400
//                        && (GlobalManager.GetPassiveBuff == 4 || GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
//                        &&
//                        ((!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady()) ||
//                         (Champion.Q.IsReady() && Champion.W.IsReady() && Champion.E.IsReady())))
//                    {
//                        MenuManager.Orbwalker.SetAttack(false);
//                    }

                    Champion.AABlock();
                    LaneOptions.ImprovedCombo();

                    if (target.Distance(GlobalManager.GetHero) >=
                        GlobalManager.Config.Item("minaarange").GetValue<Slider>().Value)
                    {
                        MenuManager.Orbwalker.SetAttack(false);
                    }
                    else
                    {
                        MenuManager.Orbwalker.SetAttack(true);
                    }
                }

                if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
                {
                    LaneOptions.Mixed();
                    MenuManager.Orbwalker.SetAttack(true);
                }

                if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
                {
                    if (!GlobalManager.Config.Item("disablelane").GetValue<KeyBind>().Active)
                        LaneOptions.LaneClear();

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

                if (GlobalManager.Config.Item("UseQauto").GetValue<bool>() && target != null)
                {
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
                    .Any(turret => turret.IsValidTarget(300) && turret.IsAlly && turret.Health > 0))
                    return;

                 Champion.W.CastOnUnit(target);
                // DebugClass.ShowDebugInfo(true);
            }
            catch
            {
                // ignored
            }
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
