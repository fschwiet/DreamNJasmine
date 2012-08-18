using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core
{
    [Serializable]
    public class TestResultShim
    {
        public enum Result
        {
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

        public Result Status { get; private set; }
        public string FailureReason { get; private set; }
        public string FailureStackTrace { get; private set; }
        public Site FailureSite { get; private set; }
        public TimeSpan ExecutionTime { get; private set; }

        public TestResultShim()
        {
        }

        public void Success()
        {
            Status = Result.Successs;
        }

        public bool IsSuccess { get { return Status == Result.Successs;  } }

        public void SetError(string reason, string stackTrace, Site failureSite)
        {
            Status = Result.Error;
            FailureReason = reason;
            FailureStackTrace = stackTrace;
            FailureSite = failureSite;
        }

        public void SetExecutionTime(TimeSpan timeSpan)
        {
            ExecutionTime = timeSpan;
        }
    }
}
