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

        public static Obj_AI_Hero Player = ObjectManager.Player;
        private static Obj_AI_Hero _rengo;
        private static int lastcasted;

        public static void OnLoad(EventArgs args)
        {

            Game.PrintChat("<font color='#6f00ff'>[Ward Bush]:</font> <font color='#FFFFFF'>" + "To Enable, Type w on, To Disable, Type w off" + "</font>");

            Game.OnUpdate += OnUpdate;
            GameObject.OnCreate += OnCreateObject;
            Obj_AI_Base.OnProcessSpellCast += OnProcess;
        }

        private static void OnProcess(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (args.SData.Name == "randomnamehere")
            {
                if (args.Target.IsAlly || args.Target.IsMe)
                {
                    _target = (Obj_AI_Hero) args.Target;
                }
            }
        }

        private static void OnUpdate(EventArgs args)
        {

            Game.OnUpdate += OnUpdate;
            if (_rengo.IsDead || Environment.TickCount - lastcasted > 8*10*10*10)
            {
                _rengo = null;
            }
            if (_rengo == null) return;
            var rengopos = _rengo.Position;
            var spellbook = Player.Spellbook;
            switch (Player.ChampionName.ToLower())
            {
                case "vayne":
                    ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    if (!Ready(SpellSlot.E))
                    ReadyCast(Player.AttackRange, SpellSlot.Q, BackWardsCast(500));
                    break;

                case "ahri":
                    ReadyCast(500, SpellSlot.E, default(Vector3));
                    break;

                case "alistar":
                    SelfCast(500, SpellSlot.Q);
                    break;

                case "anivia":
                    ReadyCast(500, SpellSlot.W, _target.Position.Extend(_rengo.Position, 150));
                    break;

                case "caitlyn":
                    ReadyCast(1000, SpellSlot.W, _target.Position);
                    break;

                case "annie":
                    if (Player.HasBuff("pyromaniaparticle"))
                        ReadyCast(500, SpellSlot.Q, default(Vector3), true);
                    break;

                case "ashe":
                    ReadyCast(500, SpellSlot.R, default(Vector3), true);
                    break;

                case "azir":
                    ReadyCast(700, SpellSlot.R, _target.Position.Extend(_rengo.Position, Player.BoundingRadius + 100));
                    break;

                case "blitzcrank":
                    SelfCast(500, SpellSlot.R);
                    break;

                case "chogath":
                    ReadyCast(300, SpellSlot.Q, _target.Position);
                    ReadyCast(500, SpellSlot.W, default(Vector3), true);
                    break;

                case "Diana":
                    SelfCast(500, SpellSlot.E);
                    break;

                case "Draven":
                    ReadyCast(600, SpellSlot.E, default(Vector3), true);
                    break;

                case "elise":
                    ReadyCast(600, SpellSlot.E, default(Vector3), true);
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
                    if(_target.IsMe)
                    SelfCast(Player.AttackRange, SpellSlot.Q);
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
                    ReadyCast(500, SpellSlot.E, _target.Position);
                    break;

                case "leblanc":
                    ReadyCast(300, SpellSlot.E, default(Vector3), true);
                    break;

                case "lulu":
                    ReadyCast(500, SpellSlot.W, default(Vector3), true);
                    break;

                case "lux":
                    ReadyCast(300, SpellSlot.Q, default(Vector3), true);
                    break;

                case "nami":
                    ReadyCast(400, SpellSlot.Q, _target.Position);
                    break;

                case "quinn":
                    ReadyCast(500, SpellSlot.Q, _rengo.Position);
                    ReadyCast(500, SpellSlot.E, default(Vector3), true);
                    break;

                case "riven":
                    SelfCast(500, SpellSlot.W);
                    break;

                case "shaco":
                    SelfCast(300, SpellSlot.Q);
                    break;

                case "soraka":
                    ReadyCast(500, SpellSlot.E, _target.Position);
                    break;

                case "swain":
                    ReadyCast(500, SpellSlot.E, _target.Position);
                    break;

                case "thresh":
                    ReadyCast(400, SpellSlot.E, BackWardsCast(100));
                    break;

                case "tristana":
                    ReadyCast(500, SpellSlot.R, default(Vector3), true);
                    break;

                case "velkoz":
                    ReadyCast(600, SpellSlot.E, _target.Position);
                    break;

                case "viktor":
                    ReadyCast(700, SpellSlot.W, _target.Position);
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

        public static string[] Champlist =
        {
            "Vayne", "Ahri", "Alistar", "Anivia", "Caitlyn", "Annie"
            , "Ashe", "Azir", "Bitzcrank", "Chogath", "Diana", "Draven",
            "Elise", "Ezreal", "Fiddlesticks", "Fizz", "Garen", "Gragas", "Irelia", "Janna"
            , "Jayce", "Jinx", "LeBlanc", "Leesin", "Leona", "Lulu", "Lux", "Nami"
            , "Quinn", "Riven", "Shaco", "Sivir", "Soraka", "Swain", "Syndra", "Thresh"
            , "Tristana", "Velkoz", "Viktor", "MonkeyKing", "Zyra", "Xerath"
        };

        private static Obj_AI_Hero _target;

        public static Vector3 BackWardsCast(float range)
        {
            var rengopos = _rengo.Position;
            return rengopos.Extend(Player.Position, range);
        }

        public static void ReadyCast(float range, SpellSlot slot, Vector3 position = new Vector3(), bool targetted = false)
        {
            var spellbook = Player.Spellbook;
            var spell = spellbook.GetSpell(slot);
            if (!spell.IsReady()) return;
            if (_rengo.Distance(_target) > range) return;

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
                _rengo = (Obj_AI_Hero) enemy;
                lastcasted = Environment.TickCount;
            }
        }
    }
}
