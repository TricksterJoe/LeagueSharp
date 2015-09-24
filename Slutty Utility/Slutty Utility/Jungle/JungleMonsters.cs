using System.Collections.Generic;
using System.Linq;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Utility.Jungle
{
    class JungleMonsters
    { 
        public static List<Camp> JungleCamps = new List<Camp>();

        public struct Monster
        {
            public string Name;
            public bool BigMob;
            public bool IsDead;
            public Monster(string name, bool bigMob = false,bool isDead=false)
            {
                Name = name;
                BigMob = bigMob;
                IsDead = isDead;
            }
        }
        
        public struct Camp
        {
            public float SpawnTime;
            public float RespawnTime;
            public Vector2 Location;
            public bool IsDead;
            public List<Monster> Monsters;

            public Camp(float spawnTime, float respawnTime, Vector2 location, List<Monster> monsters,bool isDead=false)
            {
                SpawnTime = spawnTime;
                RespawnTime = respawnTime;
                Location = location;
                IsDead = isDead;
                Monsters = monsters;
            }
        }
        public static void LoadCamps()
        {
            if (JungleCamps.Count > 1) return;

            JungleCamps.Add(
                new Camp(115, 300, SummonersRift.Jungle.Blue_RedBuff, new List<Monster>(new[]
                {
                    new Monster("SRU_Blue1.1.1", true),
                    new Monster("SRU_BlueMini1.1.2"),
                    new Monster("SRU_BlueMini21.1.3")
                })));


            JungleCamps.Add(
                new Camp(115, 300, SummonersRift.Jungle.Blue_BlueBuff, new List<Monster>(new[]
                {
                    new Monster("SRU_Red4.1.1", true),
                    new Monster("SRU_RedMini4.1.2"),
                    new Monster("SRU_RedMini4.1.3")
                })));

            JungleCamps.Add(
               new Camp(115, 100, SummonersRift.Jungle.Blue_Wolf, new List<Monster>(new[]
               {
                    new Monster("SRU_Murkwolf2.1.1", true),
                    new Monster("SRU_MurkwolfMini2.1.2"),
                    new Monster("SRU_MurkwolfMini2.1.3")
               })));

            JungleCamps.Add(
               new Camp(115, 100, SummonersRift.Jungle.Blue_RazorBeak, new List<Monster>(new[]
               {
                    new Monster("SRU_Razorbeak3.1.1", true),
                    new Monster("SRU_RazorbeakMini3.1.2"),
                    new Monster("SRU_RazorbeakMini3.1.3"),
                    new Monster("SRU_RazorbeakMini3.1.4")
               })));

            JungleCamps.Add(
             new Camp(115, 100, SummonersRift.Jungle.Blue_Krug, new List<Monster>(new[]
             {
                    new Monster("SRU_Krug5.1.2", true),
                    new Monster("SRU_KrugMini5.1.1")
             })));

            JungleCamps.Add(
            new Camp(115, 100, SummonersRift.Jungle.Blue_Gromp, new List<Monster>(new[]
            {
                    new Monster("SRU_Gromp13.1.1", true)
            })));

            JungleCamps.Add(
                new Camp(115, 300, SummonersRift.Jungle.Red_BlueBuff, new List<Monster>(new[]
                {
                    new Monster("SRU_Blue7.1.1", true),
                    new Monster("SRU_BlueMini7.1.2"),
                    new Monster("SRU_BlueMini27.1.3")
                })));


            JungleCamps.Add(
                new Camp(115, 300, SummonersRift.Jungle.Red_RedBuff, new List<Monster>(new[]
                {
                    new Monster("SRU_Red10.1.1", true),
                    new Monster("SRU_RedMini10.1.2"),
                    new Monster("SRU_RedMini10.1.3")
                })));

            JungleCamps.Add(
               new Camp(115, 100, SummonersRift.Jungle.Red_Wolf, new List<Monster>(new[]
               {
                    new Monster("SRU_Murkwolf8.1.1", true),
                    new Monster("SRU_MurkwolfMini8.1.2"),
                    new Monster("SRU_MurkwolfMini8.1.3")
               })));

            JungleCamps.Add(
               new Camp(115, 100, SummonersRift.Jungle.Red_RazorBeak, new List<Monster>(new[]
               {
                    new Monster("SRU_Razorbeak9.1.1", true),
                    new Monster("SRU_RazorbeakMini9.1.2"),
                    new Monster("SRU_RazorbeakMini9.1.3"),
                    new Monster("SRU_RazorbeakMini9.1.4")
               })));

            JungleCamps.Add(
             new Camp(115, 100, SummonersRift.Jungle.Red_Krug, new List<Monster>(new[]
             {
                    new Monster("SRU_Krug11.1.2", true),
                    new Monster("SRU_KrugMini11.1.1")
             })));

            JungleCamps.Add(
            new Camp(115, 100, SummonersRift.Jungle.Red_Gromp, new List<Monster>(new[]
            {
                    new Monster("SRU_Gromp14.1.1", true)
            })));

            JungleCamps.Add(
           new Camp(150, 360, SummonersRift.River.Dragon, new List<Monster>(new[]
           {
                    new Monster("SRU_Dragon6.1.1", true)
           })));

            JungleCamps.Add(
           new Camp(120, 420, SummonersRift.River.Baron, new List<Monster>(new[]
           {
                    new Monster("SRU_Baron12.1.1", true)
           })));
//
//            JungleCamps.Add("Blue_BlueBuff", new Monster(115, 300, SummonersRift.Jungle.Blue_BlueBuff));
//            JungleCamps.Add("Blue_Gromp", new Monster(115, 100, SummonersRift.Jungle.Blue_Gromp));
//            JungleCamps.Add("Blue_Krug", new Monster(115, 100, SummonersRift.Jungle.Blue_Krug));
//            JungleCamps.Add("Blue_RazorBeak", new Monster(115, 100, SummonersRift.Jungle.Blue_RazorBeak));
//            JungleCamps.Add("Blue_Murkwolf", new Monster(115, 100, SummonersRift.Jungle.Blue_Wolf));
//
//            JungleCamps.Add("Red_RedBuff", new Monster(115, 300, SummonersRift.Jungle.Red_RedBuff));
//            JungleCamps.Add("Red_RedBuff", new Monster(115, 300, SummonersRift.Jungle.Red_RedBuff));
//            JungleCamps.Add("Red_Gromp", new Monster(115, 100, SummonersRift.Jungle.Red_Gromp));
//            JungleCamps.Add("Red_Krug", new Monster(115, 100, SummonersRift.Jungle.Red_Krug));
//            JungleCamps.Add("Red_RazorBeak", new Monster(115, 100, SummonersRift.Jungle.Red_RazorBeak));
//            JungleCamps.Add("Red_Murkwolf", new Monster(115, 100, SummonersRift.Jungle.Red_Wolf));
//
//            JungleCamps.Add("Neutral_Baron", new Monster(115, 420, SummonersRift.River.Baron));
//            JungleCamps.Add("Neutral_Dragon", new Monster(115, 360, SummonersRift.River.Dragon));
        }
    }
}
