using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class TestResultShim
    {
        enum Status
        {
            Inconclusive = 0,
            Successs,
            Error
        }

        public enum Site
        {
            Test,
            SetUp,
            TearDown,
            Parent,
            Child
        }

        Status _status;
        string _failureReason;
        string _failureStackTrace;
        Site _failureSite;

        public TestResultShim()
        {
        }

        public void Success()
        {
            _status = Status.Successs;
        }

        public bool IsSuccess { get { return _status == Status.Successs;  } }

        public TestResult ApplyToNunitResult(TestResult result)
        {
            switch(_status)
            {
                case Status.Inconclusive:
                    break;
                case Status.Successs:
                    result.Success();
                    break;
                case Status.Error:
                    result.SetResult(ResultState.Error, _failureReason, _failureStackTrace, GetNUnitFailureSite(_failureSite));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }

        public void SetError(string reason, string stackTrace, Site failureSite)
        {
            _status = Status.Error;
            _failureReason = reason;
            _failureStackTrace = stackTrace;
            _failureSite = failureSite;
        }

        private static FailureSite GetNUnitFailureSite(Site site)
        {
            switch(site)
            {
                case Site.Test:
                    return FailureSite.Test;
                case Site.SetUp:
                    return FailureSite.SetUp;
                case Site.TearDown:
                    return FailureSite.TearDown;
                case Site.Parent:
                    return FailureSite.Parent;
                case Site.Child:
                    return FailureSite.Child;
                default:
                    throw new ArgumentOutOfRangeException("site");
            }
        }
    }
}
