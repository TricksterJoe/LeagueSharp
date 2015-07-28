using System;
using System.Linq;
using LeagueSharp;
using Color = System.Drawing.Color;
using LeagueSharp.Common;

namespace Slutty_ryze
{
    internal class Program
    {

        private static void Main(string[] args)
        {
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
            DamageIndicator.DamageToUnit = GetComboDamage;

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
#pragma warning disable 618
            Interrupter.OnPossibleToInterrupt += RyzeInterruptableSpell;
#pragma warning restore 618
            Orbwalking.BeforeAttack += Orbwalking_BeforeAttack;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

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
                AABlock();
                Combo();
            }

            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Mixed)
            {
                Mixed();
                MenuManager.Orbwalker.SetAttack(true);
            }

            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear)
            {
                if (GlobalManager.Config.Item("disablelane").GetValue<KeyBind>().Active)
                    LaneClear();


                if (GlobalManager.Config.Item("presslane").GetValue<KeyBind>().Active)
                    LaneClear();


                MenuManager.Orbwalker.SetAttack(true);
                JungleClear();
            }

            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LastHit)
                LastHit();


            if (MenuManager.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.None)
            {
                if (GlobalManager.Config.Item("tearS").GetValue<KeyBind>().Active)
                    ItemManager.TearStack();

                if (GlobalManager.Config.Item("autoPassive").GetValue<KeyBind>().Active)
                    AutoPassive();

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
            KillSteal();
            ItemManager.Potion();

            if (GlobalManager.Config.Item("level").GetValue<bool>())
            {
                AutoLevelManager.LevelUpSpells();
            }
            if (GlobalManager.Config.Item("autow").GetValue<bool>()
                && target.UnderTurret(true))
            {
                if (target == null)
                    return;

                if (ObjectManager.Get<Obj_AI_Turret>()
                    .Any(turret => turret.IsValidTarget(300) && turret.IsAlly && turret.Health > 0))
                {
                    Champion.W.CastOnUnit(target);
                }
            }
        }

        private static float IgniteDamage(Obj_AI_Hero target)
        {
            if (Champion.GetIgniteSlot() == SpellSlot.Unknown || GlobalManager.GetHero.Spellbook.CanUseSpell(Champion.GetIgniteSlot()) != SpellState.Ready)
                return 0f;
            return (float)GlobalManager.GetHero.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
        }


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

  

        private static void RyzeInterruptableSpell(Obj_AI_Base unit, InterruptableSpell spell)
        {
            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);
            var wSpell = GlobalManager.Config.Item("useW2I").GetValue<bool>();
            if (wSpell)
                Champion.W.CastOnUnit(target);
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            if (!sender.IsEnemy) return;

            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);
            var qSpell = GlobalManager.Config.Item("useQW2D").GetValue<bool>();

            if (sender.NetworkId != target.NetworkId) return;
            if (!qSpell) return;
            if (!Champion.Q.IsReady() || !(args.EndPos.Distance(GlobalManager.GetHero) < Champion.Q.Range)) return;
            var delay = (int)(args.EndTick - Game.Time - Champion.Q.Delay - 0.1f);

            if (delay > 0)
                Utility.DelayAction.Add(delay * 1000, () => Champion.Q.Cast(args.EndPos));
            else
                Champion.Q.Cast(args.EndPos);

            if (!Champion.Q.IsReady() || !(args.EndPos.Distance(GlobalManager.GetHero) < Champion.Q.Range)) return;

            if (delay > 0)
                Utility.DelayAction.Add(delay * 1000, () => Champion.Q.Cast(args.EndPos));
            else
                Champion.W.CastOnUnit(target);
        }

        private static Color GetColor(bool b)
        {
            return b ? Color.DarkGreen : Color.Red;
        }

        private static string BoolToString(bool b)
        {
            return b ? "On" : "off";
        }
       /*
        static Point[] getPoints(Vector2 c, int R)
        {
            int X = (int)c.X;
            int Y = (int)c.Y;

            int R2 = (int)(R / Math.PI);

            Point[] pt = new Point[5];

            pt[0].X = X;
            pt[0].Y = Y;

            pt[1].X = X;
            pt[1].Y = Y + R * 2;

            pt[2].X = X + R * 2;
            pt[2].Y = Y;

            pt[3].X = X - R * 2;
            pt[3].Y = Y;

            pt[4].X = X;
            pt[4].Y = Y - R * 2;

            return pt;
        }

        private static void drawCircleThing(int radius, SharpDX.Vector2 center, Color c)
        {
            var lineWalls = getPoints(center,radius);
            Drawing.DrawLine(lineWalls[0], lineWalls[1], 2, c);
            Drawing.DrawLine(lineWalls[0], lineWalls[2], 2, c);
            Drawing.DrawLine(lineWalls[0], lineWalls[3], 2, c);
            Drawing.DrawLine(lineWalls[0], lineWalls[4], 2, c);
        }
        */
 
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (GlobalManager.GetHero.IsDead)
                return;
            if (!GlobalManager.Config.Item("Draw").GetValue<bool>())
                return;
            if (!GlobalManager.GetHero.Position.IsOnScreen())
                return;

           // drawCircleThing((int)Champion.Q.Range/2, Drawing.WorldToScreen(GlobalManager.GetHero.Position), Color.Pink);

            if (GlobalManager.Config.Item("qDraw").GetValue<bool>() && Champion.Q.Level > 0)
                Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.Q.Range, Color.Green);
            if (GlobalManager.Config.Item("eDraw").GetValue<bool>() && Champion.E.Level > 0)
                Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.E.Range, Color.Gold);
            if (GlobalManager.Config.Item("wDraw").GetValue<bool>() && Champion.W.Level > 0)
                Render.Circle.DrawCircle(GlobalManager.GetHero.Position, Champion.W.Range, Color.Black);

            var tears = GlobalManager.Config.Item("tearS").GetValue<KeyBind>().Active;
            var passive = GlobalManager.Config.Item("autoPassive").GetValue<KeyBind>().Active;
            var laneclear = GlobalManager.Config.Item("disablelane").GetValue<KeyBind>().Active;

            if (!GlobalManager.Config.Item("notdraw").GetValue<bool>()) return;

            var heroPosition = Drawing.WorldToScreen(GlobalManager.GetHero.Position);
            var textDimension = Drawing.GetTextExtent("Stunnable!");

            Drawing.DrawText(heroPosition.X - textDimension.Width, heroPosition.Y - textDimension.Height,
                GetColor(tears),
                "Tear Stack: " + BoolToString(tears));

            Drawing.DrawText(heroPosition.X - 150, heroPosition.Y - 30, GetColor(passive),
                "Passive Stack: " + BoolToString(passive));

            Drawing.DrawText(heroPosition.X + 20, heroPosition.Y - 30, GetColor(laneclear),
                "Lane Clear: " + BoolToString(laneclear));
            
        }

        private static void Combo()
        {
            Champion.SetIgniteSlot(GlobalManager.GetHero.GetSpellSlot("summonerdot"));
            var qSpell = GlobalManager.Config.Item("useQ").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("useE").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("useW").GetValue<bool>();
            var rSpell = GlobalManager.Config.Item("useR").GetValue<bool>();
            var rwwSpell = GlobalManager.Config.Item("useRww").GetValue<bool>();
            var target = TargetSelector.GetTarget(Champion.W.Range, TargetSelector.DamageType.Magical);

            if (!target.IsValidTarget(Champion.Q.Range)) return;

            if (target.IsValidTarget(Champion.W.Range) && (target.Health < IgniteDamage(target) + Champion.W.GetDamage(target)))
                GlobalManager.GetHero.Spellbook.CastSpell(Champion.GetIgniteSlot(), target);


            switch (GlobalManager.Config.Item("combooptions").GetValue<StringList>().SelectedIndex)
            {
                case 1:
                    if (Champion.R.IsReady())
                    {
                        if (GlobalManager.GetPassiveBuff == 1 || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();

                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetPassiveBuff == 2)
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                    if (target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                            }
                        }

                        if (GlobalManager.GetPassiveBuff == 3)
                        {
                            if (Champion.Q.IsReady()
                                && target.IsValidTarget(Champion.Q.Range))
                                Champion.Qn.Cast(target);

                            if (Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW")
                                        && (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()))
                                        Champion.R.Cast();

                                    if (!rwwSpell
                                        && (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()))
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.Qn.Range)
                                && Champion.Q.IsReady()
                                && qSpell)
                                Champion.Qn.Cast(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && Champion.E.IsReady()
                                && eSpell)
                                Champion.E.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();

                                    if (!rwwSpell)
                                        Champion.R.Cast();

                                    if (!Champion.Q.IsReady() && !Champion.W.IsReady() && !Champion.E.IsReady())
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (wSpell
                                && Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (eSpell
                                && Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                    if (!Champion.E.IsReady() && !Champion.Q.IsReady() && !Champion.W.IsReady())
                                        Champion.R.Cast();
                                }
                            }
                        }
                    }

                    if (!Champion.R.IsReady())
                    {
                        if (GlobalManager.GetPassiveBuff == 1
                            || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);
                        }

                        if (GlobalManager.GetPassiveBuff == 2)
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetPassiveBuff == 3)
                        {
                            if (Champion.Q.IsReady()
                                && target.IsValidTarget(Champion.Q.Range))
                                Champion.Qn.Cast(target);

                            if (Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);
                        }

                        if (GlobalManager.GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(Champion.E.Range)
                                && Champion.E.IsReady()
                                && eSpell)
                                Champion.E.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.Qn.Range)
                                && Champion.Q.IsReady()
                                && qSpell)
                                Champion.Qn.Cast(target);
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (wSpell
                                && Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (eSpell
                                && Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);
                        }
                    }
                    break;


                case 0:

                    if (target.IsValidTarget(Champion.Q.Range))
                    {
                        if (GlobalManager.GetPassiveBuff <= 2
                            || !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))
                        {
                            if (target.IsValidTarget(Champion.Q.Range)
                                && qSpell
                                && Champion.Q.IsReady())
                                Champion.Q.Cast(target);

                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && eSpell
                                && Champion.E.IsReady())
                                Champion.E.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }


                        if (GlobalManager.GetPassiveBuff == 3)
                        {
                            if (Champion.Q.IsReady()
                                && target.IsValidTarget(Champion.Q.Range))
                                Champion.Qn.Cast(target);

                            if (Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetPassiveBuff == 4)
                        {
                            if (target.IsValidTarget(Champion.W.Range)
                                && wSpell
                                && Champion.W.IsReady())
                                Champion.W.CastOnUnit(target);

                            if (target.IsValidTarget(Champion.Qn.Range)
                                && Champion.Q.IsReady()
                                && qSpell)
                                Champion.Qn.Cast(target);

                            if (target.IsValidTarget(Champion.E.Range)
                                && Champion.E.IsReady()
                                && eSpell)
                                Champion.E.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                }
                            }
                        }

                        if (GlobalManager.GetHero.HasBuff("ryzepassivecharged"))
                        {
                            if (wSpell
                                && Champion.W.IsReady()
                                && target.IsValidTarget(Champion.W.Range))
                                Champion.W.CastOnUnit(target);

                            if (qSpell
                                && Champion.Qn.IsReady()
                                && target.IsValidTarget(Champion.Qn.Range))
                                Champion.Qn.Cast(target);

                            if (eSpell
                                && Champion.E.IsReady()
                                && target.IsValidTarget(Champion.E.Range))
                                Champion.E.CastOnUnit(target);

                            if (Champion.R.IsReady()
                                && rSpell)
                            {
                                if (target.IsValidTarget(Champion.W.Range)
                                    && target.Health > (Champion.Q.GetDamage(target) + Champion.E.GetDamage(target)))
                                {
                                    if (rwwSpell && target.HasBuff("RyzeW"))
                                        Champion.R.Cast();
                                    if (!rwwSpell)
                                        Champion.R.Cast();
                                    if (!Champion.E.IsReady() && !Champion.Q.IsReady() && !Champion.W.IsReady())
                                        Champion.R.Cast();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (wSpell
                            && Champion.W.IsReady()
                            && target.IsValidTarget(Champion.W.Range))
                            Champion.W.CastOnUnit(target);

                        if (qSpell
                            && Champion.Qn.IsReady()
                            && target.IsValidTarget(Champion.Qn.Range))
                            Champion.Qn.Cast(target);

                        if (eSpell
                            && Champion.E.IsReady()
                            && target.IsValidTarget(Champion.E.Range))
                            Champion.E.CastOnUnit(target);
                    }
                    break;
            }

            if (!Champion.R.IsReady() || GlobalManager.GetPassiveBuff != 4 || !rSpell) return;

            if (Champion.Q.IsReady() || Champion.W.IsReady() || Champion.E.IsReady()) return;

            Champion.R.Cast();
        }

        private static void LaneClear()
        {
            if (GlobalManager.GetPassiveBuff == 4
                && !GlobalManager.GetHero.HasBuff("RyzeR")
                && GlobalManager.Config.Item("passiveproc").GetValue<bool>())
                return;

            var qlchSpell = GlobalManager.Config.Item("useQlc").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useElc").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWlc").GetValue<bool>();
            var q2LSpell = GlobalManager.Config.Item("useQ2L").GetValue<bool>();
            var e2LSpell = GlobalManager.Config.Item("useE2L").GetValue<bool>();
            var w2LSpell = GlobalManager.Config.Item("useW2L").GetValue<bool>();
            var rSpell = GlobalManager.Config.Item("useRl").GetValue<bool>();
            var rSlider = GlobalManager.Config.Item("rMin").GetValue<Slider>().Value;
            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;
            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            if (GlobalManager.GetHero.ManaPercent <= minMana)
                return;

            foreach (var minion in minionCount)
            {
                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minion.Health <= Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range)
                    && minion.Health <= Champion.W.GetDamage(minion))
                    Champion.W.CastOnUnit(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range)
                    && minion.Health <= Champion.E.GetDamage(minion))
                    Champion.E.CastOnUnit(minion);

                if (q2LSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minion.Health >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3))
                    Champion.Q.Cast(minion);

                if (e2LSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range)
                    && minion.Health >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3))
                    Champion.E.CastOnUnit(minion);

                if (w2LSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range)
                    && minion.Health >= (GlobalManager.GetHero.GetAutoAttackDamage(minion) * 1.3))
                    Champion.W.CastOnUnit(minion);

                if (rSpell
                    && Champion.R.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range)
                    && minionCount.Count > rSlider)
                    Champion.R.Cast();
            }
        }


        private static void JungleClear()
        {
            var qSpell = GlobalManager.Config.Item("useQj").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("useEj").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("useWj").GetValue<bool>();
            var rSpell = GlobalManager.Config.Item("useRj").GetValue<bool>();
            var mSlider = GlobalManager.Config.Item("useJM").GetValue<Slider>().Value;


            if (GlobalManager.GetHero.ManaPercent < mSlider)
                return;


            var jungle = MinionManager.GetMinions(Champion.Q.Range, MinionTypes.All, MinionTeam.Neutral,
                MinionOrderTypes.MaxHealth).FirstOrDefault();

            if (!jungle.IsValidTarget())
                return;

            if (eSpell
                && jungle.IsValidTarget(Champion.E.Range)
                && Champion.E.IsReady())
                Champion.E.CastOnUnit(jungle);
            if (qSpell
                && jungle.IsValidTarget(Champion.Q.Range)
                && Champion.Q.IsReady())
                Champion.Q.Cast(jungle);

            if (wSpell
                && jungle.IsValidTarget(Champion.W.Range)
                && Champion.W.IsReady())
                Champion.W.CastOnUnit(jungle);

            if (!rSpell || (GlobalManager.GetPassiveBuff != 4 && !GlobalManager.GetHero.HasBuff("RyzePassiveStack"))) return;

            Champion.R.Cast();
        }

        private static void LastHit()
        {
            var qlchSpell = GlobalManager.Config.Item("useQl2h").GetValue<bool>();
            var elchSpell = GlobalManager.Config.Item("useEl2h").GetValue<bool>();
            var wlchSpell = GlobalManager.Config.Item("useWl2h").GetValue<bool>();

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);

            foreach (var minion in minionCount)
            {
                if (qlchSpell
                    && Champion.Q.IsReady()
                    && minion.IsValidTarget(Champion.Q.Range - 20)
                    && minion.Health < Champion.Q.GetDamage(minion))
                    Champion.Q.Cast(minion);

                if (wlchSpell
                    && Champion.W.IsReady()
                    && minion.IsValidTarget(Champion.W.Range - 10)
                    && minion.Health < Champion.W.GetDamage(minion))
                    Champion.W.CastOnUnit(minion);

                if (elchSpell
                    && Champion.E.IsReady()
                    && minion.IsValidTarget(Champion.E.Range - 10)
                    && minion.Health < Champion.E.GetDamage(minion))
                    Champion.E.CastOnUnit(minion);
            }
        }

        private static void Mixed()
        {
            var qSpell = GlobalManager.Config.Item("UseQM").GetValue<bool>();
            var qlSpell = GlobalManager.Config.Item("UseQMl").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("UseEM").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("UseWM").GetValue<bool>();
            var minMana = GlobalManager.Config.Item("useEPL").GetValue<Slider>().Value;

            if (GlobalManager.GetHero.ManaPercent < GlobalManager.Config.Item("mMin").GetValue<Slider>().Value)
                return;

            var target = TargetSelector.GetTarget(900, TargetSelector.DamageType.Magical);
            if (qSpell
                && Champion.Q.IsReady()
                && target.IsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.IsReady()
                && target.IsValidTarget(Champion.W.Range))
                Champion.W.CastOnUnit(target);

            if (eSpell
                && Champion.E.IsReady()
                && target.IsValidTarget(Champion.E.Range))
                Champion.E.CastOnUnit(target);

            var minionCount = MinionManager.GetMinions(GlobalManager.GetHero.Position, Champion.Q.Range, MinionTypes.All, MinionTeam.NotAlly);
            {
                if (GlobalManager.GetHero.ManaPercent <= minMana)
                    return;

                foreach (var minion in minionCount)
                {
                    if (!qlSpell || !Champion.Q.IsReady() || !(minion.Health < Champion.Q.GetDamage(minion))) continue;
                    Champion.Q.Cast(minion);
                }
            }
        }

        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);
            if (target == null || !target.IsValidTarget() || target.IsInvulnerable)
                return;

            var qSpell = GlobalManager.Config.Item("useQ2KS").GetValue<bool>();
            var wSpell = GlobalManager.Config.Item("useW2KS").GetValue<bool>();
            var eSpell = GlobalManager.Config.Item("useE2KS").GetValue<bool>();
            if (qSpell
                && Champion.Q.GetDamage(target) > target.Health
                && target.IsValidTarget(Champion.Q.Range))
                Champion.Q.Cast(target);

            if (wSpell
                && Champion.W.GetDamage(target) > target.Health
                && target.IsValidTarget(Champion.W.Range))
                Champion.W.CastOnUnit(target);

            if (eSpell
                && Champion.E.GetDamage(target) > target.Health
                && target.IsValidTarget(Champion.E.Range))
                Champion.E.CastOnUnit(target);
        }


        private static void AABlock()
        {
            var aaBlock = GlobalManager.Config.Item("AAblock").GetValue<bool>();
            if (aaBlock)
                MenuManager.Orbwalker.SetAttack(false);
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            if (Champion.Q.IsReady() || GlobalManager.GetHero.Mana <= Champion.Q.Instance.ManaCost * 5)
                return Champion.Q.GetDamage(enemy) * 5;

            if (Champion.E.IsReady() || GlobalManager.GetHero.Mana <= Champion.E.Instance.ManaCost * 5)
                return Champion.E.GetDamage(enemy) * 5;

            if (Champion.W.IsReady() || GlobalManager.GetHero.Mana <= Champion.W.Instance.ManaCost * 3)
                return Champion.W.GetDamage(enemy) * 3;

            return 0;
        }

      

        private static void Orbwalking_BeforeAttack(Orbwalking.BeforeAttackEventArgs args)
        {
            var mura = GlobalManager.Config.Item("muramana").GetValue<bool>();

            if (!mura) return;

            var muramanai = Items.HasItem(ItemManager.Muramana) ? 3042 : 3043;

            if (!args.Target.IsValid<Obj_AI_Hero>() || !args.Target.IsEnemy || !Items.HasItem(muramanai) ||
                !Items.CanUseItem(muramanai))
                return;

            if (!GlobalManager.GetHero.HasBuff("Muramana"))
                Items.UseItem(muramanai);
        }
     
        private static void AutoPassive()
        {
            var minions = MinionManager.GetMinions(
                GlobalManager.GetHero.ServerPosition, Champion.Q.Range, MinionTypes.All, MinionTeam.Enemy,
                MinionOrderTypes.MaxHealth);

            if (GlobalManager.GetHero.Mana < GlobalManager.Config.Item("ManapSlider").GetValue<Slider>().Value) return;

            if (GlobalManager.GetHero.IsRecalling() || minions.Count >= 1) return;

            var target = TargetSelector.GetTarget(Champion.Q.Range, TargetSelector.DamageType.Magical);

            if (target != null) return;

            var stackSliders = GlobalManager.Config.Item("stackSlider").GetValue<Slider>().Value;
            if (GlobalManager.GetHero.IsRecalling() || GlobalManager.GetHero.InFountain()) return;

            if (GlobalManager.GetPassiveBuff >= stackSliders)
                return;

            if (Environment.TickCount - Champion.Q.LastCastAttemptT >=
                GlobalManager.Config.Item("autoPassiveTimer").GetValue<Slider>().Value*1000 - (100 + Game.Ping) &&
                Champion.Q.IsReady())
            {
                if (!Game.CursorPos.IsZero)
                    Champion.Q.Cast(Game.CursorPos);
                else
                    Champion.Q.Cast();
            }
            Console.WriteLine(Game.Ping);

        }

    }
}
