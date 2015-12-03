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
        public static Menu Config;
        private static int lastwarded;
        
        public static void OnLoad(EventArgs args)
        {
            Config = new Menu("Auto Ward Bush", "Auto Ward Bush");
            Config.AddItem(new MenuItem("Enable", "Enable")).SetValue(true);
            Config.AddItem(new MenuItem("Enable Humanizer", "EnableHumanizer")).SetValue(true);
            Config.AddToMainMenu();
            Game.OnUpdate += OnUpdate;
            Game.PrintChat("<font color='#6f00ff'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "Make sure to upvote in Database :)" + "</font>");

        }

        

        private static void OnUpdate(EventArgs args)
        {
            if (!Config.Item("Enable").GetValue<bool>()) return;

           var random = Config.Item("EnableHumanizer").GetValue<bool>() ? WeightedRandom.Next(200, 700) : 0;
            
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
