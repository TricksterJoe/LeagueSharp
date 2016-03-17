using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using Lee_Sin.Misc;
using SharpDX;
using Color = System.Drawing.Color;

namespace Lee_Sin
{
    class EventHandler : LeeSin
    {

        public static void OnCreate(GameObject sender, EventArgs args)
        {
            

            if (!GetBool("wardinsec", typeof(KeyBind)) && !GetBool("starcombo", typeof(KeyBind)) &&
                Orbwalker.ActiveMode != Orbwalking.OrbwalkingMode.Combo && Environment.TickCount - Lastcanjump > 2000
                && Environment.TickCount - Misc.BubbaKush.Lastthingy > 2000)
                return;

            if (ProcessW2 || !W.IsReady() || Player.GetSpell(SpellSlot.W).Name != "BlindMonkWOne" ||
                Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkwtwo")
                return;
            if (sender.Name.ToLower().Contains("ward"))
            {
                var ward = (Obj_AI_Base)sender;
                var target = TargetSelector.GetTarget(Q.Range + 800, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
                }

                if (target == null) return;
                var poss = InsecPos.WardJumpInsecPosition.InsecPos(target, GetValue("fixedwardrange"), true);
                var distancepos = Player.Position.Extend(target.Position, Player.Distance(target) - 150); 
                if (sender.Position.Distance(poss.To3D()) < 100)
                {
                    Lsatcanjump1 = Environment.TickCount;
                }
                if (sender.Position.Distance(distancepos) < 150)
                {
                    LeeSin.Lasttotarget = Environment.TickCount;
                }
            }
            if (sender.Name.ToLower().Contains("ward") && W.IsReady() && sender.IsAlly)
            {
               
                Lastwcasted = Environment.TickCount;
                var ward = (Obj_AI_Base)sender;

                if (ward.IsMe) return;
                W.Cast(ward);
                Created = true;
            }
        }



        #region Ally selector 

        public static void OnWndProc(WndEventArgs args)
        {
            if (args.Msg != (uint)WindowsMessages.WM_LBUTTONDOWN)
            {
                return;
            }

            var asec = ObjectManager.Get<Obj_AI_Hero>().Where(a => a.IsEnemy && a.Distance(Game.CursorPos) < 200 && a.IsValid && !a.IsDead);
            if (asec.Any())
            {
                return;
            }
            if (!LastClickBool || ClickCount == 0)
            {
                ClickCount++;
                LastClickPos = Game.CursorPos;
                LastClickBool = true;
                SelectedAllyAiMinion = null;
                SelectedAllyAiMinionv = new Vector3();
                return;
            }

            if (ClickCounts == 0)
            {
                SelectedAllyAiMinionv = new Vector3();
            }


            if (LastClickBool && LastClickPos.Distance(Game.CursorPos) < 100)
            {
                ClickCount++;
                LastClickBool = false;
            }

            SelectedAllyAiMinion =
                ObjectManager.Get<Obj_AI_Base>()
                    .Where(
                        x => x.IsValid && x.Distance(Game.CursorPos, true) < 40000 && x.IsAlly && !x.IsMe)
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();

        }

        public static int ClickCounts { get; set; }

        public static Vector3 SelectedAllyAiMinionv
        { get; set; }

        #endregion

        #region processspellcast,

        public static void OnSpellcast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //if (sender.IsMe)
            //{
            //    Game.PrintChat(args.SData.Name);
            //}
            //var en = HeroManager.Enemies.Where(x => x.Distance(Player) < 1200).ToList();
            //var getresults = BubbaKush.GetPositions(Player, 1125, (byte)GetValue("enemiescount"), en);
            //if (getresults.Count > 1)
            //{
            //    Game.PrintChat("JUSTDOIT");
            //    if (R.IsReady() && GetBool("xeflash", typeof(bool)))
            //    {
            //        for (int[] i = {0}; i[0] < getresults.Count; i[0]++)
            //        {
            //            var order =
            //                en.Where(a => a.Distance(Player) < R.Range).OrderBy(x => x.Distance(getresults[i[0]]));

            //            if (order.FirstOrDefault() != null)
            //            R.Cast(order.FirstOrDefault());
            //        }
            //    }
                    
            //    if (GetBool("xeflash", typeof (bool)))
            //    {
            //        if (R.IsReady())
            //        if (GetBool("wardinsec", typeof (KeyBind)) ||
            //            Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
            //            return;

            //        var getposition = BubbaKush.SelectBest(getresults, Player);
            //        if (args.SData.Name == "BlindMonkRKick")
            //        {
            //            var poss = getposition;

            //            Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"), poss, true);
            //        }
            //    }
            //}
            if (sender.IsMe)
            {
                if (args.SData.Name.Contains("flash"))
                    LeeSin.LastTeleported = Environment.TickCount;
                switch (args.SData.Name)
                {
                    case "BlindMonkQOne":
                    case "blinkmonkqtwo":
                        Junglelastq = Environment.TickCount;
                        break;

                    case "BlindMonkWOne":
                    case "blindmonkwtwo":
                        Junglelastw = Environment.TickCount;
                        break;

                    case "BlindMonkEOne":
                    case "blindmonketwo":
                        Junglelaste = Environment.TickCount;
                        break;
                }
            }

            if (args.SData.Name.ToLower() == "blindmonkqtwo")
            {
                LeeSin.Lastq2Casted = Environment.TickCount;
            }

            if (args.SData.Name== "BlindMonkQOne")
            {
                LeeSin.Lastq1Casted = Environment.TickCount;
            }

            if (args.SData.Name == "BlindMonkRKick")
            {
                if (Environment.TickCount - Misc.BubbaKush.Lastthingy < 2000 && GetBool("activatebubba", typeof(KeyBind)))
                {
                    var getresults = Misc.BubbaKush.GetPositions(Player, 600, (byte) GetValue("enemiescount"),
                        HeroManager.Enemies.Where(x => x.Distance(Player) < 1200).ToList());
                    if (getresults.Count > 1)
                    {
                        var getposition = Misc.BubbaKush.SelectBest(getresults, Player);

                        var poss = getposition;

                        Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"), poss, true);
                    }
                }   

                Lastr = Environment.TickCount;
                var target = TargetSelector.GetTarget(R.Range, TargetSelector.DamageType.Physical);
                if (target != null)
                {
                    target = TargetSelector.GetSelectedTarget() == null ? target : TargetSelector.SelectedTarget;
                }

                if (target != null && HasFlash())
                {
                   if (Environment.TickCount - LeeSin.Lsatcanjump1 > 3000)
                    {                    
                        if (Steps == LeeSin.steps.Flash ||
                            (Environment.TickCount - Lastflashward < 2000 && Wardjumpedtotarget) ||
                            Environment.TickCount - Lastflashoverprio < 3000 ||
                            Environment.TickCount - Wardjumpedto < 2000
                            ||  Environment.TickCount - Misc.BubbaKush.Lastthingy < 2000) 
                        {
                            if (GetBool("wardinsec", typeof (KeyBind)) || GetBool("starcombo", typeof (KeyBind))
                                || Environment.TickCount - Misc.BubbaKush.Lastthingy < 2000)
                            {
                                var pos = InsecPos.FlashInsecPosition.InsecPos(target, 230);
                                var poss = Player.Position.Extend(target.Position,
                                    +target.Position.Distance(Player.Position) + 230);

                                Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"),
                                    !GetBool("wardinsec", typeof (KeyBind)) ? poss : pos, true);
                            }
                        }
                    }
                }
            }

