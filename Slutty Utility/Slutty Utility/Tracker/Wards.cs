using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Slutty_Utility.Tracker
{
    internal class Wards : Helper
    {
        public static GameObject greenward;
        internal enum WardType
        {
            Green,
            Pink
        }

        public struct Ward
        {
            public Vector3 location;
            public float lifeSpan;
            public float range;
            public string wardID;
            public string Name;
            public WardType id; //0 = trinket,ect
        }

        public Wards()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }


        public static List<Ward> WardList = new List<Ward>();

        private static void OnLoad(EventArgs args)
        {
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreate;
        }

        private static void OnUpdate(EventArgs args)
        {
            WardList.FindAll(ward => ward.lifeSpan < TickCount).ForEach(ward => WardList.Remove(ward)); //change to ondelete
        }
        

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            #region Ward Types

            var yTrinket = new Ward
            {
                location = sender.Position,
                wardID = "YellowTrinket",
                Name = "TrinketTotemLvl1",
                lifeSpan = TickCount + 60000,
                range = 1100,
                id = WardType.Green
            };
            WardList.Add(yTrinket);

            var yTrinketUpgrade = new Ward
            {
                location = sender.Position,
                wardID = "YellowTrinketUpgrade",
                Name = "TrinketTotemLvl2",
                lifeSpan = TickCount + 120000,
                range = 1100,
                id = WardType.Green
            };
            WardList.Add(yTrinketUpgrade);

            var yTrinketUpgrade2 = new Ward
            {
                location = sender.Position,
                wardID = "SightWard",
                Name = "TrinketTotemLvl3",
                lifeSpan = TickCount + 180000,
                range = 1100,
                id = WardType.Green
            };
            WardList.Add(yTrinketUpgrade2);

            var sightWard = new Ward
            {
                location = sender.Position,
                wardID = "SightWard",
                Name = "SightWard",
                lifeSpan = TickCount + 180000,
                range = 1100,
                id = WardType.Green
            };
            WardList.Add(sightWard);

            var ghostWard = new Ward
            {
                location = sender.Position,
                wardID = "SightWard",
                Name = "ItemGhostWard",
                lifeSpan = TickCount + 180000,
                range = 1100,
                id = WardType.Green
            };
            WardList.Add(ghostWard);

            var wrigglelantern = new Ward
            {
                location = sender.Position,
                wardID = "SightWard",
                Name = "wrigglelantern",
                lifeSpan = TickCount + 180000,
                range = 1100,
                id = WardType.Green
            };
            WardList.Add(wrigglelantern);

            var trinketPink = new Ward
            {
                location = sender.Position,
                wardID = "VisionWard",
                Name = "TrinketTotemLvl3B",
                lifeSpan = int.MaxValue,
                range = 1100,
                id = WardType.Pink
            };
            WardList.Add(trinketPink);

            var VisionWard = new Ward
            {
                location = sender.Position,
                wardID = "VisionWard",
                Name = "VisionWard",
                lifeSpan = int.MaxValue,
                range = 1100,
                id = WardType.Pink
            };
            WardList.Add(VisionWard);

            #endregion


            if (!sender.IsValid<Obj_AI_Base>())
                return;
            if (sender.IsAlly)
                return;

            foreach (var wards in WardList)
            {
                var colors = wards.id == WardType.Green ? Color.Green : Color.Pink;
                if (wards.wardID == sender.Name)
                {
                    Drawing.DrawCircle(wards.location, 100, colors);
                }
            }

        }
    }
}