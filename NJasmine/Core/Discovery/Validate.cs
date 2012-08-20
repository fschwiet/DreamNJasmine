using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.Discovery
{
    public class Validate
    {
        public static string CheckForCategoryError(string category)
        {
            string invalidReason = null;
            if (category.IndexOfAny(new char[] {',', '!', '+', '-'}) >= 0)
            {
                invalidReason = "Category name must not contain ',', '!', '+' or '-'";
            }
            return invalidReason;
        }
    }
}
