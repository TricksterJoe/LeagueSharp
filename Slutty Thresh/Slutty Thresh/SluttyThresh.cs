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
    internal class SluttyThresh : MenuConfig
    {
        public const string ChampName = "Thresh";
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
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

            Q = new Spell(SpellSlot.Q, 1080);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 500);
            R = new Spell(SpellSlot.R, 400);

            Q.SetSkillshot(0.4f, 60f, 1400f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(0.5f, 50f, 2200f, false, SkillshotType.SkillshotCircle);

            FlashSlot = Player.GetSpellSlot("SummonerFlash");

            CreateMenuu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
            Interrupter2.OnInterruptableTarget += ThreshInterruptableSpell;
            CustomEvents.Unit.OnDash += Unit_OnDash;
            AntiGapcloser.OnEnemyGapcloser += OnGapCloser;
            Obj_AI_Hero.OnProcessSpellCast += Game_ProcessSpell;

        }



        private static void Game_OnUpdate(EventArgs args)
        {
            Orbwalker.SetAttack(true);
            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    Combo();
                    break;

                case Orbwalking.OrbwalkingMode.LaneClear:
                    LaneClear();
                    break;

                case Orbwalking.OrbwalkingMode.LastHit:
                    break;

                case Orbwalking.OrbwalkingMode.Mixed:
                    //   Mixed();
                    break;

                case Orbwalking.OrbwalkingMode.None:
                    break;
            }

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
                        if (Environment.TickCount - eattempt >= 90f + Game.Ping
                            && Q.IsReady())
                            Q.Cast(target.ServerPosition);
                    }
                }
            }


            if (Config.Item("qflash").GetValue<KeyBind>().Active)
                flashq();

            wcast();
            Itemusage();

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
                HeroManager.Allies.Where(x => !x.IsMe))
            {
                if (Config.Item("faceop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (hero.HealthPercent <= Config.Item("facehp" + hero.ChampionName).GetValue<Slider>().Value)
                    {
                        if (hero.Distance(Player) >= 750f)
                            mountain.Cast(hero);
                    }
                }
            }
            foreach (var hero in
                HeroManager.Allies.Where(x => !x.IsMe))
            {
                if (Config.Item("locketop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (hero.HealthPercent <= Config.Item("lockethp" + hero.ChampionName).GetValue<Slider>().Value)
                    {
                        if (hero.Distance(Player) >= 600)
                            locket.Cast();
                    }
                }
            }






            foreach (var hero in HeroManager.Allies.Where(x => !x.IsMe))
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
                        if (hero.Distance(Player) <= 750f)
                            mikael.Cast(hero);
                    }
                }
            }
        }


        private static void wcast()
        {
            if (Player.ManaPercent < Config.Item("manalant").GetValue<Slider>().Value)
                return;
           // Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            foreach (var hero in
                HeroManager.Allies.Where(x => !x.IsMe
                                              && !x.IsDead))
            {
                if (Config.Item("healop" + hero.ChampionName).GetValue<StringList>().SelectedIndex == 0)
                {
                    if (hero.HealthPercent <= Config.Item("hpsettings" + hero.ChampionName).GetValue<Slider>().Value
                        && hero.Distance(Player) <= W.Range)
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
            var qrange1 = Config.Item("qrange").GetValue<Slider>().Value;
            var rslider = Config.Item("rslider").GetValue<Slider>().Value;
            var rSpell = Config.Item("useR").GetValue<bool>();
            var eSpell = Config.Item("useE").GetValue<bool>();
           // var wSpell = Config.Item("useW").GetValue<bool>();

            Obj_AI_Hero target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (target == null)
                return;

            if (target.HasBuff("threshQ")
                || (Player.Distance(target) <= 650 && E.IsReady()))
                Orbwalker.SetAttack(false);
            else
                Orbwalker.SetAttack(true);

            if (target.HasBuff("threshQ"))
            {
                lastbuff = Environment.TickCount;
            }
            if (Q.IsReady()
                && (E.IsReady() || ObjectManager.Player.GetSpell(SpellSlot.E).Cooldown <= 3000f)
                && qSpell
                && !target.HasBuff("threshQ")
                && target.IsValidTarget(Q.Range)
                && target.Distance(Player) >= qrange1)
            {
                Q.Cast(target);
                lastq = Environment.TickCount;
            }

            if (q2Spell
                && target.HasBuff("threshQ"))
            {
                Utility.DelayAction.Add(q2Slider, () => Q.Cast());
                
            }

            switch (Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                    if (target.IsValidTarget(E.Range)
                        && eSpell
                        && !target.IsImmovable
                        && Environment.TickCount - lastq >= 40 + Game.Ping)
                    {
                        E.Cast(target.ServerPosition);
                        elastattempt = Environment.TickCount;
                    }
                    break;

                case 1:
                    if (target.IsValidTarget(E.Range)
                        && Environment.TickCount - lastq >= 40 + Game.Ping 
                        && eSpell)
                        E.Cast(target.Position.Extend(Player.ServerPosition,
                            Vector3.Distance(target.Position, Player.Position) + 400));
                    elastattemptin = Environment.TickCount;
                    break;
            }

            if (rSpell
                && Player.CountEnemiesInRange(R.Range - 30) >= rslider
                && ((Environment.TickCount - elastattempt > 180f + Game.Ping)
                    || (Environment.TickCount - elastattemptin > 180f + Game.Ping)))
                R.Cast();
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
                return;

            if (E.IsInRange(gapcloser.Start))
                E.Cast(Player.Position.Extend(gapcloser.Sender.Position, 400));
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
                        Utility.DelayAction.Add(400, () => W.Cast(heros.Position));
                }
            }
        }

        
        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);

            if (!sender.IsEnemy)
                return;

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
                E.Cast(sender.ServerPosition);
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
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.Green);

            if (qfDraw
                && Q.IsReady()
                && FlashSlot.IsReady())
                Render.Circle.DrawCircle(Player.Position, 1440, Color.Red);

            if (wDraw
                && W.Level > 0)
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Black);

            if (eDraw
                && E.Level > 0)
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.Gold);

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
