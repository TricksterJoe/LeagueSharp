using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;

namespace ODarius
{
    internal class MenuConfig 
    {
        public static Obj_AI_Hero Player = ObjectManager.Player;
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public const string Menuname = "ODarius";

        public static void LoadMenu()
        {
            #region Orbwalker/target selector

            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));

            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));

            #endregion

            #region Drawings

            var drawMenu = new Menu("Drawing Settings", "Drawings");
            {
                drawMenu.AddItem(new MenuItem("Draw", "Display Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("rDraw", "R Drawing").SetValue(true));
                drawMenu.AddItem(new MenuItem("FillDamage", "Show Combo Damage").SetValue(true));
                drawMenu.AddItem(
                    new MenuItem("RushDrawWDamageFill", "Combo Damage Fill").SetValue(true));
                Config.AddSubMenu(drawMenu);
            }

            #endregion

            #region Combo Settings

            var comboMenu = new Menu("Combo Settings", "Combo Settings");
            {
                comboMenu.AddItem(new MenuItem("comboMenu.useq",
                    " Use Q " + "[" + Player.Spellbook.GetSpell(SpellSlot.Q).Name + "]")).SetValue(true);

                comboMenu.AddItem(new MenuItem("comboMenu.usew",
                    " Use W " + "[" + Player.Spellbook.GetSpell(SpellSlot.W).Name + "]")).SetValue(true);

                comboMenu.AddItem(new MenuItem("comboMenu.usee",
                    " Use E " + "[" + Player.Spellbook.GetSpell(SpellSlot.E).Name + "]")).SetValue(true);

                comboMenu.AddItem(
                    new MenuItem("comboMenu.user", "R Combo Mode").SetValue(
                        new StringList(new[] { "Always R", "R Kill Steal"}, 1)));
                Config.AddSubMenu(comboMenu);
            }

            #endregion

            #region Clear Settings

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                var laneMenu = new Menu("Lane Clear", "Lane Clear");
                {
                    laneMenu.AddItem(new MenuItem("laneMenu.manaslider", "Min %Mana").SetValue(new Slider(45)));
                    laneMenu.AddItem(new MenuItem("laneMenu.useq", "Use Q")).SetValue(true);
                    laneMenu.AddItem(new MenuItem("laneMenu.minminions", "Min Minions For Q").SetValue(new Slider(3, 1, 20)));
                    laneMenu.AddItem(new MenuItem("laneMenu.usew", "Use W")).SetValue(true);
                }

                var jungleMenu = new Menu("Jungle Clear", "Jungle Clear");
                {
                    jungleMenu.AddItem(new MenuItem("jungleMenu.manaslider", "Min %Mana").SetValue(new Slider(30)));
                    jungleMenu.AddItem(new MenuItem("jungleMenu.useq", "Use Q")).SetValue(true);
                    jungleMenu.AddItem(new MenuItem("jungleMenu.usew", "Use W")).SetValue(true);
                }

                clearMenu.AddSubMenu(jungleMenu);
                clearMenu.AddSubMenu(laneMenu);
                Config.AddSubMenu(clearMenu);
            }

            #endregion
            /*
            #region Harras Settings

            var harrasMenu = new Menu("Harras Settings", "Harras Settings");
            {
                harrasMenu.AddItem(new MenuItem("harrasMenu.useq", "Use Q").SetValue(true));
                harrasMenu.AddItem(new MenuItem("harrasMenu.usew", "Use W").SetValue(true));
                harrasMenu.AddItem(new MenuItem("harrasMenu.usee", "Use E").SetValue(true));
                Config.AddSubMenu(harrasMenu);
            }

            #endregion
             */

            Config.AddToMainMenu();
        }
}
}
