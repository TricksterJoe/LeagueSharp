using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace Slutty_Veigar
{
    internal class Helper
    {
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Hero Player { get { return ObjectManager.Player; } }
        public static Menu Config;
        private static readonly DateTime AssemblyLoadTime = DateTime.Now;
        public const string Menuname = "Slutty Veigar";

        public static void AddBool(Menu menu, string displayName, string name, bool value = true)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(value));
        }

        public static float TickCount
        {
            get { return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds; }
        }

        public static void AddValue(Menu menu, string displayName, string name, int startVal, int minVal = 0, int maxVal = 100)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(new Slider(startVal, minVal, maxVal)));
        }

        public static void AddKeyBind(Menu menu, string displayName, string name, char key, KeyBindType type)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(new KeyBind(key, type)));
        }

        public static bool GetBool(string name, Type oType)
        {
            return oType == typeof(KeyBind) ? Config.Item(name).GetValue<KeyBind>().Active : Config.Item(name).GetValue<bool>();
        }

        public static int GetValue(string name)
        {
            return Config.Item(name).GetValue<Slider>().Value;
        }

        public static int GetStringValue(string name)
        {
            return Config.Item(name).GetValue<StringList>().SelectedIndex;
        }

        public static Menu Activator()
        {
            return new Menu("Activator", "Activator");
        }

        public static bool ManaCheck(string name)
        {
            return Player.ManaPercent <= Config.Item(name).GetValue<Slider>().Value;
        }

        public static bool ItemReady(int id)
        {
            return Items.CanUseItem(id);
        }

        public static bool HasItem(int id)
        {
            return Items.HasItem(id);
        }
        /* // add 
        public bool BookCast(SpellSlot spell, GameObject name)           
        {
           
        }
         */

        public static bool HealthCheck(string name)
        {
            return Player.HealthPercent <= Config.Item(name).GetValue<Slider>().Value;
        }

        public static bool UseUnitItem(int item, Obj_AI_Hero target)
        {
            return Items.UseItem(item, target);
        }

        public static bool SelfCast(int item)
        {
            return Items.UseItem(item);
        }

        public static bool PlayerBuff(string name)
        {
            return Player.HasBuff(name);
        }

        public static void PotionCast(int id, string buff)
        {
            if (ItemReady(id) && !PlayerBuff(buff)
                && !Player.IsRecalling() && !Player.InFountain()
                && Player.CountEnemiesInRange(700) >= 1)
            {
                SelfCast(id);
            }
        }

        public static void ElixerCast(int id, string buff)
        {
            if (!PlayerBuff(buff)
                && HasItem(id))
            {
                SelfCast(id);
            }
        }

        public static float SpellRange(SpellSlot spellSlot)
        {
            return Player.Spellbook.GetSpell(spellSlot).SData.CastRange;
        }
    }
}
