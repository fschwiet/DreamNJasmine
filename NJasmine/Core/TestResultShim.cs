using System
;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace NJasmine.Core
{
    [Serializable]
    public class TestResultShim
    {
        public enum Result
        {
            Successs = 0,
            Skipped = 1,
            Error = 2,
        }

        public enum Site
        {
            Test,
            SetUp,
            TearDown,
            Parent,
            Child
        }

        public Result Status { get { return _result.Value; } }
        public string FailureReason { get; private set; }
        public string FailureStackTrace { get; private set; }
        public Site FailureSite { get; private set; }
        public TimeSpan ExecutionTime { get; private set; }
        public string ReasonSkipped { get; private set; }

        public Result? _result;

        public TestResultShim()
        {
        }

        public void Success()
        {
            ApplyOutcome(Result.Successs);
        }

        public bool IsSuccess { get { return Status == Result.Successs;  } }

        public void SetError(string reason, string stackTrace, Site failureSite)
        {
            ApplyOutcome(Result.Error);
            FailureReason = reason;
            FailureStackTrace = stackTrace;
            FailureSite = failureSite;
        }

        public void SetExecutionTime(TimeSpan timeSpan)
        {
            ExecutionTime = timeSpan;
        }

        public void SetSkipped(string reason)
        {
            ApplyOutcome(Result.Skipped);
            ReasonSkipped = reason;
        }

        private void ApplyOutcome(Result result)
        {
            if (_result.HasValue)
                throw new InvalidOperationException("TestResultShim status should only be set once.");

            _result = result;
        }
    }
}
