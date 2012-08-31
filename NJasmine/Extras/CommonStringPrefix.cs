using System;
using System.Collections.Generic;
using System.Linq;

namespace NJasmine.Extras
{
    public class CommonStringPrefix
    {
        public static string Of(IEnumerable<string> strings)
        {
            var commonPrefix = strings.FirstOrDefault() ?? "";

            foreach(var s in strings)
            {
                var potentialMatchLength = Math.Min(s.Length, commonPrefix.Length);

                if (potentialMatchLength < commonPrefix.Length)
                    commonPrefix = commonPrefix.Substring(0, potentialMatchLength);

                for(var i = 0; i < potentialMatchLength; i++)
                {
                    if (s[i] != commonPrefix[i])
                    {
                        commonPrefix = commonPrefix.Substring(0, i);
                        break;
                    }
                }
            }

            return commonPrefix;
        }
    }
}