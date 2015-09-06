using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;

namespace OAhri
{
   internal class GlobalManager : Ahri
    {
       #region R Count

       /// <summary>
       /// R Count
       /// </summary>
       /// <returns></returns>
       public static int RCount()
       {
           var buff = Player.Buffs.FirstOrDefault(x => x.Name == "AhriTumble");
           return buff != null ? buff.Count : 0;
       }

       #endregion

       #region Extend

       /// <summary>
       /// Simplfying Extend for me
       /// </summary>
       /// <param name="from"></param>
       /// <param name="to"></param>
       /// <param name="x"></param>
       /// <returns></returns>
       public static Vector3 Extend(Vector3 from, Vector3 to, float x)
       {
           return from.Extend(to, x);
       }

       #endregion

       #region Diffrenet Combo calculations

       /// <summary>
       /// Ignite Damage
       /// </summary>
       /// <param name="target"></param>
       /// <returns></returns>
       private static float IgniteDamage(Obj_AI_Hero target)
       {
           if (Ignite == SpellSlot.Unknown || Player.Spellbook.CanUseSpell(Ignite) != SpellState.Ready)
               return 0f;
           return (float) Player.GetSummonerSpellDamage(target, Damage.SummonerSpell.Ignite);
       }


       private static DamageToUnitDelegate _damageToUnit;
       public static bool EnableDrawingDamage { get; set; }
       public static Color DamageFillColor { get; set; }

       public delegate float DamageToUnitDelegate(Obj_AI_Hero hero);



       public static DamageToUnitDelegate DamageToUnit
       {
           get { return _damageToUnit; }

           set
           {
               if (_damageToUnit == null)
               {
                   Drawing.OnDraw += DrawManager.Drawing_OnDrawChamp;
               }
               _damageToUnit = value;
           }
       }


       /// <summary>
       /// Regular Calculation
       /// </summary>
       /// <param name="enemy"></param>
       /// <returns></returns>
       public static float RComboCalc(Obj_AI_Hero enemy)
       {
           var damage = 0d;

           if (Q.Instance.ManaCost <= Player.Mana)
               damage += Q.GetDamage(enemy);

           if (W.Instance.ManaCost <= Player.Mana)
               damage += W.GetDamage(enemy);

           if (E.Instance.ManaCost <= Player.Mana)
               damage += E.GetDamage(enemy);

           if (R.Instance.ManaCost <= Player.Mana)
               damage += R.GetDamage(enemy)*RCount();

           return (float) damage;
       }


       /// <summary>
       /// Total enemy Hp calculation
       /// </summary>
       /// <param name="enemy"></param>
       /// <returns></returns>
       public static float ComboCalc(Obj_AI_Hero enemy)
       {
           var damage =
               ObjectManager.Get<Obj_AI_Hero>()
                   .Where(x => !x.IsAlly && x.IsValidTarget(1000))
                   .Where(
                       hp =>
                           Q.Instance.ManaCost + W.Instance.ManaCost + E.Instance.ManaCost + R.Instance.ManaCost <=
                           Player.Mana)
                   .Aggregate(0d,
                       (current, hp) =>
                           current + (Q.GetDamage(hp) + W.GetDamage(hp) + E.GetDamage(hp) + (R.GetDamage(hp)*RCount())));
           return (float) damage;
       }


       /// <summary>
       /// Fukll Combo calculation
       /// </summary>
       /// <param name="enemy"></param>
       /// <returns></returns>
       public static float GetComboDamage(Obj_AI_Hero enemy)
       {
           var damage = 0d;
           if (Q.IsReady())
               damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

           if (E.IsReady())
               damage += Player.GetSpellDamage(enemy, SpellSlot.E);

           if (W.IsReady())
               damage += Player.GetSpellDamage(enemy, SpellSlot.W);

           if (R.IsReady())
               damage += Player.GetSpellDamage(enemy, SpellSlot.R)*RCount();

           if (Ignite.IsReady())
               damage += IgniteDamage(enemy);

           /*
            if (Q.Instance.ManaCost + W.Instance.ManaCost
                + E.Instance.ManaCost + R.Instance.ManaCost <= Player.Mana)
                damage += Q.GetDamage(enemy) + W.GetDamage(enemy) + E.GetDamage(enemy) + (R.GetDamage(enemy)*RCount());
            */

           return (float) damage;
       }

       #endregion

       // Thanks to Evade Geomatry! Credits goes to Evade!
       #region Wall Length

       /// <summary>
       /// Wall Lentgh
       /// </summary>
       /// <param name="start"></param>
       /// <param name="end"></param>
       /// <returns></returns>
       public static float GetWallLength(Vector3 start, Vector3 end)
       {
           double distance = Vector3.Distance(start, end);
           var firstPosition = Vector3.Zero;
           var lastPosition = Vector3.Zero;

           for (uint i = 0; i < distance; i += 10)
           {
               var tempPosition = start.Extend(end, i);
               if (tempPosition.IsWall() && firstPosition == Vector3.Zero)
               {
                   firstPosition = tempPosition;
               }
               lastPosition = tempPosition;
               if (!lastPosition.IsWall() && firstPosition != Vector3.Zero)
               {
                   break;
               }
           }

           return Vector3.Distance(firstPosition, lastPosition);
       }

       #endregion

       #region Is Over Walla

       /// <summary>
       /// Checks For Wall
       /// </summary>
       /// <param name="start"></param>
       /// <param name="end"></param>
       /// <returns></returns>
       public static bool IsOverWall(Vector3 start, Vector3 end)
       {
           double distance = Vector3.Distance(start, end);
           for (uint i = 0; i < distance; i += 10)
           {
               var tempPosition = start.Extend(end, i).To2D();
               if (tempPosition.IsWall())
               {
                   return true;
               }
           }
           return false;
       }

       #endregion

       #region First Wall point

       /// <summary>
       /// First wall Point
       /// </summary>
       /// <param name="start"></param>
       /// <param name="end"></param>
       /// <returns></returns>
       public static Vector3 GetFirstWallPoint(Vector3 start, Vector3 end)
       {
           double distance = Vector3.Distance(start, end);
           for (uint i = 0; i < distance; i += 10)
           {
               var tempPosition = start.Extend(end, i);
               if (!tempPosition.IsWall())
               {
                   return tempPosition.Extend(start, -35);
               }
           }

           return Vector3.Zero;
       }

       #endregion

    }
}
