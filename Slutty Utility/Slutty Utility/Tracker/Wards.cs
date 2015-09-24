using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace Slutty_Utility.Tracker
{
    internal class Wards : Helper
    {
        //TY Kallen for coding 90% of this
        // I love you <3
        //Please no coperino

        //Ward Spell names by Anderluis from Tracker# thank you
        public struct PlacedWard
        {
            public Ward BaseWard;
            public Vector3 Location;
            public float DeathTime;

            public PlacedWard(Ward ward, Vector3 location, float deathTime)
            {
                BaseWard = ward;
                Location = location;
                DeathTime = deathTime;
            }
        }

        public struct Ward
        {
            public float LifeSpan;
            public float Range;
            public bool IsPink;
            public Ward(float life, float range, bool isPink = false)
            {
                LifeSpan = life;
                Range = range;
                IsPink = isPink;
            }
        }


        public static Dictionary<string, Ward> WardStructure = new Dictionary<string, Ward>();
        public static List<PlacedWard> ActiveWards = new List<PlacedWard>();
        public static Dictionary<string, Ward> SpellWards = new Dictionary<string, Ward>();

        private static void LoadWardData()
        {
            if (SpellWards.Count < 1)
                LoadSpellData();

            if (WardStructure.Count > 1) return;
            WardStructure.Add("YellowTrinket", new Ward(60, 100));
            WardStructure.Add("YellowTrinketUpgrade", new Ward(120, 100));
            WardStructure.Add("VisionWard", new Ward(float.MaxValue, 100, true));
            WardStructure.Add("SightWard", new Ward(180, 100));
            WardStructure.Add("itemplacementmissile", new Ward(180, 100));
        }

        private static void LoadSpellData()
        {
            if (WardStructure.Count < 1) return; //Ward data needs to be loaded first
            if (SpellWards.Count > 1) return;

            SpellWards.Add("TrinketTotemLvl1", WardStructure["YellowTrinket"]);
            SpellWards.Add("TrinketTotemLvl2", WardStructure["YellowTrinketUpgrade"]);
            SpellWards.Add("TrinketTotemLvl3", WardStructure["SightWard"]);

            SpellWards.Add("SightWard", WardStructure["SightWard"]);
            SpellWards.Add("ItemGhostWard", WardStructure["SightWard"]);
            SpellWards.Add("wrigglelantern", WardStructure["SightWard"]);
            SpellWards.Add("ItemFeralFlare", WardStructure["SightWard"]);

            SpellWards.Add("TrinketTotemLvl3B", WardStructure["VisionWard"]);
            SpellWards.Add("VisionWard", WardStructure["VisionWard"]);
        }

        public static void OnLoad()
        {
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCsreate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDeletes;
            Obj_AI_Base.OnProcessSpellCast += OnSpell;
            Drawing.OnDraw += OnDraws;
        }


        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is MissileClient))
                return;


            var missile = (MissileClient)sender;

            if (missile.SpellCaster.IsAlly) return;
            if (missile.SData.Name != "itemplacementmissile" || missile.SpellCaster.IsVisible) return;

            var sPos = missile.StartPosition;
            if (!WardStructure.ContainsKey(missile.SData.Name)) return;
            ActiveWards.Add(new PlacedWard(WardStructure[missile.SData.Name], new Vector3(sPos.X, sPos.Y, NavMesh.GetHeightForPosition(sPos.X, sPos.Y)),
                Game.Time + WardStructure[missile.SData.Name].LifeSpan));
        }

        private static void OnDraws(EventArgs args)
        {
            LoadWardData();

            if (!GetBool("enviorment.wards", typeof(bool))) return;

            foreach (var ward in ActiveWards)
            {
                var rangecolor = ward.BaseWard.IsPink ? Color.Magenta : Color.Green;
                var timercolors = ward.BaseWard.LifeSpan > Game.Time + 181 ? Color.Red : Color.Black;
                var time = ward.BaseWard.LifeSpan > Game.Time + 181 ? "Pink" : ((int) ward.DeathTime - (int)Game.Time).ToString(CultureInfo.InvariantCulture);

                Render.Circle.DrawCircle(ward.Location, ward.BaseWard.Range, rangecolor);

                Drawing.DrawText(Drawing.WorldToScreen(ward.Location).X, Drawing.WorldToScreen(ward.Location).Y,
                    timercolors, time);


            }
        }

        private static void OnSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsAlly || sender.IsMe)
                return;

            if (!SpellWards.ContainsKey(args.SData.Name)) return;

              var endPosition = ObjectManager.Player.GetPath(args.End).ToList().Last();
            ActiveWards.Add(new PlacedWard(SpellWards[args.SData.Name], endPosition,
                Game.Time + SpellWards[args.SData.Name].LifeSpan));
        }


        private static void OnDeletes(GameObject sender, EventArgs args)
        {
            LoadWardData();
            if (!(sender is Obj_AI_Base))
                return;
            foreach (var ward in ActiveWards)
            {
                if (WardStructure.ContainsKey(sender.Name) && ward.Location == sender.Position)
                {
                    ActiveWards.Remove(ward);                  
                }
            }

            foreach (var ward in ActiveWards)
            {
                if (SpellWards.ContainsKey(sender.Name) && ward.Location == sender.Position)
                {
                    ActiveWards.Remove(ward);
                }
            }

        }

        private static void OnCsreate(GameObject sender, EventArgs args)
        {
            LoadWardData();
            if ((!(sender is Obj_AI_Base))) return;
            if (sender.IsAlly)
                return;

            if (!WardStructure.ContainsKey(sender.Name)) return;
            ActiveWards.Add(new PlacedWard(WardStructure[sender.Name], sender.Position,
                Game.Time + WardStructure[sender.Name].LifeSpan));
        }



        private static void OnUpdate(EventArgs args)
        {
            ActiveWards.FindAll(ward => ward.DeathTime < Game.Time).ForEach(ward => ActiveWards.Remove(ward));
        }

    }
}