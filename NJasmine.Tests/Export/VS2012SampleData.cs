using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmineTests.Export
{
    public class VS2012SampleData
    {
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

at PowerAssert.PAssert.IsTrue(Expression`1 expression)l
at NJasmine.GivenWhenThenFixture.expect(Expression`1 expectation) in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 321
at NJasmineTests.Specs.beforeAll.beforeAll_can_use_expectations.<Specify>b__3() in c:\src\NJasmine\NJasmine.Tests\Specs\beforeAll\beforeAll_can_use_expectations.cs:line 42
at NJasmine.GivenWhenThenFixture.<>c__DisplayClass13.<beforeAll>b__11() in c:\src\NJasmine\NJasmine\GivenWhenThenFixture.cs:line 254
")
        {
            var result = @"<?xml version='1.0' encoding='UTF-8'?>
<TestRun id='3b3f1e83-e5b2-4d21-9e60-32459ef35961' name='user@NZNZNZ6 2012-09-01 10:44:22' runUser='nznznz6\user' xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'>
  <Times creation='2012-09-01T10:44:22.3933642-07:00' queuing='2012-09-01T10:44:22.3943642-07:00' start='2012-09-01T10:44:22.3993645-07:00' finish='2012-09-01T10:44:22.4613680-07:00' />
  <TestSettings name='default' id='45bfd059-4ef9-4cc0-8e6b-20574a536812'>
    <Execution>
      <TestTypeSpecific />
    </Execution>
    <Deployment runDeploymentRoot='user_NZNZNZ6 2012-09-01 10_44_22' />
  </TestSettings>
  <Results>
    <UnitTestResult executionId='2d21219e-f85d-4792-98f9-492b7f35f39f' testId='82fe35ec-feca-4d3c-9fed-43013c8f7508' testName='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' computerName='NZNZNZ6' startTime='2012-09-01T10:44:22.4453671-07:00' endTime='2012-09-01T10:44:22.4453671-07:00' testType='13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b' outcome='Failed' testListId='8c84fa94-04c1-424b-9868-57a2d4851a1d' relativeResultsDirectory='2d21219e-f85d-4792-98f9-492b7f35f39f'>
      <Output>
        <ErrorInfo>
          <Message>System.Exception : IsTrue failed, expression was:

False</Message>
          <StackTrace>SPECIFICATION:
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

</StackTrace>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
  </Results>
  <TestDefinitions>
    <UnitTest name='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' storage='c:\src\njasmine\bin\njasmine.tests.dll' id='82fe35ec-feca-4d3c-9fed-43013c8f7508'>
      <Execution id='2d21219e-f85d-4792-98f9-492b7f35f39f' />
      <TestMethod codeBase='C:\src\NJasmine\bin\NJasmine.Tests.dll' adapterTypeName='Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter' className='NamespaceIsntNJasmineTests' name='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' />
    </UnitTest>
  </TestDefinitions>
  <TestEntries>
    <TestEntry testId='82fe35ec-feca-4d3c-9fed-43013c8f7508' executionId='2d21219e-f85d-4792-98f9-492b7f35f39f' testListId='8c84fa94-04c1-424b-9868-57a2d4851a1d' />
  </TestEntries>
  <TestLists>
    <TestList name='Results Not in a List' id='8c84fa94-04c1-424b-9868-57a2d4851a1d' />
    <TestList name='All Loaded Results' id='19431567-8539-422a-85d7-44ee4e166bda' />
  </TestLists>
  <ResultSummary outcome='Failed'>
    <Counters total='1' executed='1' passed='0' failed='1' error='0' timeout='0' aborted='0' inconclusive='0' />
  </ResultSummary>
</TestRun>";
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
