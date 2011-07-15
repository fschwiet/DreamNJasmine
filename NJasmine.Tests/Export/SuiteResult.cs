using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class SuiteResult : BaseResult
    {
        public SuiteResult(XElement xml) : base("test suite", xml)
        {
        }

        public SuiteResult thatsInconclusive()
        {
            thatHasResult("Inconclusive");
            return this;
        }

        public SuiteResult thatSucceeds()
        {
            thatHasResult("Success");
            return this;
        }

        public SuiteResult thatHasNoResults()
        {
            var results = _xml.Descendants("results");

            Assert.AreEqual(0, results.Count(), "Expected suite " + _name + " to not have any results.");

            return this;
        }

        public TestResult ShouldHaveTest(string name)
        {
            var tests = _xml.Descendants("test-case").Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(),
                String.Format("Expected test not found in suite {0}, expected test named {1}", _name, name));

            return new TestResult(tests.Single());
        }
    }
}