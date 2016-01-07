using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Anti_Rengar
{
    internal class AntiRengar
    {
        public static string[] Champlist =
{
            "Vayne", "Ahri", "Alistar", "Anivia", "Caitlyn", "Annie"
            , "Ashe", "Azir", "Bitzcrank", "Chogath", "Diana", "Draven",
            "Elise", "Ezreal", "Fiddlesticks", "Fizz", "Garen", "Gragas", "Irelia", "Janna"
            , "Jayce", "Jinx", "LeBlanc", "Leesin", "Leona", "Lulu", "Lux", "Nami"
            , "Quinn", "Riven", "Shaco", "Sivir", "Soraka", "Swain", "Syndra", "Thresh"
            , "Tristana", "Velkoz", "Viktor", "MonkeyKing", "Zyra", "Xerath"
        };

        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Obj_AI_Hero _rengo;
        private static int lastcasted;
        public static Menu Config;
        public const string Menuname = "Anti Rengar";
        public static void AddBool(Menu menu, string displayName, string name, bool value = true)
        {
            menu.AddItem(new MenuItem(name, displayName).SetValue(value));
        }

        public static void OnLoad(EventArgs args)
        {
         //   if (!Champlist.Contains(Player.ChampionName)) return;
            Config = new Menu(Menuname, Menuname, true);
            AddBool(Config, "Enable", "enable");
          

            // important on/off
            switch (Player.ChampionName)
            {
                case "Vayne":
                    AddBool(Config, "Use E", "usee");
                    AddBool(Config, "Use Q", "useq");
                    break;
                case "Ahri":
                    AddBool(Config, "Use R", "user");
                    break;
                case "Alistar":
                    AddBool(Config, "Use W", "usew");
                    break;

            }

            Config.AddToMainMenu();

            
            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
            GameObject.OnDelete += OnDeleteObject;
         
        }

        private static void OnDeleteObject(GameObject sender, EventArgs args)
        {
         //   if (sender.Name.ToLower().Contains("rengar"))
           // Game.PrintChat(sender.Name);

            if (sender.Name == "Rengar_LeapSound.troy")
            {
                Utility.DelayAction.Add(300, () => _rengo = null);
            }
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "randomnamehere")
            {
                if (args.Target.IsAlly || args.Target.IsMe)
                {
                    _target = (Obj_AI_Hero)args.Target;
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {
//
        //   Game.PrintChat(Player.Buffs.FirstOrDefault().Name);
            if (!Config.Item("enable").GetValue<bool>()) return;

            if (_rengo == null) return;
            if ((_rengo.IsDead) || Environment.TickCount - lastcasted > 8 * 10 * 10 * 10)
            {
                _rengo = null;
            }
            if (_rengo == null) return;
            var rengopos = _rengo.Position;
            var spellbook = Player.Spellbook;
            switch (Player.ChampionName.ToLower())
            {
                case "aatrox":
                    {
                        ReadyCast(400, SpellSlot.Q, BackWardsCast(500));
                    }
                    break;
                case "akali":
                    {
                        ReadyCast(1200, SpellSlot.W, Player.Position, false);
                    }
                    break;

                case "braum":
                    {
                        ReadyCast(1200, SpellSlot.R, default(Vector3), true);
                    }
                    break;

                case "cassiopeia":
                    {
                        ReadyCast(1200, SpellSlot.R, default(Vector3), true); 
                    }
                    break;

                case "corki":
                    {
                        ReadyCast(500, SpellSlot.W, BackWardsCast(600));
                    }
                    break;

                case "galio":
                    {
                        ReadyCast(1200, SpellSlot.R, default(Vector3), false);
                    }
                    break;

                case "graves":
                    {
                        ReadyCast(500, SpellSlot.E, BackWardsCast(500));
                    }
                    break;

                case "hecarim":
                    {
                        ReadyCast(300, SpellSlot.E, default(Vector3), true);
                    }
                    break;

                case "vayne":
                    if (Config.Item("useq").GetValue<bool>())
                    ReadyCast(500, SpellSlot.Q, BackWardsCast(500));

                    if (!Ready(SpellSlot.Q) || Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                    {
                        if (Config.Item("usee").GetValue<bool>())
                            ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    }

                    if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed)
                    Player.IssueOrder(GameObjectOrder.AttackTo, _rengo);
                    break;

                case "ahri":
                    if (!Player.HasBuff("AhriTumble"))
                    {
                        if (Config.Item("user").GetValue<bool>())
                            ReadyCast(500, SpellSlot.R, BackWardsCast(430));
                    }
                    if (Player.HasBuff("AhriTumble") || !Player.Spellbook.GetSpell(SpellSlot.R).IsReady())
                   Utility.DelayAction.Add(200, () => ReadyCast(500, SpellSlot.E, default(Vector3)));
                    break;

                case "alistar":
                        SelfCast(1000, SpellSlot.Q);

                    if (!Player.Spellbook.GetSpell(SpellSlot.Q).IsReady())
                    {
                        if (Config.Item("usew").GetValue<bool>())
                            Utility.DelayAction.Add(200, () =>
                            ReadyCast(500, SpellSlot.W, default(Vector3), true));
                    }
                    break;

                case "caitlyn":
                    ReadyCast(500, SpellSlot.E, BackWardsCast2(430));
                    break;

                case "annie":
                    if (Player.HasBuff("pyromaniaparticle"))
                        ReadyCast(500, SpellSlot.Q, default(Vector3), true);
                    break;

                case "ashe":
                    Utility.DelayAction.Add(200, () => ReadyCast(1000, SpellSlot.R, default(Vector3)));
                    break;

                case "azir":
                    ReadyCast(700, SpellSlot.R, Player.Position.Extend(_rengo.Position, Player.BoundingRadius + 100));
                    break;

                case "blitzcrank":
                    SelfCast(500, SpellSlot.R);
                    break;

                case "chogath":
                    ReadyCast(800, SpellSlot.Q, Player.Position);
                    ReadyCast(800, SpellSlot.W, default(Vector3), true);
                    break;

                case "diana":
                    SelfCast(500, SpellSlot.E);
                    break;

                case "draven":
                   Utility.DelayAction.Add(300, () => ReadyCast(600, SpellSlot.E, _rengo.Position));
                    break;

                case "elise":
                    Utility.DelayAction.Add(200, () => ReadyCast(500, SpellSlot.E, default(Vector3)));
                    break;

                case "ezreal":
                    ReadyCast(600, SpellSlot.E, BackWardsCast(500));
                    break;

                case "fiddlesticks":
                    ReadyCast(600, SpellSlot.E, default(Vector3), true);
                    ReadyCast(600, SpellSlot.Q, default(Vector3), true);
                    break;

                case "fizz":
                    ReadyCast(800, SpellSlot.E, BackWardsCast(500));
                    break;

                case "garen":
                        SelfCast(Player.AttackRange, SpellSlot.Q);
                    if (Player.Spellbook.GetSpell(SpellSlot.Q).State == SpellState.Surpressed &&
                        _rengo.Position.Distance(Player.Position) < Player.AttackRange + Player.BoundingRadius + _rengo.BoundingRadius  ) 
                    {
                        Player.IssueOrder(GameObjectOrder.AttackTo, _rengo);
                    }
                    break;

                case "gragas":
                    ReadyCast(700, SpellSlot.E, default(Vector3), true);
                    break;

                case "irelia":
                    ReadyCast(400, SpellSlot.E, default(Vector3), true);
                    break;

                case "janna":
                    ReadyCast(800, SpellSlot.Q, default(Vector3), true);
                    break;

                case "jayce":
                    ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    break;

                case "jinx":
                    ReadyCast(500, SpellSlot.E, Player.Position);
                    break;

                case "leblanc":
                    ReadyCast(300, SpellSlot.E, default(Vector3), true);
                    break;

                case "lulu":
                    ReadyCast(500, SpellSlot.W, default(Vector3), true);
                    break;

                case "lux":
                    ReadyCast(600, SpellSlot.Q, default(Vector3), true);
                    break;

                case "nami":
                   Utility.DelayAction.Add(300, () => ReadyCast(400, SpellSlot.Q, rengopos));
                    break;

                case "quinn":
                    ReadyCast(500, SpellSlot.Q, _rengo.Position);
                    ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    break;

                case "riven":
                    SelfCast(1000, SpellSlot.W);
                    break;

                case "shaco":
                    ReadyCast(Player.AttackRange, SpellSlot.Q, BackWardsCast(500));
                    break;

                case "soraka":
                    Utility.DelayAction.Add(200, () => ReadyCast(1000, SpellSlot.E, _rengo.Position));
                    break;

                case "swain":
                    Utility.DelayAction.Add(200, () => ReadyCast(1000, SpellSlot.E, _rengo.Position));
                    break;

                case "thresh":
                    Utility.DelayAction.Add(200, () => ReadyCast(1000, SpellSlot.E, _rengo.Position));
                    break;

                case "tristana":
                    ReadyCast(500, SpellSlot.R, default(Vector3), true);
                    break;

                case "velkoz":
                    Utility.DelayAction.Add(200, () => ReadyCast(1000, SpellSlot.E, Player.Position));
                    break;

                case "viktor":
                    ReadyCast(1000, SpellSlot.W, Player.Position);
                    break;

                case "monkeyking":
                    SelfCast(400, SpellSlot.W);
                    break;

                case "zyra":
                    ReadyCast(800, SpellSlot.E, default(Vector3), true);
                    break;

                case "xerath":
                    ReadyCast(800, SpellSlot.E, _rengo.Position);
                    break;


            }
        }
        private static Obj_AI_Hero _target;

        public static Vector3 BackWardsCast(float range)
        {
            if (_rengo == null) return new Vector3();
            
            var rengopos = _rengo.Position;
            return rengopos.Extend(Player.Position, + _rengo.Position.Distance(Player.Position) + range);
        }

        public static Vector3 BackWardsCast2(float range)
        {
            if (_rengo == null) return new Vector3();

            var rengopos = _rengo.Position;
            return Player.Position.Extend(rengopos, +_rengo.Position.Distance(Player.Position) + range);
        }


        public static void ReadyCast(float range, SpellSlot slot, Vector3 position = new Vector3(), bool targetted = false)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            if (!spell.IsReady()) return;
            if (_rengo == null) return;
            if (_rengo.Position.Distance(Player.Position) > range) return;

            if (!targetted)
                spellbook.CastSpell(slot, position);
            else
                spellbook.CastSpell(slot, _rengo);
        }

        public static void SelfCast(float range, SpellSlot slot)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            if (!spell.IsReady()) return;
            if (!_rengo.IsValidTarget(range)) return;

            spellbook.CastSpell(slot);

        }


        public static bool Ready(SpellSlot slot)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            return !spell.IsReady();
        }
        private static void OnCreateObject(GameObject sender, EventArgs args)
        {
            if (!sender.IsEnemy) return;

            if (sender.Name != "Rengar_LeapSound.troy") return;

            foreach (var enemy in
                HeroManager.Enemies.Where(hero => hero.IsValidTarget(1500) && hero.ChampionName == "Rengar"))
            {
                _rengo = (Obj_AI_Hero)enemy;
                lastcasted = Environment.TickCount;
            }
        }
    }
}
