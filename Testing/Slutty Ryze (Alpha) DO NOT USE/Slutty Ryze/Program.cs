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
            DamageIndicator.DamageToUnit = Champion.GetComboDamage;

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
#pragma warning disable 618
            Interrupter.OnPossibleToInterrupt += Champion.RyzeInterruptableSpell;
#pragma warning restore 618
            Orbwalking.BeforeAttack += Champion.Orbwalking_BeforeAttack;
            CustomEvents.Unit.OnDash += Champion.Unit_OnDash;
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
                Champion.AABlock();
                Combo();
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

            if (target.IsValidTarget(Champion.W.Range) && (target.Health < Champion.IgniteDamage(target) + Champion.W.GetDamage(target)))
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
    }
}
