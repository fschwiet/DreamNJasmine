using System;

namespace NJasmineTests.Export
{
    public class VS2012TestResult : ITestResult
    {
        public ITestResult withFailureMessage(string expectedMessage)
        {
            return this;
        }

        public ITestResult thatSucceeds()
        {
            return this;
        }

        public ITestResult thatErrors()
        {
            return this;
        }

        public ITestResult thatIsNotRunnable()
        {
            return this;
        }

        public ITestResult thatFails()
        {
            return this;
        }

        public ITestResult thatFailsInAnUnspecifiedManner()
        {
            return this;
        }

        public ITestResult withDetailedMessageThat(Action<string> handler)
        {
            return this;
        }

        public ITestResult withCategories(params string[] categories)
        {
            return this;
        }
    }
}
