using System.Collections.Generic;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Utility.Jungle
{
    class JungleMonsters
    { 
        public static Dictionary<string,Camp> JungleCamps = new Dictionary<string, Camp>();

        public struct Monster
        {
            public string Name;
            public bool BigMob;

            public Monster(string name, bool bigMob = false)
            {
                Name = name;
                BigMob = bigMob;
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
            if (JungleCamps.ContainsKey("Blue_RedBuff")) return;

            JungleCamps.Add("Blue_RedBuff",
                new Camp(115, 300, SummonersRift.Jungle.Blue_BlueBuff, new List<Monster>(new[]
                {
                    new Monster("SRU_Blue1.1.1", true),
                    new Monster("SRU_BlueMini1.1.2"),
                    new Monster("SRU_BlueMini21.1.3")
                })));


            JungleCamps.Add("Blue_BlueBuff",
                new Camp(115, 300, SummonersRift.Jungle.Blue_BlueBuff, new List<Monster>(new[]
                {
                    new Monster("SRU_Blue1.1.1", true),
                    new Monster("SRU_BlueMini1.1.2"),
                    new Monster("SRU_BlueMini21.1.3")
                })));

            //JungleCamps.Add("Blue_BlueBuff", new Monster(115, 300, SummonersRift.Jungle.Blue_BlueBuff));
            //JungleCamps.Add("Blue_Gromp", new Monster(115, 100, SummonersRift.Jungle.Blue_Gromp));
            //JungleCamps.Add("Blue_Krug", new Monster(115, 100, SummonersRift.Jungle.Blue_Krug));
            //JungleCamps.Add("Blue_RazorBeak", new Monster(115, 100, SummonersRift.Jungle.Blue_RazorBeak));
            //JungleCamps.Add("Blue_Murkwolf", new Monster(115, 100, SummonersRift.Jungle.Blue_Wolf));

            //JungleCamps.Add("Red_RedBuff", new Monster(115, 300, SummonersRift.Jungle.Red_RedBuff));
            //JungleCamps.Add("Red_RedBuff", new Monster(115, 300, SummonersRift.Jungle.Red_RedBuff));
            //JungleCamps.Add("Red_Gromp", new Monster(115, 100, SummonersRift.Jungle.Red_Gromp));
            //JungleCamps.Add("Red_Krug", new Monster(115, 100, SummonersRift.Jungle.Red_Krug));
            //JungleCamps.Add("Red_RazorBeak", new Monster(115, 100, SummonersRift.Jungle.Red_RazorBeak));
            //JungleCamps.Add("Red_Murkwolf", new Monster(115, 100, SummonersRift.Jungle.Red_Wolf));

            //JungleCamps.Add("Neutral_Baron", new Monster(115, 420, SummonersRift.River.Baron));
            //JungleCamps.Add("Neutral_Dragon", new Monster(115, 360, SummonersRift.River.Dragon));
        }
    }
}
