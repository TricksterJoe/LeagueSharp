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
using Rectangle = SharpDX.Rectangle;
using Version = System.Version;

namespace Lee_Sin
{
    internal class LeeSin : Helper
    {
        #region vars, enums, misc
        public static bool CanFlash = false;
       public static  bool CanWard = false;
        public static Spell Q, W, E, R, Rnormal;
        public static SpellSlot FlashSlot;
        public static int Lastward;
        public static steps? Steps;
        public static int Laste;
        public static int Lastqc;
        public static int Lastelane;
        public static bool Processw;
        public static int Lastwj;
        public static int Lastej;
        public static SpellSlot Smite;
        public static readonly int Hydra;
        public static readonly int Tiamat;
        public static readonly int Youm;
        public static readonly int Omen;
        public static Obj_AI_Base SelectedAllyAiMinion;
        public static int Lastwards;
        public static int Lastprocessw;
        public static int Lastwardjump;
        public static bool Processr;
        public static int Lastprocessr;
        public static bool Processr2;
        public static int ClickCount;
        public static bool LastClickBool;
        public static Vector3 LastClickPos;
        public static bool B;
        public static bool ProcessW2;
        public static int Lastwcombo;
        public static int Lastwarr;
        public static int Processr2T;
        public static int Lastqh;
        public static int Lasteh;
        public static bool Processroncast;
        public static int Processroncastr;
        public static Geometry.Polygon.Rectangle UltPolyExpectedPos;
        public static bool Created;
        public static Vector3? RCombo;
        public static StringFormat Stringf;
        public static int Lastcasted;
        public static int Lastwcasted;
        public static bool Wardjumpedtotarget;
        public static int Lastqcasted;
        public static int Lastflashward;
        public static int Wardjumpedto;
        public static int Junglelastq;
        public static int Junglelastw;
        public static int Junglelaste;
        public static int Lastflashed;
        public static bool Canwardflash = true;
        public static int Lastqcasted1;
        public static Obj_AI_Base Minions;
        public static int Wardlastcasted;
        public static int Lastr;
        public static int Lastq { get; set; }
        public static bool Buff { get; set; }
        public static int Lastflashoverprio;
        protected static int Lastcastedw;
        protected static int Lastq2Casted;
        protected static int Lastq1Casted;
        protected static int Lastwardmanager;
        protected static int Lastwardjumpd;
        protected static int Sightwardcreated;
        protected static int Lastcanjump;
        protected static int Lsatcanjump1;
        public static int Lastbuff { get; set; }
        public static int Lastq12 { get; set; }
        public static Obj_AI_Base Minionss { get; set; }

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


        public static bool Spellweave
        {
            get
            {
                return Environment.TickCount - Junglelastq > Game.Ping + 250 &&
                       Environment.TickCount - Junglelastw > Game.Ping + 250
                       && Environment.TickCount - Junglelaste > Game.Ping + 250;
            }
        }

        public static bool CanCast(SpellSlot spellSlot)
        {
            switch (spellSlot)
            {
                case SpellSlot.Q:
                    return !HasPassive() && Spellweave && Player.Spellbook.GetSpell(SpellSlot.Q).IsReady()
                        && !W2() && !E2();
                case SpellSlot.W:
                    return !HasPassive() && Spellweave && Player.Spellbook.GetSpell(SpellSlot.W).IsReady()
                        && !Q2() && !E2();
                case SpellSlot.E:
                    return !HasPassive() && Spellweave && Player.Spellbook.GetSpell(SpellSlot.E).IsReady()
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
        
        
        public static bool LastQ(Obj_AI_Hero target)
        {
            if (target == null) return false;

            if (Buff)
            {
                Lastq12 = Environment.TickCount;
                 Buff = false;
            }



            var poss = InsecPos.WardJumpInsecPosition.InsecPos(target, GetValue("fixedwardrange"), true);

            foreach (var obj in ObjectManager.Get<Obj_AI_Base>()
                .Where(
                    x => x.IsEnemy &&
                         x.Buffs.Any(a => a.Name.ToLower().Contains("blindmonkqone"))
                )) 
            {
                if (obj.Distance(target) < 350 || obj.Distance(poss) < 450 ||
                    (CanWardFlash(target) && obj.Distance(target) < 900))
                {
                    if (!Buff)
                        Lastbuff = Environment.TickCount;
                }
            }

            if (Environment.TickCount - Lastq2Casted < 100 &&
                Environment.TickCount - Lastq12 > 3000 && Environment.TickCount - Lastbuff < 100)
            {
                Buff = true;
            }

            return Environment.TickCount - Lastq12  > 3000;
        }

        public static Obj_AI_Base Objs { get; set; }


        public static bool CanWardFlash(Obj_AI_Hero target)
        {
            var wardFlashBool = GetBool("expwardflash", typeof(bool));
            var slot = Items.GetWardSlot();

            return slot != null && HasFlash() && R.IsReady() && W.IsReady() && wardFlashBool && Environment.TickCount - Lastr > 1000;
        }

        public static bool Colbool { get; set; }

        public static Vector3 Playerpos { get; set; }

        public static Geometry.Polygon.Rectangle UltPoly { get; set; }

        public static Vector3 RCombos { get; set; }

        public static Render.Text Text { get; set; }

        public static ColorBGRA color { get; set; }
        public static int Lasttotarget { get; protected set; }
        public static int LastTeleported { get; set; }
        public static int LastBubba { get; set; }
    }
}