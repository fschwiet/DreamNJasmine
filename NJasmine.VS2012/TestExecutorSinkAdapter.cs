using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NJasmine.Core;
using NJasmine.Core.Discovery;

namespace NJasmine.VS2012
{
    public class TestExecutorSinkAdapter : MarshalByRefObject, ITestResultListener
    {
        readonly IFrameworkHandle _frameworkHandle;
        readonly Dictionary<string, TestCase> _tests;

        public TestExecutorSinkAdapter(IFrameworkHandle frameworkHandle, IEnumerable<TestCase> tests)
        {
            _frameworkHandle = frameworkHandle;

            _tests = new Dictionary<string, TestCase>();

            foreach(var test in tests)
            {
                _tests[test.FullyQualifiedName] = test;
            }
        }

        public void NotifyStart(string testFullName)
        {
            _frameworkHandle.RecordStart(_tests[testFullName]);
        }

        public void NotifyEnd(string testFullName, TestResultShim testResult)
        {
            var test = _tests[testFullName];

            var result = new TestResult(test)
            {
                Outcome = MapToOutcome(testResult),
                DisplayName = testFullName
            };

            if (result.Outcome == TestOutcome.Failed)
            {
                result.ErrorMessage = testResult.FailureReason;
                result.ErrorStackTrace = testResult.FailureStackTrace;
            }
            else if (result.Outcome == TestOutcome.Skipped)
            {
                // TODO: can we include the reason skipped in VS output somehow?
                result.Messages.Add(
                    new TestResultMessage("ReasonSkipped", testResult.ReasonSkipped));
            }

            _frameworkHandle.RecordEnd(test, result.Outcome);
            _frameworkHandle.RecordResult(result);
        }

        public TestOutcome MapToOutcome(TestResultShim shim)
        {
            switch(shim.Status)
            {
            case TestResultShim.Result.Successs:
                return TestOutcome.Passed;
            case TestResultShim.Result.Error:
                return TestOutcome.Failed;
            case TestResultShim.Result.Skipped:
                return TestOutcome.Skipped;
            default:
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
