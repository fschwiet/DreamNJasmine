using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmine.Core
{
    public class NUnitFrameworkUtil
    {
        //  this function is extract from NUnitFramework.ApplyCommonAttributes
        public static void ApplyCategoryToTest(string category, Test test)
        {
            test.Categories.Add(category);

            if (category.IndexOfAny(new char[] { ',', '!', '+', '-' }) >= 0)
            {
                test.RunState = RunState.NotRunnable;
                test.IgnoreReason = "Category name must not contain ',', '!', '+' or '-'";
            }
        }
    }
}
