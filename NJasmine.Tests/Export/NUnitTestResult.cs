using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NJasmine.Extras;
using NJasmine.Marshalled;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class NUnitTestResult : NUnitBaseResult, ITestResult
    {
        public NUnitTestResult(XElement element) : base("test", element)
        {
        }

        public ITestResult withFailureMessage(string expectedMessage)
        {
            var failure = _xml.Element("failure").Element("message");

            Expect.That(() => failure.Value.Contains(expectedMessage));

            return this;
        }

        public ITestResult thatSucceeds()
        {
            thatHasResult("Success");
            return this;
        }

        public ITestResult thatErrors()
        {
            thatHasResult("Error");
            return this;
        }

        public ITestResult thatIsNotRunnable()
        {
            thatHasResult("NotRunnable");
            return this;
        }

        public ITestResult thatFails()
        {
            thatHasResult("Failure");
            return this;
        }

        public ITestResult thatFailsInAnUnspecifiedManner()
        {
            var result = GetResult();

            var possibleFailures = new[] {"Failure", "Error"};

            Assert.True(possibleFailures.Contains(result),
                String.Format("Expected test {0} to have failure result, actual was {1}.", _name, result));

            return this;
        }

        public ITestResult withDetailedMessageThat(Action<string> handler)
        {
            var stackTrace = _xml.Element("failure").Element("stack-trace");

            handler(stackTrace.Value);

            return this;
        }

        public ITestResult withCategories(params string[] categories)
        {
            return withCategories<NUnitTestResult>(categories);
        }
    }
}