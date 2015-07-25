using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LCItems = LeagueSharp.Common.Items;
using ItemData = LeagueSharp.Common.Data.ItemData;

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
        private static SpellSlot FlashSlot;
        public static float FlashRange = 450f;

        public static Dictionary<string, string> channeledSpells = new Dictionary<string, string>();
        private static int elastattempt;
        private static int elastattemptin;

        public static void OnLoad(EventArgs args)
        {
            if (Player.ChampionName != ChampName)
                return;

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
            Config.SubMenu("Drawings").AddItem(new MenuItem("qfDraw", "Q->Flash Draw").SetValue(true));

            var comboMenu = new Menu("Combo Settings (SB)", "combospells");
            {
                comboMenu.AddItem(new MenuItem("useQ", "Use Q (Death Sentence)").SetValue(true));
                comboMenu.AddItem(new MenuItem("smartq", "Smart Q").SetValue(true));
                comboMenu.AddItem(new MenuItem("useQ1", "Use Second Q").SetValue(true));
                comboMenu.AddItem(
                    new MenuItem("useQ2", "Use Second Q Delay (Death Leap)").SetValue(new Slider(1000, 0, 1500)));
                comboMenu.AddItem(new MenuItem("useE", "Use E (Flay)").SetValue(true));
                comboMenu
                    .AddItem(
                        new MenuItem("combooptions", "E Mode").SetValue(new StringList(new[] {"Out", "In"}, 1)));
                comboMenu.AddItem(new MenuItem("useR", "Use R (The Box)").SetValue(true));
            }
            Config.AddSubMenu(comboMenu);

            var lantMenu = new Menu("Lantern Settings", "lantern");
            {
                foreach (var hero in 
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => !x.IsEnemy)
                        .Where(x => !x.IsMe)
                        .Where((x => x.IsAlly)))
                {
                    {
                        lantMenu.AddItem(new MenuItem("healop" + hero.ChampionName, hero.ChampionName))
                            .SetValue(new StringList(new[] {"Lantern", "No Lantern"}));

                        lantMenu.AddItem(
                            new MenuItem("hpsettings" + hero.ChampionName, "Lantern When %HP <").SetValue(new Slider(20)));
                    }

                }
                lantMenu.AddItem(new MenuItem("manalant", "%Mana for lantern").SetValue(new Slider(50)));
                lantMenu.AddItem(new MenuItem("autolantern", "Auto Lantern Ally When Q hits").SetValue(false));
            }

            var laneMenu = new Menu("Lane Clear", "laneclear");
            {
                laneMenu.AddItem(new MenuItem("useelch", "Use E").SetValue(true));
                // laneMenu.AddItem(new MenuItem("elchslider", "Minimum Minions For E").SetValue(new Slider(0, 1, 10)));
            }
            Config.AddSubMenu(laneMenu);

            Config.AddSubMenu(lantMenu);
            var flashMenu = new Menu("Flash Hook Settings", "flashf");
            {
                flashMenu.AddItem(new MenuItem("flashmodes", "Flash Modes")
                    .SetValue(new StringList(new[] {"Flash->E->Q", "Flash->Q"})));
                flashMenu.AddItem(new MenuItem("qflash", "Flash Hook").SetValue(new KeyBind('T', KeyBindType.Press)));
            }

            Config.AddSubMenu(flashMenu);

            var miscMenu = new Menu("Miscellaneous (Background)", "miscsettings");

            var eventMenu = new Menu("Events", "eventssettings");
            {
                eventMenu.AddItem(new MenuItem("useW2I", "Interrupt with W").SetValue(true));
                eventMenu.AddItem(new MenuItem("useQW2D", "W/Q On Dashing").SetValue(true));
            }

            var itemMenu = new Menu("Item Usage", "items");
            var shieldMenu = new Menu("Shield Usage", "shield");
            {
                var mountainmenu = new Menu("Face Of The Mountain", "faceof");
                {
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly
                                        && !x.IsMe))
                    {
                        {
                            mountainmenu.AddItem(new MenuItem("faceop" + hero.ChampionName, hero.ChampionName))
                                .SetValue(new StringList(new[] {"Use", "No Use"}));

                            mountainmenu.AddItem(
                                new MenuItem("facehp" + hero.ChampionName, "Use When %HP <").SetValue(new Slider(20)));
                        }
                    }
                }

                var locketmenu = new Menu("Locket Of Solari", "locksol");
                {
                    foreach (var hero in
                        ObjectManager.Get<Obj_AI_Hero>()
                            .Where(x => x.IsAlly
                                        && !x.IsMe))
                    {
                        {
                            locketmenu.AddItem(new MenuItem("locketop" + hero.ChampionName, hero.ChampionName))
                                .SetValue(new StringList(new[] {"Use", "No Use"}));

                            locketmenu.AddItem(
                                new MenuItem("lockethp" + hero.ChampionName, "Use When %HP <").SetValue(new Slider(20)));
                        }
                    }

                }
                shieldMenu.AddSubMenu(locketmenu);
                shieldMenu.AddSubMenu(mountainmenu);
            }

            var healMenu = new Menu("Healing Items", "heals");
            {
                var mikaelss = new Menu("Mikael's Crucibile", "mikaels");
                mikaelss.AddItem(new MenuItem("charm", "Charm", true).SetValue(true));
                mikaelss.AddItem(new MenuItem("snare", "Snare", true).SetValue(true));
                mikaelss.AddItem(new MenuItem("taunt", "Taunt", true).SetValue(true));
                mikaelss.AddItem(new MenuItem("suppression", "Suppression", true).SetValue(true));
                mikaelss.AddItem(new MenuItem("stun", "Stun", true).SetValue(true));
                //  mikaelss.AddItem(new MenuItem("mikaelshp", "Use On %HP", true).SetValue(new Slider(20, 40)));
                var allies = new Menu("Ally Config", "AllysConfig");
                foreach (var hero in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(x => x.IsAlly
                                    && !x.IsMe))
                {
                    {
                        allies.AddItem(new MenuItem("healmikaels" + hero.ChampionName, hero.ChampionName))
                            .SetValue(new StringList(new[] {"Use Mikaels", "Don't Use Mikaels"}));
                    }
                }
                mikaelss.AddSubMenu(allies);
                healMenu.AddSubMenu(mikaelss);
            }
            itemMenu.AddSubMenu(shieldMenu);
            itemMenu.AddSubMenu(healMenu);
            Config.AddSubMenu(itemMenu);

            miscMenu.AddSubMenu(eventMenu);
            Config.AddSubMenu(miscMenu);

            Config.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += ThreshInterruptableSpell;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
            Obj_AI_Hero.OnProcessSpellCast += Game_ProcessSpell;

        }



        private static void Game_OnUpdate(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {

                case Orbwalking.OrbwalkingMode.Combo:
                    Orbwalker.SetAttack(true);
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    Orbwalker.SetAttack(true);
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    Orbwalker.SetAttack(true);
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    Orbwalker.SetAttack(true);
                    //   Mixed();
                    break;

                case Orbwalking.OrbwalkingMode.None:
                    Orbwalker.SetAttack(true);
                    break;

            }
            /*
            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target != null)
            {
                if (target.ChampionName == "Katarina")
                {
                    if (target.HasBuff("katarinaereduction"))
                    {
                        if (target.IsValidTarget(E.Range))
                        {
                            E.Cast(target.ServerPosition);
                            eattempt = Environment.TickCount;
                        }
                        if (Environment.TickCount - eattempt >= 90f
                            && Q.IsReady())
                            Q.Cast(target.ServerPosition);
                    }
                }
            }
             */
            
            if (Config.Item("qflash").GetValue<KeyBind>().Active)
            {
                flashq();
            }
             
            wcast();
           // Itemusage();
            
        }
        
        private static void Itemusage()
        {                     
            var charm = Config.Item("charm").GetValue<bool>();
            var stun = Config.Item("stun").GetValue<bool>();
            var snare = Config.Item("snare").GetValue<bool>();
            var suppresion = Config.Item("suppression").GetValue<bool>();
            var taunt = Config.Item("taunt").GetValue<bool>();
            // var mikaelshp = Config.Item("mikaelshp").GetValue<Slider>().Value;
            var mikael = ItemData.Mikaels_Crucible.GetItem();
            var locket = ItemData.Locket_of_the_Iron_Solari.GetItem();
            var mountain = ItemData.Face_of_the_Mountain.GetItem();
            
            foreach (var hero in
                HeroManager.Allies.Where(x => x.IsMe))
                {
                    if (Config.Item("faceop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0
                        && hero.HealthPercent <= Config.Item("facehp" + hero.ChampionName).GetValue<Slider>().Value
                        && hero.Distance(Player) >= 750f)
                    {
                        mountain.Cast();
                    }

                    if (Config.Item("locketop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0
                        && hero.HealthPercent <= Config.Item("lockethp" + hero.ChampionName).GetValue<Slider>().Value
                        && hero.Distance(Player) >= 600)
                    {
                        locket.Cast();
                    }
                }
            
             
            
            
            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe
                && x.Distance(Player) <= 750))
            {
                if (Config.Item("healmikaels" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                {

                    if (hero.HasBuffOfType(BuffType.Stun)
                        && stun ||
                        hero.HasBuffOfType(BuffType.Suppression)
                        && suppresion ||
                        hero.HasBuffOfType(BuffType.Taunt)
                        && taunt ||
                        hero.HasBuffOfType(BuffType.Charm)
                        && charm ||
                        hero.HasBuffOfType(BuffType.Snare)
                        && snare
                        || hero.HasBuffOfType(BuffType.CombatDehancer))
                    {
                        mikael.Cast(hero);
                    }
                }

            }
             
        }
         
         




        private static void wcast()
            {
                if (Player.ManaPercent < Config.Item("manalant").GetValue<Slider>().Value)
                    return;
                Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            foreach (var hero in
                HeroManager.Allies.Where(x => !x.IsMe
                                              && !x.IsDead))
            {

                if (Config.Item("healop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0
                    &&
                    hero.HealthPercent <=
                    Config.Item("hpsettings" + hero.ChampionName).GetValue<Slider>().Value
                    && hero.Distance(Player) <= W.Range
                    && target.IsValidTarget(Q.Range))
                {
                    W.Cast(hero.Position);
                }
            }
            }


        private static void Combo()
        {
            Ignite = Player.GetSpellSlot("summonerdot");
            var qSpell = Config.Item("useQ").GetValue<bool>();
            var q2Spell = Config.Item("useQ1").GetValue<bool>();
            var q2Slider = Config.Item("useQ2").GetValue<Slider>().Value;
            var rSpell = Config.Item("useR").GetValue<bool>();
            var eSpell = Config.Item("useE").GetValue<bool>();
           // var wSpell = Config.Item("useW").GetValue<bool>();

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

            if (target.HasBuff("threshQ"))
            {
                lastbuff = Environment.TickCount;
            }

            if (Q.IsReady()
                && (E.IsReady() || ObjectManager.Player.GetSpell(SpellSlot.E).Cooldown <= 3000f)
                && qSpell
                && !target.HasBuff("threshQ")
                && target.IsValidTarget(Q.Range)
                && target.Distance(Player) >= 300)
            {
                Q.Cast(target);
            }

            if (q2Spell
                && target.HasBuff("threshQ"))
            {
                Utility.DelayAction.Add(q2Slider, () => Q.Cast());
                lastq = Environment.TickCount;
            }

            switch (Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (target.IsValidTarget(E.Range)
                        && eSpell
                        && !target.IsImmovable
                        && Environment.TickCount - lastq >= 150)
                    {
                        E.Cast(target.ServerPosition);
                        elastattempt = Environment.TickCount;
                    }
                    break;

                case 1:
                    if (target.IsValidTarget(E.Range)
                        && Environment.TickCount - lastq >= 150
                        && eSpell)
                        E.Cast(target.Position.Extend(Player.ServerPosition,
                            Vector3.Distance(target.Position, Player.Position) + 400));
                    elastattemptin = Environment.TickCount;
                    break;
            }

            if (target.IsValidTarget(R.Range)
                && rSpell
                && ((Environment.TickCount - elastattempt > 250f)
                    || (Environment.TickCount - elastattemptin > 250f)))
            {
                R.Cast();
            }
        }


        private static void flashq()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;
            var x = target.Position.Extend(Prediction.GetPrediction(target, 1).UnitPosition, FlashRange);
            switch (Config.Item("flashmodes").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                Player.Spellbook.CastSpell(FlashSlot, x);
                Q.Cast(x);
                E.Cast(Player.Position);
                    break;

                    /*
                case 1:
                E.Cast(Player.Position);
                Q.Cast(x);
                Player.Spellbook.CastSpell(FlashSlot, x);
                    break;
                     */

                case 1:
                Player.Spellbook.CastSpell(FlashSlot, x);
                Q.Cast(x);
                    break;
            }
        }

        /*
        private static void Mixed()
        {
            throw new NotImplementedException();
        }
         */

        private static void LaneClear()
        {
            var elchSpell = Config.Item("useelch").GetValue<bool>();
          //  var elchSlider = Config.Item("elchslider").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(Player.Position, Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            if (minionCount == null)
                return;
            
            foreach (var minion in minionCount)
            {
                if (elchSpell
                    && minion.IsValidTarget(E.Range)
                    && E.IsReady())
                {
                    E.Cast(minion.Position);
                }
            }
        }
         
        
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
         

        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (!hero.IsMe)
                return;
           // var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if ((args.SData.Name == "threshqinternal" || args.SData.Name == "ThreshQ")
                && Config.Item("autolantern").GetValue<bool>()
                && W.IsReady())
            {
                foreach (var heros in
                    HeroManager.Allies.Where(x => !x.IsMe
                                                  && x.Distance(Player) <= W.Range))
                    {
                    Console.WriteLine("work");
                        Utility.DelayAction.Add(700, () => W.Cast(heros.Position));
                    }
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
        
         
        
        private static void ThreshInterruptableSpell(Obj_AI_Hero sender, Interrupter2.InterruptableTargetEventArgs args)
        {
            if (E.IsReady()
                && E.IsInRange(sender)
                && Config.Item("useE2I").GetValue<bool>())
            {
                E.Cast(sender.ServerPosition);
            }
        }
         

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (!Config.Item("Draw").GetValue<bool>())
                return;

            var qDraw = Config.Item("qDraw").GetValue<bool>();
            var eDraw = Config.Item("eDraw").GetValue<bool>();
            var wDraw = Config.Item("wDraw").GetValue<bool>();
            var qfDraw = Config.Item("qfDraw").GetValue<bool>();

            if (qDraw
                && Q.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);
            }

            if (qfDraw
                && Q.IsReady()
                && FlashSlot.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, 1440, Color.Red);
            }

            if (wDraw
                && W.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);
            }

            if (eDraw
                && E.Level > 0)
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);
            }

            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            if (Q.IsReady()
                && FlashSlot.IsReady()
                && target.Distance(Player) <= Q.Range + 450
                && target.Distance(Player) >= Q.Range - 200)
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

        public static int lastbuff { get; set; }
    }
}
