using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tohoSRPG
{
    /// <summary>
    /// アビリティ
    /// </summary>
    public enum Ability
    {
        None,
        SymptonClear,
        ActAgain,
        Drive
    }

    static class AbilityString
    {
        public static void GetDescription(Ability a, out string str1, out string str2)
        {
            switch (a)
            {
                case Ability.Drive:
                    str1 = "";
                    str2 = "";
                    break;
                default:
                    str1 = "";
                    str2 = "";
                    break;
            }
        }
    }
}
