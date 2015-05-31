using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using LeagueSharp;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using Color = System.Drawing.Color;
using SharpDX;

namespace Slutty_Gnar
{
    public static class Name
    {
        public static bool IsMiniGnar(this Obj_AI_Hero target)
        {
            return target.BaseSkinName == "Gnar";
        }

        public static bool IsMegaGnar(this Obj_AI_Hero target)
        {
            return target.BaseSkinName == "gnarbig";
        }
    }
}
