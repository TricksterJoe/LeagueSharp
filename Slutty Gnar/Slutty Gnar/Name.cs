using LeagueSharp;

namespace Slutty_Gnar
{
    public static class Name
    {
        public static bool IsMiniGnar(this Obj_AI_Hero target)
        {
            return target.CharData.BaseSkinName == "Gnar";
        }

        public static bool IsMegaGnar(this Obj_AI_Hero target)
        {
            return target.CharData.BaseSkinName == "gnarbig";
        }
    }
}
