using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NJasmine.Extras;
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
            var failure = _xml.Element("failure").Element("message");

            Expect.That(() => failure.Value.Contains(expectedMessage));

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
            var stackTrace = _xml.Element("failure").Element("stack-trace");

            handler(stackTrace.Value);

            return this;
        }

        public TestResult withCategories(params string[] categories)
        {
            return withCategories<TestResult>(categories);
        }
    }
}