            if (sender.IsMe || sender.IsAlly || !sender.IsChampion()) return;

            switch (args.SData.Name)
            {
                case "MonkeyKingDecoy":
                case "AkaliSmokeBomb":
                    if (sender.Distance(Player) < E.Range)
                        E.Cast();
                    break;
            }
        }

        #endregion

        #region On spell cast

        public static void OnSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            
            if (args.Slot == SpellSlot.W &&
                (GetBool("wardinsec", typeof(KeyBind)) || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo ||
                Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear))
            {
                Processw = true;
                Lastprocessw = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.Q)
            {
                Lastqcasted = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.Q && Q2())
            {
                Lastqcasted1 = Environment.TickCount;
            }


            if (args.Slot == Player.GetSpellSlot("summonerflash") && GetBool("wardinsec", typeof(KeyBind)))
            {
                Processr = true;
                Lastprocessr = Environment.TickCount;
                Lastflashed = Environment.TickCount;
            }

            if (args.Slot == SpellSlot.R && GetBool("wardinsec", typeof(KeyBind)))
            {

                Processr2 = true;
                Processr2T = Environment.TickCount;
                Playerpos = Player.Position;
            }

            if (args.Slot == SpellSlot.W && (GetBool("wardinsec", typeof(KeyBind)) || Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.LaneClear))
            {
                ProcessW2 = true;
            }

            if (args.Slot == SpellSlot.R && GetBool("wardinsec", typeof(KeyBind)))
            {
                Processr2 = true;
                Processr2T = Environment.TickCount;

            }

        }

        #endregion

        #region #Star

        public static
        Vector2 Star(Obj_AI_Hero target)
        {
            return Player.Position.Extend(target.Position, target.Distance(Player) + 300).To2D();
        }

        #endregion

        public static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "blindmonkqtwo" && args.Target.Type == GameObjectType.obj_AI_Hero)
                {

                }
            }
        }
    }
}