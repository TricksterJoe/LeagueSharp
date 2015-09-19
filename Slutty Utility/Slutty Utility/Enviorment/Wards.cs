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

        public struct Ward
        {
            public Vector2 location;
            public float lifeSpan;
            public float range;
            public int id; //0 = trinket,ect
        }

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



        private static void OnCreate(GameObject sender, EventArgs args)
        {
            var oWard = new Ward
            {
                location = new Vector2(1, 1), lifeSpan = Time.TickCount + 60000, range = 1000, id = 1
            };
            WardList.Add(oWard);

        }
    }
}
