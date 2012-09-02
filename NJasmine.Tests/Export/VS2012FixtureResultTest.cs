using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Export
{
    public class VS2012FixtureResultTest : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a TRX file with only successes", () =>
            {
                var results = a_fixture_result_with_1_success();

                then("verification of succeeds() passes", () => results.succeeds());
                then("verification of failed() fails", () => Assert.Throws<Exception>(() => results.failed()));
            });

            given("a TRX file that only has failures", () =>
            {
                var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='1' passed='0' failed='1' error='0' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>";

                var results = arrange(() => new VS2012FixtureResult(null, result, null));

                then("verification of succeeds() fails", () => Assert.Throws<Exception>(() => results.succeeds()));
                then("verification of failed() succeeds", () => results.failed());
            });

            given("a TRX file that hasn't ran any tests", () =>
            {
                var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='0' passed='0' failed='0' error='0' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>";

                var results = arrange(() => new VS2012FixtureResult(null, result, null));

                then("verification of succeeds() fails", () => Assert.Throws<Exception>(() => results.succeeds()));
                then("verification of failed() fails", () => Assert.Throws<Exception>(() => results.failed()));
            });

            given("console output that has trace expressions", () =>
            {
                var result = @"<?xml version='1.0' encoding='UTF-8'?><TestRun xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'></TestRun>";
                var consoleResult = @"Microsoft (R) Test Execution Command Line Tool Version 11.0.50727.1
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test discovery, please wait...
<<{{test started, before include of a}}>>
<<{{after include of a}}>>";

                var results = arrange(() => new VS2012FixtureResult("testName", result, consoleResult));

                then("succeed when checking for existing trace statements", () => results.hasTrace("test started, before include of a\nafter include of a"));
                then("fail when checking for non-existing trace statements", () => Assert.Throws<AssertionException>(() => results.hasTrace("zippity bop")));
            });

            given("a fixture result without stacktraces", () =>
            {
                var results = a_fixture_result_with_1_success();

                then("withStackTraces() returns empty", () => expect(() => results.withStackTraces().Count() == 0));
            });

            given("a fixture result with a stracktrace", () =>
            {
                var stackTrace = @"SPECIFICATION:
    NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed,
    given some context,
    when some action,
    then it fails

Using odd namespace so callstack is filtered.

at PowerAssert.PAssert.IsTrue(Expression`1 expression)
at Expect.cs:11  NJasmine.Extras.Expect.That(Expression`1 expectation) in c:\src\NJasmine\NJasmine\Extras\Expect.cs:line 11
at GivenWhenThenFixture.cs:186  NJasmine.GivenWhenThenFixture.expect(Expression`1 expectation) in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 186
at stacktrace_has_NJasmine_internal_calls_removed.cs:24  NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed.&lt;Specify&gt;b__2() in c:\src\NJasmine\NJasmine.Tests\Specs\report_test_failures_usefully\stacktrace_has_NJasmine_internal_calls_removed.cs:line 24
at GivenWhenThenFixture.cs:50  NJasmine.GivenWhenThenFixture.then(String description, Action test) in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 50
at stacktrace_has_NJasmine_internal_calls_removed.cs:22  NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed.&lt;Specify&gt;b__1() in c:\src\NJasmine\NJasmine.Tests\Specs\report_test_failures_usefully\stacktrace_has_NJasmine_internal_calls_removed.cs:line 22
at GivenWhenThenFixture.cs:40  NJasmine.GivenWhenThenFixture.when(String description, Action specification) in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 40
at stacktrace_has_NJasmine_internal_calls_removed.cs:20  NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed.&lt;Specify&gt;b__0() in c:\src\NJasmine\NJasmine.Tests\Specs\report_test_failures_usefully\stacktrace_has_NJasmine_internal_calls_removed.cs:line 20
at GivenWhenThenFixture.cs:30  NJasmine.GivenWhenThenFixture.given(String description, Action specification) in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 30
at stacktrace_has_NJasmine_internal_calls_removed.cs:18  NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed.Specify() in c:\src\NJasmine\NJasmine.Tests\Specs\report_test_failures_usefully\stacktrace_has_NJasmine_internal_calls_removed.cs:line 18
";
                var trxOutput = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun id='e26bf0df-66b5-41f8-929c-39ae8e54e752' name='user@NZNZNZ7 2012-09-02 14:09:18' runUser='NZNZNZ7\user' xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'>
  <Results>
    <UnitTestResult executionId='7efd3326-9935-4570-873c-6727d8000433' testId='9265fd75-d9d2-45d6-8565-fb155797df38' testName='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' computerName='NZNZNZ7' startTime='2012-09-02T14:09:18.6021888-07:00' endTime='2012-09-02T14:09:18.6021888-07:00' testType='13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b' outcome='Failed' testListId='8c84fa94-04c1-424b-9868-57a2d4851a1d' relativeResultsDirectory='7efd3326-9935-4570-873c-6727d8000433'>
      <Output>
        <ErrorInfo>
          <StackTrace>" + stackTrace + @"</StackTrace>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
  </Results>
</TestRun>";

                then("withStackTraces() returns the strack trace", () =>
                {
                    var results = new VS2012FixtureResult("someName", trxOutput, null);

                    expect(
                        () =>
                        results.withStackTraces().Single().Split('\r', '\n').Any(
                            s => s.Trim() == "Using odd namespace so callstack is filtered."));
                });
            });
        }

        private VS2012FixtureResult a_fixture_result_with_1_success()
        {
            var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='1' passed='1' failed='0' error='0' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>";
            var results = arrange(() => new VS2012FixtureResult(null, result, null));
            return results;
        }
    }
}
