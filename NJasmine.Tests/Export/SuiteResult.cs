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

        public ISuiteResult hasTest(string expectedName, Action<ITestResult> handler)
        {
            var tests = GetTestsWithName(s => s.EndsWith(expectedName, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(),
                String.Format("Expected test not found in suite {0}, expected test named {1}", _name, expectedName));

            var name = tests.Single().Attribute("name").Value;

            var testResult = new TestResult(tests.Single());

            handler(testResult);

            return this;
        }

        IEnumerable<XElement> GetTestsWithName(Func<string, bool> matchDetector)
        {
            IEnumerable<XElement> tests =
                _xml.Descendants("test-case").Where(
                    e =>
                    e.Attribute("name") != null &&
                    e.Attribute("name").Value.StartsWith(_fullName) &&
                    matchDetector(e.Attribute("name").Value));

            return tests;
        }

        public ISuiteResult withCategories(params string[] categories)
        {
            return base.withCategories<SuiteResult>(categories);
        }

        public ISuiteResult hasSuite(string name)
        {
            return NUnitFixtureResult.FindSuite(_xml, _fullName, name);
        }

        public ISuiteResult doesNotHaveTestContaining(string skipped)
        {
            IEnumerable<XElement> matchingTests = GetTestsWithName(s => s.Contains(skipped));

            Assert.IsEmpty(matchingTests, String.Format("Expected not to have tests containing {0}, found: {1}", skipped, string.Join(", ", matchingTests.Select(t => t.Attribute("name").Value))));
            
            return this;
        }
    }
}