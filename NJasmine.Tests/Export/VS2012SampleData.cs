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
<TestRun id='6845f341-9fa0-418c-b166-3746876b23e4' name='user@NZNZNZ6 2012-09-01 11:31:08' runUser='nznznz6\user' xmlns='http://microsoft.com/schemas/VisualStudio/TeamTest/2010'>
  <Times creation='2012-09-01T11:31:08.2730406-07:00' queuing='2012-09-01T11:31:08.2740407-07:00' start='2012-09-01T11:31:08.2780409-07:00' finish='2012-09-01T11:31:08.3280438-07:00' />
  <TestSettings name='default' id='989b80fc-378d-41c9-a7f3-a4d52214ebc7'>
    <Execution>
      <TestTypeSpecific />
    </Execution>
    <Deployment runDeploymentRoot='user_NZNZNZ6 2012-09-01 11_31_08' />
  </TestSettings>
  <Results>
    <UnitTestResult executionId='41878d87-8115-4db2-bf21-1179dfac85fe' testId='668cee77-1f18-495f-8601-8133039e3755' testName='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' computerName='NZNZNZ6' startTime='2012-09-01T11:31:08.3140430-07:00' endTime='2012-09-01T11:31:08.3140430-07:00' testType='13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b' outcome='Failed' testListId='8c84fa94-04c1-424b-9868-57a2d4851a1d' relativeResultsDirectory='41878d87-8115-4db2-bf21-1179dfac85fe'>
      <Output>
        <ErrorInfo>
          <Message>System.Exception : IsTrue failed, expression was:

False</Message>
          <StackTrace>$aStackTrace
</StackTrace>
        </ErrorInfo>
      </Output>
    </UnitTestResult>
  </Results>
  <TestDefinitions>
    <UnitTest name='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' storage='c:\src\njasmine\bin\njasmine.tests.dll' id='668cee77-1f18-495f-8601-8133039e3755'>
      <Execution id='41878d87-8115-4db2-bf21-1179dfac85fe' />
      <TestMethod codeBase='C:\src\NJasmine\bin\NJasmine.Tests.dll' adapterTypeName='Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter' className='NamespaceIsntNJasmineTests' name='NamespaceIsntNJasmineTests.stacktrace_has_NJasmine_internal_calls_removed, given some context, when some action, then it fails' />
    </UnitTest>
  </TestDefinitions>
  <TestEntries>
    <TestEntry testId='668cee77-1f18-495f-8601-8133039e3755' executionId='41878d87-8115-4db2-bf21-1179dfac85fe' testListId='8c84fa94-04c1-424b-9868-57a2d4851a1d' />
  </TestEntries>
  <TestLists>
    <TestList name='Results Not in a List' id='8c84fa94-04c1-424b-9868-57a2d4851a1d' />
    <TestList name='All Loaded Results' id='19431567-8539-422a-85d7-44ee4e166bda' />
  </TestLists>
  <ResultSummary outcome='Failed'>
    <Counters total='$totalCount' executed='1' passed='0' failed='$failureCount' error='$errorCount' timeout='0' aborted='0' inconclusive='0' passedButRunAborted='0' notRunnable='0' notExecuted='0' disconnected='0' warning='0' completed='0' inProgress='0' pending='0' />
  </ResultSummary>
</TestRun>
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
