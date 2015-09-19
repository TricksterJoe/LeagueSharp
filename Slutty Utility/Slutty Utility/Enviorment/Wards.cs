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
    class Wards : Helper
    {
        public int Timers;
        public string ObjectName;
        public int Range;
        public string SpellName;

        public Wards()
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }


        public static readonly List<Wards> WardList = new List<Wards>();
        private static void OnLoad(EventArgs args)
        {
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            throw new NotImplementedException();
        }

        private static void WardDataBase()
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


        private static void OnCreate(GameObject sender, EventArgs args)
        {
            WardList.Add(
                new Wards
                {
                    Timers = 1337,
                    ObjectName = "a ward",
                }
                );

            if (!(sender is Obj_AI_Base))
                return;
            foreach (var wards in WardList)
            {
                switch (sender.Name)
                {

                }
            }
            switch (sender.Name == WardList.ToList().ToString())
            { }
        }
    }
}
