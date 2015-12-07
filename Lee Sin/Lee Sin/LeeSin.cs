using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lee_Sin.Misc;
using ItemData = LeagueSharp.Common.Data.ItemData;
using Version = System.Version;

namespace Lee_Sin
{
    internal class LeeSin : Helper
    {
        #region vars, enums, misc
        public static bool CanFlash = false;
       public static  bool CanWard = false;
        public static Spell Q, W, E, R;
        public static SpellSlot _flashSlot;
        public static int _lastward;
        public static steps? Steps;
        public static int _laste;
        public static int _lastqc;
        public static int _lastelane;
        public static bool _processw;
        public static int _lastwj;
        public static int _lastej;
        public static SpellSlot Smite;
        public static readonly int Hydra;
        public static readonly int Tiamat;
        public static readonly int Youm;
        public static readonly int Omen;
        public static Obj_AI_Base SelectedAllyAiMinion;
        public static int _lastwards;
        public static int _lastprocessw;
        public static int _lastwardjump;
        public static bool _processr;
        public static int _lastprocessr;
        public static bool _processr2;
        public static int _clickCount;
        public static bool _lastClickBool;
        public static Vector3 _lastClickPos;
        public static bool _b;
        public static bool _processW2;
        public static int _lastwcombo;
        public static int _lastwarr;
        public static int _processr2T;
        public static int _lastqh;
        public static int _lasteh;
        public static bool _processroncast;
        public static int _processroncastr;
        public static Geometry.Polygon.Rectangle _ultPolyExpectedPos;
        public static bool _created;
        public static Vector3? _rCombo;
        public static StringFormat _stringf;
        public static int _lastcasted;
        public static int _lastwcasted;
        public static bool _wardjumpedtotarget;
        public static int _lastqcasted;
        public static int _lastflashward;
        public static int _wardjumpedto;
        public static int _junglelastq;
        public static int _junglelastw;
        public static int _junglelaste;
        public static int lastflashed;
        public static bool canwardflash;
        public static int _lastqcasted1;
        public static Obj_AI_Base minions;
        public static int wardlastcasted;
        public static int lastr;
        public static int lastq;
        public static bool buff;
        public static int lastflashoverprio;
        protected static int Lastcastedw;

        static LeeSin()
        {
            Hydra = 3074;
            Tiamat = 3077;
            Youm = 3142;
            Omen = 3143;
        }

        public enum steps
        {
            Q,
            Q2,
            WardJump,
            WardJumpMelee,
            FlashMelee,
            Flash,
            R,
            WFlash

        }


        #endregion
        
        internal static void Load(EventArgs args)
        {
            OnLoad.OnLoaded();
        }
       
        #region Has Passive

        public static bool HasPassive()
        {
            return Player.HasBuff("blindmonkpassive_cosmetic");
        }

        #endregion


        public static bool spellweave
        {
            get
            {
                return Environment.TickCount - _junglelastq > Game.Ping + 250 &&
                       Environment.TickCount - _junglelastw > Game.Ping + 250
                       && Environment.TickCount - _junglelaste > Game.Ping + 250;
            }
        }

        public static bool CanCast(SpellSlot spellSlot)
        {
            switch (spellSlot)
            {
                case SpellSlot.Q:
                    return !HasPassive() && spellweave && Player.Spellbook.GetSpell(SpellSlot.Q).IsReady()
                        && !W2() && !E2();
                case SpellSlot.W:
                    return !HasPassive() && spellweave && Player.Spellbook.GetSpell(SpellSlot.W).IsReady()
                        && !Q2() && !E2();
                case SpellSlot.E:
                    return !HasPassive() && spellweave && Player.Spellbook.GetSpell(SpellSlot.E).IsReady()
                        && !Q2() && !W2();
            }
            return false;
        }


        #region Q Damage

        public static float GetQDamage(Obj_AI_Base unit)
        {
            var firstq = Q.GetDamage(unit);
            var secondq = Q.GetDamage(unit) + (unit.MaxHealth - unit.Health - Q.GetDamage(unit))*0.08;
            return (float) (firstq + secondq);
        }

        #endregion

        #region States

        public static bool Q1()
        {
           return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "BlindMonkQOne"; 
        }

        public static bool Q2()
        {
           return Player.Spellbook.GetSpell(SpellSlot.Q).Name == "blindmonkqtwo"; 
        }

        public static bool W1()
        {
           return Player.Spellbook.GetSpell(SpellSlot.W).Name == "BlindMonkWOne"; 
        }

        public static bool W2()
        {
             return Player.Spellbook.GetSpell(SpellSlot.W).Name == "blindmonkwtwo"; 
        }

        public static bool E1()
        {
           return Player.GetSpell(SpellSlot.E).Name == "BlindMonkEOne"; 
        }

        public static bool E2()
        {
            return Player.GetSpell(SpellSlot.E).Name == "blindmonketwo"; 
        }

        public static bool HasFlash()
        {
            return Player.GetSpellSlot("summonerflash").IsReady();
        }

        #endregion
        

        public static bool LastQ(Obj_AI_Hero target, bool includeMinions = true)
        {
            if (target == null) return false;
            
            if (buff)
            {
                lastq = Environment.TickCount;
                buff = false;
            }

            if (target.HasBuff("blindmonkqtwo") && Q2() && Environment.TickCount - lastq > 2000)
            {
                buff = true;
            }

            if (minionss.IsValidTarget() && minionss.HasBuff("blindmonkqtwo") && includeMinions)
            return Environment.TickCount - lastq > 2000 && minionss.Distance(Player) > 400;
            else
            return Environment.TickCount - lastq > 2000;
        }


        public static Obj_AI_Base minionss { get; set; }

        public static bool CanWardFlash(Obj_AI_Hero target)
        {
            var wardFlashBool = GetBool("expwardflash", typeof(bool));
            var slot = Items.GetWardSlot();

            return slot != null && HasFlash() && W.IsReady() &&
                   R.IsReady() && wardFlashBool && Environment.TickCount - lastr > 2000;
        }

        public static bool Colbool { get; set; }

        public static Vector3 Playerpos { get; set; }

        public static Geometry.Polygon.Rectangle UltPoly { get; set; }

        public static Vector3 RCombos { get; set; }

        public static Render.Text Text { get; set; }

        public static ColorBGRA color { get; set; }
    }
}