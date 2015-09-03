using System.Linq;
using LeagueSharp;
using LeagueSharp.Common;

namespace OAhri
{
    internal class AhriR : Ahri
    {
        public static void RUlt()
        {
            var target = TargetSelector.GetTarget(Q.Range, TargetSelector.DamageType.Magical);
            if (target == null)
                return;

            var turret =
                ObjectManager.Get<Obj_AI_Turret>().FirstOrDefault(x => !x.IsAlly && !x.IsDead && x.Distance(Player) <= 500);
            var killable = ObjectManager.Get<Obj_AI_Hero>().Where(x => !x.IsAlly && x.IsValidTarget(1000));
            var objAiHeroes = killable as Obj_AI_Hero[] ?? killable.ToArray();
            switch (Config.Item("comboMenu.user").GetValue<StringList>().SelectedIndex)
            {
                case 0:
                {
                    if (GlobalManager.RCount() >= 1)
                    {
                        if ((Q.IsReady() || W.IsReady() || E.IsReady()))
                        {
                            Utility.DelayAction.Add(2000,
                                    () => R.Cast(GlobalManager.Extend(Player.ServerPosition, Game.CursorPos,
                                R.Range)));
                        }
                    }
                    break;
                }

                case 1:
                {
                    if (target.IsValidTarget(R.Range + E.Range))
                    {
                        if (Q.IsReady() || E.IsReady() || W.IsReady())
                        {
                            if (target.Health <= GlobalManager.GetComboDamage(target) && turret == null)
                            {       
                              R.Cast(GlobalManager.Extend(Player.ServerPosition, Game.CursorPos,
                                        R.Range));
                            }
                     
                            foreach (var hp in objAiHeroes.Where(hp => GlobalManager.ComboCalc(hp) >= hp.Health))
                            {
                                R.Cast(GlobalManager.Extend(Player.ServerPosition, Game.CursorPos, R.Range));
                            }
                             
                        }

                    }
                    break;
                }
                case 2:
                {
                    return;
                }
            }
        }
    }
}