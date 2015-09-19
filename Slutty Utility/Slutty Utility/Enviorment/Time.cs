using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slutty_Utility.Enviorment
{
    class Time
    {
        private static readonly DateTime AssemblyLoadTime = DateTime.Now;

        public static float TickCount
        {
            get
            {
                return (int)DateTime.Now.Subtract(AssemblyLoadTime).TotalMilliseconds;
            }
        }

    }
}
