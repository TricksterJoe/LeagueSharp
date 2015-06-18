using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using SharpDX;
using Point = SharpDX.Point;


namespace Ult_Notifyer
{

    internal class Program
    {
        public const string Menuname = "Ult Notifyer";
        public static Menu Config;
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Items.Item BiscuitofRejuvenation = new Items.Item(2010);

        public delegate void OnProcessSpecialSpellHandler(Obj_AI_Base enemy, GameObjectProcessSpellCastEventArgs args,
            SpellData spellData);

        public static Dictionary<string, string> channeledSpells = new Dictionary<string, string>();

        private static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            Config = new Menu(Menuname, Menuname, true);
            Config.AddItem(new MenuItem("Enabled", "Enabled").SetValue(true));

            Config.AddItem(new MenuItem("Language", "Language"))
                    .SetValue(new StringList(new[] { "English", "German" }));
            Config.AddToMainMenu();
            Game.OnUpdate += Game_OnUpdate;
            Obj_AI_Hero.OnProcessSpellCast += Game_ProcessSpell;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            // Console.WriteLine(Player.Position);
        }




        private static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            Console.WriteLine(args.SData.Name);
            if (!Config.Item("Enabled").GetValue<bool>())
                return;
            // botlane
            Vector2 point1 = new Vector2(5758, 1230);
            Vector2 point2 = new Vector2(6258, 1230);
            Vector2 point3 = new Vector2(6758, 1230);
            Vector2 point4 = new Vector2(7258, 1230);
            Vector2 point5 = new Vector2(7758, 1230);
            Vector2 point6 = new Vector2(8258, 1230);
            Vector2 point7 = new Vector2(8758, 1230);
            Vector2 point8 = new Vector2(9258, 1230);
            Vector2 point9 = new Vector2(9758, 1230);
            Vector2 point10 = new Vector2(10258, 1230);
            Vector2 point11 = new Vector2(10758, 1230);
            Vector2 point12 = new Vector2(11858, 1230);
            Vector2 point13 = new Vector2(12616, 1754);
            Vector2 point14 = new Vector2(13416, 1754);
            Vector2 point15 = new Vector2(14216, 6958);
            Vector2 point16 = new Vector2(15016, 6958);
            Vector2 point17 = new Vector2(15816, 6958);
            Vector2 point18 = new Vector2(16616, 6958);
            Vector2 point19 = new Vector2(17416, 6958);
            Vector2 point20 = new Vector2(18216, 6958);
            Vector2 point21 = new Vector2(19016, 6958);
            Vector2 point22 = new Vector2(19816, 6958);
            Vector2 point23 = new Vector2(20616, 6958);
            Vector2 point24 = new Vector2(13416, 5400);
            Vector2 point25 = new Vector2(13482, 3586);

            // toplane
            Vector2 pointtop1 = new Vector2(1212, 5866);
            Vector2 pointtop2 = new Vector2(1212, 6666);
            Vector2 pointtop3 = new Vector2(1212, 7466);
            Vector2 pointtop4 = new Vector2(1212, 8266);
            Vector2 pointtop5 = new Vector2(1212, 9066);
            Vector2 pointtop6 = new Vector2(1212, 9866);
            Vector2 pointtop7 = new Vector2(1212, 10666);
            Vector2 pointtop8 = new Vector2(1212, 11466);
            Vector2 pointtop9 = new Vector2(1212, 12266);
            Vector2 pointtop10 = new Vector2(1212, 13066);
            Vector2 pointtop11 = new Vector2(1212, 13866);

            // midelane
            Vector2 pointmid1 = new Vector2(4000, 4000);
            Vector2 pointmid2 = new Vector2(4800, 4800);
            Vector2 pointmid3 = new Vector2(5600, 5600);
            Vector2 pointmid4 = new Vector2(6400, 6400);
            Vector2 pointmid5 = new Vector2(7200, 7200);
            Vector2 pointmid6 = new Vector2(8000, 8000);
            Vector2 pointmid7 = new Vector2(8800, 8800);
            Vector2 pointmid8 = new Vector2(9600, 9600);
            Vector2 pointmid9 = new Vector2(10400, 10400);
            Vector2 pointmid10 = new Vector2(1212, 5866);



