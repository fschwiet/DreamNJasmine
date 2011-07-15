﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class FixtureResult
    {
        private readonly string _name;
        private readonly string _xmlOutput;
        private readonly string _consoleOutput;
        private XDocument _doc;

        public FixtureResult(string name, string xmlOutput = null, string consoleOutput = "")
        {
            _name = name;
            _xmlOutput = xmlOutput ?? GetSampleXmlResult();
            _consoleOutput = consoleOutput;
            _doc = XDocument.Parse(xmlOutput);
        }

        public FixtureResult succeeds()
        {
            int totalCount = (int)_doc.Root.Attribute("total");
            int errorCount = GetErrorCount();
            int failureCount = GetFailureCount();

            Assert.AreEqual(0, errorCount, _name + " had errors.");
            Assert.AreEqual(0, failureCount, _name + " had failures.");
            Assert.AreNotEqual(0, totalCount, _name + " didn't have any tests.");

            return this;
        }

        public FixtureResult failed()
        {
            Assert.AreNotEqual(0, GetErrorCount() + GetFailureCount(), _name + " didn't have errors.");

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
            
            Assert.That(trace, Is.EquivalentTo(expectedTrace.Split(new [] {'\n', '\r'}, StringSplitOptions.RemoveEmptyEntries)), "Did not find expected trace in " + _name);
        }

        public TestResult hasTest(string name)
        {
            var tests = _doc.Descendants("test-case").Where(e => e.Attribute("name") != null && e.Attribute("name").Value.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            Assert.AreEqual(1, tests.Count(), "Expected test not found, expected test named " + name);

            return new TestResult();
        }

        public SuiteResult hasSuite(string name)
        {
            /*
            for-xml -exactlyOnce ""//test-suite[@name='given an outer block']"" {
                Assert ((get-xml ""@result"") -eq 'Inconclusive') 'Expected first outer block to be inconclusive';

                for-xml ""./results"" {
                    Assert $false 'Expected first outer block to not have any subresults'
                }
            }
            */

            throw new NotImplementedException();
        }

        public void hasStackTracesThat(Func<string, bool> expectation)
        {
            /*
                update-xml $xmlFile {                    $stackTrace = get-xml -exactlyonce ""//stack-trace"";                    assert (-not ($stackTrace -like '*NJasmine.Core*')) 'Expected NJasmine.Core stack elements to be removed';            */
            throw new NotImplementedException();
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
            string aTestName = "NJasmineTests.Core.build_and_run_suite_with_loops.can_load_tests")
        {
            var result = @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
<!--This file represents the results of running a test suite-->
<test-results name=""C:\src\NJasmine\build\NJasmine.tests.dll"" total=""$totalCount"" errors=""$errorCount"" failures=""$failureCount"" not-run=""2"" inconclusive=""0"" ignored=""0"" skipped=""0"" invalid=""2"" date=""2011-07-13"" time=""21:33:22"">
  <environment nunit-version=""2.5.9.10348"" clr-version=""2.0.50727.5446"" os-version=""Microsoft Windows NT 6.1.7601 Service Pack 1"" platform=""Win32NT"" cwd=""C:\src\NJasmine"" machine-name=""NZNZNZ6"" user=""user"" user-domain=""nznznz6"" />
  <culture-info current-culture=""en-US"" current-uiculture=""en-US"" />
  <test-suite type=""Assembly"" name=""C:\src\NJasmine\build\NJasmine.tests.dll"" executed=""True"" result=""Success"" success=""True"" time=""0.805"" asserts=""0"">
    <results>
      <test-suite type=""Namespace"" name=""NJasmineTests"" executed=""True"" result=""Success"" success=""True"" time=""0.783"" asserts=""0"">
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
    </results>
  </test-suite>
</test-results>
 ";
            result = result
                .Replace("$errorCount", errorCount.ToString())
                .Replace("$failureCount", failureCount.ToString())
                .Replace("$totalCount", totalCount.ToString())
                .Replace("$aTestName", aTestName);

            return result;
        }
    }
}
