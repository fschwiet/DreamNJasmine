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

        public void NotifyStart(TestContext testContext)
        {
            _frameworkHandle.RecordStart(_tests[testContext.Name.MultilineName]);
        }

        public void NotifyEnd(TestContext testContext, TestResultShim testResult)
        {
            var test = _tests[testContext.Name.MultilineName];

            var result = new TestResult(test)
            {
                Outcome = TestOutcome.Failed
            };

            _frameworkHandle.RecordStart(test);
            _frameworkHandle.RecordEnd(test, result.Outcome);
            _frameworkHandle.RecordResult(result);
        }
    }
}
