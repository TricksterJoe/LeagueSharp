using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace Lee_Sin.WardManager
{
   
    class WardSorter : LeeSin
    {
        //note: i know it's not poachers, will be changed in a few minutes.
        //Poachers id will get changed.
        public static int WardingTotem,
            SightStone,
            RubySightStone,
            VisionWard,
            Poachers,
            Poachers1,
            Poachers2,
            Poachers3,
            Poachers4;

        static WardSorter()
        {
            WardingTotem = (int) ItemId.Warding_Totem_Trinket;
            SightStone = (int) ItemId.Sightstone;
            RubySightStone = (int)ItemId.Ruby_Sightstone;
            VisionWard = (int) ItemId.Vision_Ward;
            Poachers = 3711;
            Poachers1 = 1408;
            Poachers2 = 1409;
            Poachers3 = 1410;
            Poachers4 = 1411;
        }

        public static bool HasPoachers()
        {
            return (HasItem(Poachers) && ItemReady(Poachers)) || (HasItem(Poachers1) && ItemReady(Poachers1)) ||
                   (HasItem(Poachers2) && ItemReady(Poachers2)) || (HasItem(Poachers3) && ItemReady(Poachers3)) ||
                   (HasItem(Poachers4) && ItemReady(Poachers4)); 
                   

        }
        public static Items.Item Wards()
        {
            if (HasPoachers())
            {
                return HasItem(Poachers)
                    ? new Items.Item(Poachers)
                    : HasItem(Poachers1)
                        ? new Items.Item(Poachers1)
                        : HasItem(Poachers2)
                            ? new Items.Item(Poachers2)
                            : HasItem(Poachers3) ? new Items.Item(Poachers3) : new Items.Item(Poachers4);
            }

            if (((HasItem(SightStone) && ItemReady(SightStone)) ||
                 (HasItem(RubySightStone) && ItemReady(RubySightStone))) && !HasPoachers()) 
            {
                return HasItem(SightStone) ? new Items.Item(SightStone) : new Items.Item(RubySightStone);
            }

            if (HasItem(WardingTotem) && ItemReady(WardingTotem) && !HasPoachers() &&
                (!HasItem(SightStone) || !ItemReady(SightStone)) &&
                (!HasItem(RubySightStone) || !ItemReady(RubySightStone))) 
            {
                return new Items.Item(WardingTotem);
            }

            if ((!HasItem(WardingTotem) || !ItemReady(WardingTotem)) && (!HasItem(SightStone) || !ItemReady(SightStone)) &&
                (!HasItem(RubySightStone) | !ItemReady(RubySightStone)) && !HasPoachers() &&
                HasItem(VisionWard)) 
            {
                return new Items.Item(VisionWard);
            }

            return null;
        }
    }
}
