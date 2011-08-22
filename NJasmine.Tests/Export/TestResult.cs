using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class TestResult : BaseResult
    {
        public TestResult(XElement element) : base("test", element)
        {
        }

        public TestResult withFailureMessage(string expectedMessage)
        {
            var failure = _xml.Element("failure");

            if (failure != null)
                failure = failure.Element("message");

            Assert.That(failure.Value, Is.StringContaining(expectedMessage));

            return this;
        }

        public TestResult thatSucceeds()
        {
            thatHasResult("Success");
            return this;
        }

        public TestResult thatErrors()
        {
            thatHasResult("Error");
            return this;
        }

        public TestResult thatIsNotRunnable()
        {
            thatHasResult("NotRunnable");
            return this;
        }

        public TestResult thatFails()
        {
            thatHasResult("Failure");
            return this;
        }

        public TestResult thatFailsInAnUnspecifiedManner()
        {
            var result = GetResult();

            var possibleFailures = new[] {"Failure", "Error"};

            Assert.True(possibleFailures.Contains(result),
                String.Format("Expected test {0} to have failure result, actual was {1}.", _name, result));

            return this;
        }

        public TestResult withDetailedMessageThat(Action<string> handler)
        {
            var stackTrace = _xml.Element("failure");

            if (stackTrace != null)
                stackTrace = stackTrace.Element("stack-trace");

            handler(stackTrace.Value);

            return this;
        }
    }
}