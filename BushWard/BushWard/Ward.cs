using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace BushWard
{
    internal class Ward
    {
      //  private static bool getiton;
        private static int lastwarded;
        
        public static void OnLoad(EventArgs args)
        {
            Game.OnUpdate += OnUpdate;
            Game.OnChat += OnChat; //this is me being lazy
            Game.PrintChat("<font color='#6f00ff'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "To Enable, Type w on, To Disable, Type w off" + "</font>");

        }

        private static void OnChat(GameChatEventArgs args)
        {

            if (!args.Sender.IsMe) return;
           
            if (args.Message.Equals("w on"))
            {

                if (!getiton)
                    Utility.DelayAction.Add(200, () => Game.PrintChat("<font color='#04B404'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "On" + "</font>"));
                else
                {
                    Utility.DelayAction.Add(200, () => Game.PrintChat("<font color='#04B404'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "Already On" + "</font>"));
                }
                getiton = true;
            }

            if (args.Message.Equals("w off"))
            {
                if (getiton)
                   Utility.DelayAction.Add(200, () =>  Game.PrintChat("<font color='#FF0000'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "Off" + "</font>"));
                else
                {
                    Utility.DelayAction.Add(200, () => Game.PrintChat("<font color='#FF0000'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "Already Off" + "</font>"));
                }
                getiton = false;
            }
        }

        public static bool getiton { get; set; }


        private static void OnUpdate(EventArgs args)
        {
            if (!getiton) return;

           var random = WeightedRandom.Next(200, 700);
            
            var combo = Orbwalking.Orbwalker.Instances.Find(x => x.ActiveMode == Orbwalking.OrbwalkingMode.Combo);

            if (combo == null)
            {
                return;
            }

            foreach (var heros in HeroManager.Enemies.Where(x => !x.IsDead && x.Distance(ObjectManager.Player) < 1000))
            {
                var path = heros.GetWaypoints().LastOrDefault().To3D();
               // if (path == new Vector3(null)) return;
                if (NavMesh.IsWallOfGrass(path, 1))
                {
                    //Game.PrintChat("test");
                   if (heros.Distance(path) > 200) return;
                   if (NavMesh.IsWallOfGrass(ObjectManager.Player.Position, 1) && ObjectManager.Player.Distance(path) < 200) return;
                    if (ObjectManager.Player.Distance(path) < 500)
                    {
                       
                        foreach (
    var obj in ObjectManager.Get<Obj_AI_Base>().Where(x => x.Name.ToLower().Contains("ward")
                                                           && x.IsAlly && x.Distance(path) < 300))
                        {
                            // Game.PrintChat("hi");
                            if (NavMesh.IsWallOfGrass(obj.Position, 1)) return;
                            //   }
                        }
                        var items = Items.GetWardSlot();
                        if (items != null && Environment.TickCount - lastwarded > 1000)
                        {
                          Utility.DelayAction.Add(random, () =>   ObjectManager.Player.Spellbook.CastSpell(items.SpellSlot, path));
                            lastwarded = Environment.TickCount;
                        }
                    }
                }
            }
        }   
    }
}
