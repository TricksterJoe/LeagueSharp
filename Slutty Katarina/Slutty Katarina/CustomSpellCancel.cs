using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace Slutty_Katarina
{
    class CustomSpellCancel
    {
        
        /// <summary>
        /// Allow user to cancel channeling
        /// </summary>
        public static bool CanBeCanceledByUser { get; set; }

        /// <summary>
        /// check if the spell is being channeled
        /// </summary>
        public static bool IsChanneling = false;

        /// <summary>
        /// Is spell type channel
        /// </summary>
        public static bool IsChannelTypeSpell { get; set; }

        /// <summary>
        /// Is spell targettable
        /// </summary>
        public static bool TargetSpellCancel { get; set; }

        /// <summary>
        /// Should the spell  be interuptable by casting other spells
        /// </summary>
        public static bool LetSpellcancel { get; set; }

        /// <summary>
        /// Last time casting has been issued
        /// </summary>
        private static int _cancelSpellIssue;


        /// <summary>
        /// Spell setings
        /// </summary>
        /// <param name="letUserCancel"></param>
        /// <param name="targetted"></param>
        /// <param name="letSpellCancel"></param>
        public static void Setinterruptible(bool letUserCancel, bool targetted,
            bool letSpellCancel = false)
        {
            CanBeCanceledByUser = letUserCancel;
            TargetSpellCancel = targetted;
            IsChanneling = false;
            LetSpellcancel = letSpellCancel;

            Obj_AI_Base.OnDoCast += OnDoCast;
            GameObject.OnDelete += OnDelete;
            Game.OnWndProc += OnWndProc;
            Obj_AI_Base.OnIssueOrder += OnOrder;
            Spellbook.OnCastSpell += OnCastSpell;


        }

        /// <summary>
        /// Diffrenet spell process names
        /// </summary>
        private static readonly string[] _processName =
        {
            "DrainChannel", "KatarinaR", "Crowstorm",
            "GalioIdolOfDurand", "AlZaharNetherGrasp",
            "ReapTheWhirlwind"
        };

        /// <summary>
        /// Diffrenet object names
        /// </summary>
        private static readonly string[] _deleteObject =
        {
            "Fiddlesticks_Base_Drain.troy", "katarina_deathLotus_tar.troy",
            "Galio_Base_R_explo.troy", "Malzahar_Base_R_Beam.troy",
            "ReapTheWhirlwind_green_cas.troy",
        };


        /// <summary>
        /// Check when the skill object has been casted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnDoCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe) return;

            if (_processName.Contains(args.SData.Name))
            {
                IsChanneling = true;
            }
        }

        /// <summary>
        /// Check when an object has been deleted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (_deleteObject.Contains(sender.Name))
            {
                IsChanneling = false;
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {

            if (LetSpellcancel) return;

            args.Process = !IsChanneling;
        }

        public static void CastCancelSpell()
        {
            if (!IsChanneling && Utils.TickCount - _cancelSpellIssue > 400 + Game.Ping)
            {
                ObjectManager.Player.Spellbook.CastSpell(Slot);
                _cancelSpellIssue = Utils.TickCount;
            }
        }

        public static SpellSlot Slot { get; set; }

        public static void CastCancelSpell(Vector3 position)
        {
            if (!IsChanneling && Utils.TickCount - _cancelSpellIssue > 400 + Game.Ping)
            {
                ObjectManager.Player.Spellbook.CastSpell(Slot, position);
                _cancelSpellIssue = Utils.TickCount;
            }
        }


        /// <summary>
        /// Check when a spell has been casted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void OnOrder(Obj_AI_Base sender, GameObjectIssueOrderEventArgs args)
        {
            if (!sender.IsMe) return;

            if (!IsChanneling) return;

            if (args.Order == GameObjectOrder.MoveTo || args.Order == GameObjectOrder.AttackTo ||
                args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AutoAttack)
            {
                args.Process = false;
            }
        }

        /// <summary>
        /// When player sends a key command
        /// </summary>
        /// <param name="args"></param>
        private static void OnWndProc(WndEventArgs args)
        {

            if (!CanBeCanceledByUser) return;

            if (args.Msg == 517)
            {
                IsChanneling = false;
            }
        }
    }
}
