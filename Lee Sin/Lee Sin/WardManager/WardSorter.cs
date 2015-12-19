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
            Poachers1 = 3711;
            Poachers2 = 3711;
            Poachers3 = 3711;
            Poachers4 = 3711;
        }

        public static bool HasPoachers()
        {
            return HasItem(Poachers) || HasItem(Poachers1) || HasItem(Poachers2) || HasItem(Poachers3) ||
                   HasItem(Poachers4);
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

            if ((HasItem(SightStone) || HasItem(RubySightStone)) && !HasPoachers())
            {
                return HasItem(SightStone) ? new Items.Item(SightStone) : new Items.Item(RubySightStone);
            }

            if (HasItem(WardingTotem) && !HasPoachers() && !HasItem(SightStone) && !HasItem(RubySightStone))
            {
                return new Items.Item(WardingTotem);
            }

            if (!HasItem(WardingTotem) && !HasItem(SightStone) && !HasItem(RubySightStone) && !HasPoachers() &&
                HasItem(VisionWard))
            {
                return new Items.Item(VisionWard);
            }

            return null;
        }
    }
}
