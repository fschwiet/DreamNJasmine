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

        public void containsTrace(string expectedTrace)
        {
            string resetMarker = "{{<<RESET>>}}";
            string tracePattern = @"<<\{\{(.*)}}>>";

            var lastReset = _consoleOutput.LastIndexOf(resetMarker);

            if (lastReset < 0)
                lastReset = 0;
            else
                lastReset = lastReset + resetMarker.Length;

            MatchCollection matches = new Regex(tracePattern).Matches(_consoleOutput, lastReset);
            
            var trace = matches.OfType<Match>().Select(m => m.Groups[1].Value).ToArray();
            
            Assert.That(trace, Is.EquivalentTo(expectedTrace.Split(new [] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)), "Did not find expected trace in " + _testName);
        }

        public ITestResult hasTest(string name)
        {
            return hasTestWithFullName(_testName + ", " + name);
        }

        public ITestResult hasTestWithFullName(string name)
        {
            var tests = _doc.Descendants("test-case").Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(), "Expected test not found, expected test named " + name);

            return new TestResult(tests.Single());
        }

        public ISuiteResult hasSuite(string name)
        {
            return FindSuite(_doc.Root, _testName, name);
        }

        public static SuiteResult FindSuite(XElement element, string fixtureName, string name)
        {
            string expectedSuiteName = name;

            IEnumerable<XElement> allSuites = element.Descendants("test-suite");

            var suites = allSuites.Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(expectedSuiteName, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, suites.Count(), 
                "Expected test suite not found, expected suite named '" + expectedSuiteName + "', found:\n"
                + string.Join("\n", allSuites.Select(s => s.Attribute("name").Value)));

            return new SuiteResult(fixtureName, suites.Single());
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

        public static string GetSampleXmlResult(
            int totalCount = 0
            , int errorCount = 0, 
            int failureCount = 0,
            string aSuiteName = "NJasmineTests",
            string aSuiteResult = "Success",
            string aTestName = "NJasmineTests.Core.build_and_run_suite_with_loops.can_load_tests", 
            string aStackTrace = @"SPECIFICATION:
NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations,
when using expect within beforeAll,
fails

at PowerAssert.PAssert.IsTrue(Expression`1 expression)
at NJasmine.GivenWhenThenFixture.expect(Expression`1 expectation) in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 321
at NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations.<Specify>b__3() in c:\src\NJasmine\NJasmine.Tests\Specs\beforeAll\beforeAll_can_use_expectations.cs:line 42
at NJasmine.GivenWhenThenFixture.<>c__DisplayClass13.<beforeAll>b__11() in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 254
")
        {
            var result = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<!--This file represents the results of running a test suite-->
<test-results name=""C:\src\NJasmine\build\NJasmine.tests.dll"" total=""$totalCount"" errors=""$errorCount"" failures=""$failureCount"" not-run=""2"" inconclusive=""0"" ignored=""0"" skipped=""0"" invalid=""2"" date=""2011-07-13"" time=""21:33:22"">
  <environment nunit-version=""2.5.9.10348"" clr-version=""2.0.50727.5446"" os-version=""Microsoft Windows NT 6.1.7601 Service Pack 1"" platform=""Win32NT"" cwd=""C:\src\NJasmine"" machine-name=""NZNZNZ6"" user=""user"" user-domain=""nznznz6"" />
  <culture-info current-culture=""en-US"" current-uiculture=""en-US"" />
  <test-suite type=""Assembly"" name=""C:\src\NJasmine\build\NJasmine.tests.dll"" executed=""True"" result=""Success"" success=""True"" time=""0.805"" asserts=""0"">
    <results>
      <test-suite type=""Namespace"" name=""$aSuiteName"" executed=""True"" result=""$aSuiteResult"" success=""True"" time=""0.783"" asserts=""0"">
        <results>
          <test-suite type=""Namespace"" name=""Core"" executed=""True"" result=""Success"" success=""True"" time=""0.491"" asserts=""0"">
            <results>
              <test-suite type=""TestFixture"" name=""build_and_run_suite_with_loops"" executed=""True"" result=""Success"" success=""True"" time=""0.271"" asserts=""0"">
                <results>
                  <test-case name=""$aTestName"" executed=""True"" result=""Success"" success=""True"" time=""0.222"" asserts=""0"" />
                  <test-case name=""NJasmineTests.Core.build_and_run_suite_with_loops.can_run_tests_a1"" executed=""True"" result=""Success"" success=""True"" time=""0.019"" asserts=""1"" />
                  <test-case name=""NJasmineTests.Core.build_and_run_suite_with_loops.can_run_tests_a3"" executed=""True"" result=""Success"" success=""True"" time=""0.001"" asserts=""1"" />
                </results>
              </test-suite>
            </results>
          </test-suite>
        </results>
      </test-suite>
        <test-case name=""NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations, when using expect within beforeAll, fails"" executed=""True"" result=""Error"" success=""False"" time=""0.060"" asserts=""0"">
        <properties>
          <property name=""MultilineName"" value=""NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations,&#xA;when using expect within beforeAll,&#xA;fails"" />
        </properties>
        <failure>
          <message><![CDATA[SetUp : System.Exception : IsTrue failed, expression was:

False]]></message>
                              <stack-trace><![CDATA[$aStackTrace]]></stack-trace>
        </failure>
      </test-case>

    </results>
  </test-suite>
</test-results>
 ";
            result = result
                .Replace("$errorCount", errorCount.ToString())
                .Replace("$failureCount", failureCount.ToString())
                .Replace("$totalCount", totalCount.ToString())
                .Replace("$aSuiteName", aSuiteName)
                .Replace("$aSuiteResult", aSuiteResult)
                .Replace("$aTestName", aTestName)
                .Replace("$aStackTrace", aStackTrace);

            return result;
        }
    }
}
