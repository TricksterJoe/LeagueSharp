using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Slutty_Thresh
{
    internal class SluttyThresh
    {
        public const string ChampName = "Thresh";
        public const string Menuname = "Slutty Thresh";
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public static Spell Q, W, E, R;
        private static SpellSlot Ignite;
        private static int lastq2;
        private static SpellSlot FlashSlot;
        public static float FlashRange = 450f;

        public static Dictionary<string, string> channeledSpells = new Dictionary<string, string>();
        private static int lastattempt;
        private static int elastattempt;
        private static int elastattemptin;

        public static void OnLoad(EventArgs args)
        {

            Q = new Spell(SpellSlot.Q, 1040);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 390);
            R = new Spell(SpellSlot.R, 400);

            Q.SetSkillshot(0.5f, 60f, 1900f, true, SkillshotType.SkillshotLine);
            FlashSlot = Player.GetSpellSlot("SummonerFlash");
            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings").AddItem(new MenuItem("Draw", "Display Drawings").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("wDraw", "W Drawing").SetValue(true));
            Config.SubMenu("Drawings").AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));

            var comboMenu = new Menu("Combo Settings (SB)", "combospells");
            {
                comboMenu.AddItem(new MenuItem("useQ", "Use Q (Death Sentence)").SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("useQ2", "Use Second Q Delay (Death Leap)").SetValue(new Slider(0, 500, 1500)));              
                comboMenu.AddItem(new MenuItem("useE", "Use E (Flay)").SetValue(true));
                comboMenu
                    .AddItem(
                        new MenuItem("combooptions", "E Mode").SetValue(new StringList(new[] {"Out", "In"}, 1)));
                comboMenu.AddItem(new MenuItem("useR", "Use R (The Box)").SetValue(true));
            }
            Config.AddSubMenu(comboMenu);

            var lantMenu = new Menu("Lantern Settings", "lantern");
            {
                foreach (var hero in HeroManager.Allies)
                {
                    if (!hero.IsMe)
                    lantMenu.AddItem(new MenuItem("healop" + hero.ChampionName, hero.ChampionName))
                        .SetValue(new StringList(new[] { "Lantern", "No Lantern" }, 1));

                    lantMenu.AddItem(new MenuItem("hpsettings" + hero.ChampionName, "% Hp to").SetValue(new Slider(20)));

                }
            }

            Config.AddSubMenu(lantMenu);

            Config.SubMenu("Fhook").AddItem(new MenuItem("qflash", "Flash Hook").SetValue(new KeyBind('T', KeyBindType.Press)));

            var miscMenu = new Menu("Miscellaneous (Background)", "miscsettings");

            var eventMenu = new Menu("Events", "eventssettings");
            {
                eventMenu.AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));
                eventMenu.AddItem(new MenuItem("useQW2D", "W/Q On Dashing").SetValue(true));
                eventMenu.AddItem(new MenuItem("level", "Auto Skill Level Up").SetValue(true));
                eventMenu.AddItem(new MenuItem("autow", "Auto W enemy under turret").SetValue(true));
            }

            var ksMenu = new Menu("Kill Steal", "kssettings");
            {
                ksMenu.AddItem(new MenuItem("KS", "Kill Steal")).SetValue(true);
                ksMenu.AddItem(new MenuItem("useQ2KS", "Use Q KS").SetValue(true));
                ksMenu.AddItem(new MenuItem("useE2KS", "Use E KS").SetValue(true));
            }

            miscMenu.AddSubMenu(eventMenu);
            miscMenu.AddSubMenu(ksMenu);
            Config.AddSubMenu(miscMenu);

            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
           // Interrupter2.OnInterruptableTarget += ThreshInterruptableSpell;
           CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
            Obj_AI_Hero.OnProcessSpellCast += Game_ProcessSpell;

        }



        private static void Game_OnUpdate(EventArgs args)
        {

         /*   
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target.HasBuff("katarinaereduction")
                && target != null)
            {
                if (target.ChampionName == "Katarina"
                    && target.IsValidTarget(E.Range))
                {
                    E.Cast(target.ServerPosition);
                    eattempt = Environment.TickCount;
                }
                if (Environment.TickCount - eattempt >= 90f
                    && Q.IsReady())
                Q.Cast(target.ServerPosition);
            }
          */
            
            
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                   // LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:                    
                    break;

                 case Orbwalking.OrbwalkingMode.Mixed:
                 //   Mixed();
                    break;

                case Orbwalking.OrbwalkingMode.None:
                    break;

            }

            if (Config.Item("qflash").GetValue<KeyBind>().Active)
            {
                flashq();
            }
             
        }


        private static void Combo()
        {
            Ignite = Player.GetSpellSlot("summonerdot");
            var qSpell = Config.Item("useQ").GetValue<bool>();
            var q2Slider = Config.Item("useQ2").GetValue<Slider>().Value;
            var rSpell = Config.Item("useR").GetValue<bool>();
            var eSpell = Config.Item("useE").GetValue<bool>();
            /*
            var wSpell = Config.Item("useW").GetValue<bool>();
            var rSpell = Config.Item("useR").GetValue<bool>();
            var rwwSpell = Config.Item("useRww").GetValue<bool>();

             */

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (target.HasBuff("threshQ")
                || (Player.Distance(target) <= 650 && E.IsReady()))
            {
                Orbwalker.SetAttack(false);
            }
            else
            {
                Orbwalker.SetAttack(true);
            }


            if (Q.IsReady()
                && (E.IsReady() || ObjectManager.Player.GetSpell(SpellSlot.E).Cooldown <= 3000f)
                && qSpell
                && target.IsValidTarget(Q.Range)
                && target.Distance(Player) >= 400)
            {
                Q.Cast(target);
                lastq = Environment.TickCount;

                if (target.HasBuff("threshQ") /*find buff name in console later!! */
                    && Environment.TickCount - lastq >= q2Slider)
                {
                    Q.Cast(target);
                    lastq2 = Environment.TickCount;
                }
            }

            switch (Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (target.IsValidTarget(E.Range)
                        && eSpell
                        && !target.IsImmovable
                        && Environment.TickCount - lastq >= 700)
                    {
                        E.Cast(target.ServerPosition);
                        elastattempt = Environment.TickCount;
                    }
                    break;
                case 1:
                    if (target.IsValidTarget(E.Range)
                        && Environment.TickCount - lastq >= 700
                                                && eSpell)
                        E.Cast(target.Position.Extend(Player.ServerPosition,
                            Vector3.Distance(target.Position, Player.Position) + 400));
                    elastattemptin = Environment.TickCount;
                    break;
            }
            
            if (target.IsValidTarget(R.Range)
                && rSpell
                && (target.HasBuff("threshq") 
                || (Environment.TickCount - elastattempt > 100f) 
                || (Environment.TickCount - elastattemptin > 100f)))
            {
                R.Cast();
            }
             

            foreach (var friend in
    ObjectManager.Get<Obj_AI_Hero>().Where(x => x.IsAlly).Where(x => !x.IsDead).Where(x => !x.IsZombie))
            {
                var friendHealth = (int)friend.Health / friend.MaxHealth * 100;
                var health = Config.Item("autoRPercent").GetValue<Slider>().Value;

                if (friendHealth <= health)
                {
                    W.Cast(friend.ServerPosition);
                }
            }

            /*  && Q.IsReady()* would be used for second instance */
        }

        private static void flashq()
        {
      Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

      var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
      var x = target.Position.Extend(Prediction.GetPrediction(target, 1).UnitPosition, FlashRange);

            if (Q.IsReady()
                && FlashSlot.IsReady()
                && target.Distance(Player) <= Q.Range+450
                && target.Distance(Player) >= Q.Range-200)
            { 
              
                Player.Spellbook.CastSpell(FlashSlot, x);
                Q.Cast(target);
                if (E.IsReady())
                {
                    E.Cast(Player.Position);
                }

            }
        }
        /* 
        private static void Mixed()
        {
            throw new NotImplementedException();
        }

        private static void LaneClear()
        {
            throw new NotImplementedException();
        }
         */

        private static void OnGapCloser(ActiveGapcloser gapcloser)
        {
            if (gapcloser.Sender.IsAlly
                || gapcloser.Sender.IsMe)
            {
                return;
            }

            if (E.IsInRange(gapcloser.Start))
            {
                E.Cast(Player.Position.Extend(gapcloser.Sender.Position, 400));
            }

        }

        private static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {

            if (!hero.IsMe)
            {
                return;
            }
            channeledSpells["ThreshQinternal"] = "Thresh";
            string name;
            Console.WriteLine(args.SData.Name);
            
            if ((channeledSpells.TryGetValue(args.SData.Name, out name)
                 && hero.Spellbook.IsCastingSpell))
            {
                W.Cast(Player.Position);
            }
             
        }

        
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
            {
                return;
            }

            if (sender.NetworkId == target.NetworkId)
            {
                if (E.IsReady()
                   && E.IsInRange(sender.ServerPosition))
                {
                    E.Cast(Player.Position.Extend(sender.Position, 400));
                }
            }

        }

         
        /*
        private static void ThreshInterruptableSpell(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            throw new NotImplementedException();
        }
         */

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (!Config.Item("Draw").GetValue<bool>())
                return;


            if (Config.Item("qDraw").GetValue<bool>() && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);
            }

            Render.Circle.DrawCircle(Player.Position, 1440, Color.Red);

            if (Config.Item("wDraw").GetValue<bool>() && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);
            }

            if (Config.Item("eDraw").GetValue<bool>() && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);
            }
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (Q.IsReady()
                && FlashSlot.IsReady()
                && target.Distance(Player) <= Q.Range + 450
                && target.Distance(Player) >= Q.Range - 200
                && target != null)
            {
                var heroPosition = Drawing.WorldToScreen(Player.Position);
                var textDimension = Drawing.GetTextExtent("Stunnable!");
                Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                    Color.DarkGreen, "Can Flash Q!");
            }
            else
            {
                var heroPosition = Drawing.WorldToScreen(Player.Position);
                var textDimension = Drawing.GetTextExtent("Stunnable!");
                Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                    Color.Red, "Can't Flash Q!");
            }
        }

        public static int lastq { get; set; }

        public static int eattempt { get; set; }
    }
}
