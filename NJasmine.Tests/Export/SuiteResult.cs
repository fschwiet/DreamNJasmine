using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NJasmine.Extras;
using NJasmine.Marshalled;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class SuiteResult : BaseResult, ISuiteResult
    {
        readonly string _fullName;

        public SuiteResult(string fixtureName, XElement xml) : base("test suite", xml)
        {
            _fullName = fixtureName + ", " + _name;
        }

        public ISuiteResult thatsInconclusive()
        {
            thatHasResult("Inconclusive");
            return this;
        }

        public ISuiteResult thatSucceeds()
        {
            thatHasResult("Success");
            return this;
        }

        public ISuiteResult thatHasNoResults()
        {
            var results = _xml.Descendants("results");

            Assert.AreEqual(0, results.Count(), "Expected suite " + _name + " to not have any results.");

            return this;
        }

        public ITestResult hasTest(string expectedName)
        {
            IEnumerable<XElement> tests = _xml.Descendants("test-case").Where(e => e.Attribute("name") != null && e.Attribute("name").Value.EndsWith(expectedName, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(),
                String.Format("Expected test not found in suite {0}, expected test named {1}", _name, expectedName));

            var name = tests.Single().Attribute("name").Value;

            Expect.That(() => name.StartsWith(_fullName));

            return new TestResult(tests.Single());
        }

        public ISuiteResult withCategories(params string[] categories)
        {
            return base.withCategories<SuiteResult>(categories);
        }

        public ISuiteResult hasSuite(string name)
        {
            return FixtureResult.FindSuite(_xml, _fullName, name);
        }
    }
}