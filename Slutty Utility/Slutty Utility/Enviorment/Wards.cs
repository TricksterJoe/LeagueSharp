using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Utility.Enviorment
{
    class Wards
    {
<<<<<<< HEAD
        public int Timers;
        public string ObjectName;
        public int Range;
        public string SpellName;
        public int Time;

        /*
        internal struct Ward
        {
            public Vector2 location;
            public float lifeSpan;
            public char type;
        }
        public static readonly List<Ward> WardsOnMap = new List<Ward>();
        // oncreate
        public void onCreate()
        {
            var nWard = new Ward();
            WardsOnMap.Add(nWard);
            {
            nWard.lifeSpan = Environment.TickCount + 60000;
            nWard.type = 'p';
            }          
        }
         */
=======

        public struct Ward
        {
            public Vector2 location;
            public float lifeSpan;
            public float range;
            public int id; //0 = trinket,ect
        }
>>>>>>> origin/master

        public Wards()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
            
        }


        public static List<Ward> WardList = new List<Ward>();
        private static void OnLoad(EventArgs args)
        {
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            throw new NotImplementedException();
        }
<<<<<<< HEAD
       public struct Ward
        {
            public Vector3 location;
            public float lifeSpan;
            public char type;
        }
        List<Ward> WardsOnMap = new List<Ward>();
        // oncreate
        public void onCreate()
        {
            var nWard = new Ward
            {
                lifeSpan = Environment.TickCount + 60000,
                type = 'p'
                
            };
            WardsOnMap.Add(new Ward());
        }
        public static void WardDataBase()
        {
            //Trinkets:
            WardList.Add(
            new Wards
            {
                Timers = 1 * 60 * 1000,
                ObjectName = "YellowTrinket",
                Range = 1100,
                SpellName = "TrinketTotemLvl1",
            });

            WardList.Add(
            new Wards
            {
                Timers = 2 * 60 * 1000,
                ObjectName = "YellowTrinketUpgrade",
                Range = 1100,
                SpellName = "TrinketTotemLvl2",
            });

            WardList.Add(
            new Wards
            {
                Timers = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "TrinketTotemLvl3",
            });

            //Ward items and normal wards:
            WardList.Add(
            new Wards
            {
                Timers = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "SightWard",
            });

            WardList.Add(
            new Wards
            {
                Timers = 3 * 60 * 1000,
                ObjectName = "SightWard",
                Range = 1100,
                SpellName = "ItemGhostWard",
            });
        }
=======

>>>>>>> origin/master


        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var oWard = new Ward
            {
                location = new Vector2(1, 1), lifeSpan = Time.TickCount + 60000, range = 1000, id = 1
            };
            WardList.Add(oWard);

<<<<<<< HEAD
                }
            }
            switch (sender.Name == Ward.)
            { }
=======
>>>>>>> origin/master
        }
    }
}
