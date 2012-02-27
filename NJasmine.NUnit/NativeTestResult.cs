using System;
using NJasmine.Core;
using NUnit.Core;

namespace NJasmine.NUnit
{
    public class NativeTestResult
    {
        public static TestResult ApplyToNunitResult(TestResultShim shim, TestResult result)
        {
            switch (shim.Status)
            {
                case TestResultShim.Result.Inconclusive:
                    break;
                case TestResultShim.Result.Successs:
                    result.Success();
                    break;
                case TestResultShim.Result.Error:
                    result.SetResult(ResultState.Error, shim.FailureReason, shim.FailureStackTrace, GetNUnitFailureSite(shim.FailureSite));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        private static FailureSite GetNUnitFailureSite(TestResultShim.Site site)
        {
            switch (site)
            {
                case TestResultShim.Site.Test:
                    return FailureSite.Test;
                case TestResultShim.Site.SetUp:
                    return FailureSite.SetUp;
                case TestResultShim.Site.TearDown:
                    return FailureSite.TearDown;
                case TestResultShim.Site.Parent:
                    return FailureSite.Parent;
                case TestResultShim.Site.Child:
                    return FailureSite.Child;
                default:
                    throw new ArgumentOutOfRangeException("site");
            }
        }
    }
}