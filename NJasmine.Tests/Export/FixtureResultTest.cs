using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    [Explicit]
    public class FixtureResultTest : GivenWhenThenFixture
    {
        Type expectedAssertionType = typeof(AssertionException);

        public override void Specify()
        {
            describe("succeeds", delegate
            {
                it("allows a passing test result", delegate
                {
                    new FixtureResult("hello", GetSampleXmlResult(1)).succeeds();
                });

                var cases = new Dictionary<string, TestDelegate>();
                
                cases.Add("running against error", delegate
                {
                    var result = new FixtureResult("hello", GetSampleXmlResult(1, 1));
                    result.succeeds();
                });

                cases.Add("running against failure", delegate
                {
                    var result = new FixtureResult("hello", GetSampleXmlResult(1, 0, 1));
                    result.succeeds();
                });

                cases.Add("running against no tests", delegate
                {
                    var result = new FixtureResult("hello", GetSampleXmlResult(0));
                    result.succeeds();
                });

                CheckScenariosCauseErrorWithMessageContaining(cases, "hello");
            });

            describe("failed", delegate
            {
                it("allows test results with errors or failures", delegate
                {
                    new FixtureResult("hello", GetSampleXmlResult(1, 1)).failed();
                    new FixtureResult("hello", GetSampleXmlResult(1, 0, 1)).failed();
                });

                var cases = new Dictionary<string, TestDelegate>();

                cases.Add("running against no tests", delegate
                {
                    var result = new FixtureResult("hello", GetSampleXmlResult(0));
                    result.failed();
                });

                CheckScenariosCauseErrorWithMessageContaining(cases, "hello");
            });
        }

        private void CheckScenariosCauseErrorWithMessageContaining(Dictionary<string, TestDelegate> cases, string expected)
        {
            foreach (var scenario in cases)
            {
                it("asserts when " + scenario.Key, delegate
                {
                    var exception = Assert.Throws(expectedAssertionType, scenario.Value);

                    Assert.That(exception.Message, Is.StringContaining(expected));
                });
            }
        }

        static string GetSampleXmlResult(int totalCount = 0, int errorCount = 0, int failureCount = 0)
        {
            var result =
                @"<?xml version=""1.0"" encoding=""utf-8"" standalone=""no""?>
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
                  <test-case name=""NJasmineTests.Core.build_and_run_suite_with_loops.can_load_tests"" executed=""True"" result=""Success"" success=""True"" time=""0.222"" asserts=""0"" />
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
                .Replace("$totalCount", totalCount.ToString());

            return result;
        }
    }
}