            channeledSpells["DravenRCast"] = "Draven";
            channeledSpells["JinxR"] = "Jinx";
            channeledSpells["EzrealTrueshotBarrage"] = "Ezreal";
            channeledSpells["EnchantedCrystalArrow"] = "Ashe";
            // ItemMiniRegenPotion
            string name;
            if (channeledSpells.TryGetValue(args.SData.Name, out name)
                && hero.Spellbook.IsCastingSpell)
            {
                if (!hero.IsMe)
                {
                    if ((hero.Distance(point1) <= 1500
                         || hero.Distance(point2) <= 1500
                         || hero.Distance(point3) <= 1500
                         || hero.Distance(point4) <= 1500
                         || hero.Distance(point5) <= 1500
                         || hero.Distance(point6) <= 1500
                         || hero.Distance(point7) <= 1500
                         || hero.Distance(point8) <= 1500
                         || hero.Distance(point9) <= 1500
                         || hero.Distance(point10) <= 1500
                         || hero.Distance(point11) <= 1500
                         || hero.Distance(point12) <= 1500
                         || hero.Distance(point13) <= 1500
                         || hero.Distance(point14) <= 1500
                         || hero.Distance(point15) <= 1500
                         || hero.Distance(point16) <= 1500
                         || hero.Distance(point17) <= 1500
                         || hero.Distance(point18) <= 1500
                         || hero.Distance(point19) <= 1500
                         || hero.Distance(point20) <= 1500
                         || hero.Distance(point21) <= 1500
                         || hero.Distance(point22) <= 1500
                         || hero.Distance(point23) <= 1500
                         || hero.Distance(point24) <= 1500
                         || hero.Distance(point25) <= 1500))
                    {
                        switch ((Config.Item("Language").GetValue<StringList>().SelectedIndex))
                        {
                            case 0:
                            {
                                Game.Say(name + " Has Just Casted Ultimate From Bot Lane! Care!");
                                break;
                            }
                            case 1:
                            {
                                Game.Say(name + " Hat gerade seine Ultimate von der Bot Lane gecastet! Vorsicht!");
                                break;
                            }
                        }
                    }
                    if ((hero.Distance(pointtop1) <= 1500
                         || hero.Distance(pointtop2) <= 1500
                         || hero.Distance(pointtop3) <= 1500
                         || hero.Distance(pointtop4) <= 1500
                         || hero.Distance(pointtop5) <= 1500
                         || hero.Distance(pointtop6) <= 1500
                         || hero.Distance(pointtop7) <= 1500
                         || hero.Distance(pointtop8) <= 1500
                         || hero.Distance(pointtop9) <= 1500
                         || hero.Distance(pointtop10) <= 1500
                         || hero.Distance(pointtop11) <= 1500))
                    {
                        switch ((Config.Item("Language").GetValue<StringList>().SelectedIndex))
                        {
                            case 0:
                            {
                                Game.Say(name + " Has Just Casted Ultimate From Top Lane! Care!");
                                break;
                            }
                            case 1:
                            {
                                Game.Say(name + " Hat gerade seine Ultimate von der Top Lane gecastet! Vorsicht!");
                                break;
                            }
                        }
                    }
                    if (hero.Distance(pointmid1) <= 800
                        || hero.Distance(pointmid2) <= 800
                        || hero.Distance(pointmid3) <= 800
                        || hero.Distance(pointmid4) <= 800
                        || hero.Distance(pointmid5) <= 800
                        || hero.Distance(pointmid6) <= 800
                        || hero.Distance(pointmid7) <= 800
                        || hero.Distance(pointmid8) <= 800
                        || hero.Distance(pointmid9) <= 800
                        || hero.Distance(pointmid10) <= 800)
                    {
                        switch ((Config.Item("Language").GetValue<StringList>().SelectedIndex))
                        {
                            case 0:
                                {
                                    Game.Say(name + " Has Just Casted Ultimate From Mid Lane! Care!");
                                    break;
                                }
                            case 1:
                                {
                                    Game.Say(name + " Hat gerade seine Ultimate von der Mid Lane gecastet! Vorsicht!");
                                    break;
                                }
                        }
                    }
                }
            }
        }
    }
}
 