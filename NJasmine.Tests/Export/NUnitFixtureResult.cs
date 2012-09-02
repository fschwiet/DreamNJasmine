using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NJasmine.Marshalled;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class NUnitFixtureResult : IFixtureResult
    {
        private readonly string _testName;
        private readonly string _consoleOutput;
        private XDocument _doc;

        public NUnitFixtureResult(string testName, string xmlOutput = null, string consoleOutput = "")
        {
            _testName = testName;
            _consoleOutput = consoleOutput;
            _doc = XDocument.Parse(xmlOutput);
        }

        public IFixtureResult succeeds()
        {
            int totalCount = (int)_doc.Root.Attribute("total");
            int errorCount = GetErrorCount();
            int failureCount = GetFailureCount();

            Assert.AreEqual(0, errorCount, _testName + " had errors.");
            Assert.AreEqual(0, failureCount, _testName + " had failures.");
            Assert.AreNotEqual(0, totalCount, _testName + " didn't have any tests.");

            return this;
        }

        public IFixtureResult failed()
        {
            Assert.AreNotEqual(0, GetErrorCount() + GetFailureCount(), _testName + " didn't have errors.");

            return this;
        }

        public void hasTrace(string expectedTrace)
        {
            AssertContainsTrace(this._testName, this._consoleOutput, expectedTrace);
        }

        public ITestResult hasTest(string name)
        {
            return hasTestWithFullName(_testName + ", " + name);
        }

        public ITestResult hasTestWithFullName(string name)
        {
            var tests = _doc.Descendants("test-case").Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(), "Expected test not found, expected test named " + name);

            return new NUnitTestResult(tests.Single());
        }

        public ISuiteResult hasSuite(string name)
        {
            return FindSuite(_doc.Root, _testName, name);
        }

        public static NUnitSuiteResult FindSuite(XElement element, string fixtureName, string name)
        {
            string expectedSuiteName = name;

            IEnumerable<XElement> allSuites = element.Descendants("test-suite");

            var suites = allSuites.Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(expectedSuiteName, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, suites.Count(), 
                "Expected test suite not found, expected suite named '" + expectedSuiteName + "', found:\n"
                + string.Join("\n", allSuites.Select(s => s.Attribute("name").Value)));

            return new NUnitSuiteResult(fixtureName, suites.Single());
        }

        public string[] withStackTraces()
        {
            return _doc.Descendants("stack-trace").Select(s => s.Value).ToArray();
        }

        private int GetErrorCount()
        {
            return (int)_doc.Root.Attribute("errors");
        }

        private int GetFailureCount()
        {
            return (int)_doc.Root.Attribute("failures");
        }

        public static void AssertContainsTrace(string testName, string consoleOutput, string expectedTrace)
        {
            string resetMarker = "{{<<RESET>>}}";
            string tracePattern = @"<<\{\{(.*)}}>>";

            var lastReset = consoleOutput.LastIndexOf(resetMarker);

            if (lastReset < 0)
                lastReset = 0;
            else
                lastReset = lastReset + resetMarker.Length;

            MatchCollection matches = new Regex(tracePattern).Matches(consoleOutput, lastReset);

            var trace = matches.OfType<Match>().Select(m => m.Groups[1].Value).ToArray();

            Assert.That(trace, Is.EquivalentTo(expectedTrace.Split(new[] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)),
                "Did not find expected trace in " + testName);
        }
    }
}
