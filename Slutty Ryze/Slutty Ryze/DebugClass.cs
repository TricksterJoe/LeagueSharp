using System;


namespace Slutty_ryze
{
    class DebugClass
    {
        public static void ShowDebugInfo(bool b)
        {
            Console.WriteLine("Passive Stacks:{0}",GlobalManager.GetPassiveBuff);
            Console.WriteLine("Estimated Damage to Current target:{0}",GlobalManager.DamageToUnit);
            foreach (var item in GlobalManager.Config.Items)
            {
                Console.WriteLine(item);
            }
        }
    }
}
