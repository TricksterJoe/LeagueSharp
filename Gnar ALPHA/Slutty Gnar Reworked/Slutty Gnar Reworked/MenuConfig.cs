using LeagueSharp.Common;

namespace Slutty_Gnar_Reworked
{
    internal class MenuConfig
    {
        public static Menu Config;
        public static Orbwalking.Orbwalker Orbwalker;
        public const string Menuname = "Slutty Gnar";

        public static void CreateMenu()
        {
            #region general stuff
            Config = new Menu(Menuname, Menuname, true);
            Config.AddSubMenu(new Menu("Orbwalking", "Orbwalking"));
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(targetSelectorMenu);
            Config.AddSubMenu(targetSelectorMenu);

            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalking"));
            #endregion

            #region Drawings
            var draws = new Menu("Drawings", "Drawings");
            {
                draws.AddItem(new MenuItem("Draw", "Display Drawings").SetValue(true));
                draws.AddItem(new MenuItem("qDraw", "Q Drawing").SetValue(true));
                draws.AddItem(new MenuItem("wDraw", "W Drawing").SetValue(true));
                draws.AddItem(new MenuItem("eDraw", "E Drawing").SetValue(true));
            }
            Config.AddSubMenu(draws);
            #endregion

            #region Combo Menu
            var comboMenu = new Menu("Combo Settings", "combospells");
            {
                var minignar = new Menu("Mini Gnar", "Mini Gnar");
                {

                    minignar.AddItem(new MenuItem("UseQMini", "Use Q").SetValue(true));
                    minignar.AddItem(new MenuItem("UseQs", "Only Q When Target has 2 W Stacks").SetValue(true));
                    minignar.AddItem(new MenuItem("eGap", "Use E Gap Close When Killable").SetValue(true));
                    minignar.AddItem(new MenuItem("focust", "Focus Target with 2 W Stacks").SetValue(true));
                    comboMenu.AddSubMenu(minignar);
                }

                var megagnar = new Menu("Mega Gnar", "Mega Gnar");
                {

                    megagnar.AddItem(new MenuItem("UseQMega", "Use Q").SetValue(true));
                    megagnar.AddItem(new MenuItem("UseEMega", "Use E").SetValue(true));
                    megagnar.AddItem(new MenuItem("UseEMini", "Use E Only when about to transform").SetValue(true));
                    megagnar.AddItem(new MenuItem("UseWMega", "Use W").SetValue(true));
                    megagnar.AddItem(new MenuItem("UseRMega", "Use R").SetValue(true));
                    megagnar.AddItem(
                        new MenuItem("useRSlider", "Min targets R").SetValue(new Slider(3, 1, 5)));
                    comboMenu.AddSubMenu(megagnar);
                }

                Config.AddSubMenu(comboMenu);
            }
            #endregion

            #region Clear

            var clearMenu = new Menu("Clear Settings", "Clear Settings");
            {
                clearMenu.AddItem(new MenuItem("transform", "Use Spells When About To Transform").SetValue(true));
                var lanemenu = new Menu("Lane Clear", "Lane Clear");
                {
                    lanemenu.AddItem(new MenuItem("UseQl", "Use Q").SetValue(true));
                    lanemenu.AddItem(new MenuItem("UseQlslider", "Use Q Only If Hits X Minions").SetValue(new Slider(3, 1, 10)));
                    lanemenu.AddItem(new MenuItem("UseWl", "Use W").SetValue(true));
                    lanemenu.AddItem(new MenuItem("UseWlslider", "Use W Only If Hits X Minions").SetValue(new Slider(3, 1, 10)));
                    clearMenu.AddSubMenu(lanemenu);
                }

                var junglemenu = new Menu("Jungle Clear", "Jungle Clear");
                {
                    junglemenu.AddItem(new MenuItem("UseQj", "Use Q").SetValue(true));
                    junglemenu.AddItem(new MenuItem("UseWj", "Use W").SetValue(true));
                    clearMenu.AddSubMenu(junglemenu);
                }
                Config.AddSubMenu(clearMenu);
            }
            #endregion

            var mixed = new Menu("Harras", "Harras");
            {
                mixed.AddItem(new MenuItem("qharras", "Use Q").SetValue(true));
                mixed.AddItem(new MenuItem("qharras2", "Only Q If 2 Stacks").SetValue(true));
                mixed.AddItem(new MenuItem("wharras", "Use R").SetValue(true));
                mixed.AddItem(new MenuItem("autoq", "Auto Q").SetValue(false));
            }
            Config.AddSubMenu(mixed);

            #region Kill Steal
            var killsteal = new Menu("Kill Steal", "Kill Steal");
            {
                killsteal.AddItem(new MenuItem("qks", "Use Q").SetValue(true));
                killsteal.AddItem(new MenuItem("rks", "Use R").SetValue(true));
                killsteal.AddItem(new MenuItem("qeks", "Use E Gap Close + Q").SetValue(true));
            }

            Config.AddSubMenu(killsteal);
            #endregion

            var misc = new Menu("Misc", "Misc");
            {
                misc.AddItem(new MenuItem("qgap", "Q On Gap Closer").SetValue(true));
                misc.AddItem(new MenuItem("qwd", "Q/W On Dash").SetValue(true));
                misc.AddItem(new MenuItem("qwi", "W On Interruptable").SetValue(true));
            }

            Config.AddSubMenu(misc);
            
            #region Flee Key
            Config.AddItem(new MenuItem("fleekey", "Use Flee Mode")).SetValue(new KeyBind(65, KeyBindType.Press));
            #endregion
             
            Config.AddToMainMenu();
        }
    }
}