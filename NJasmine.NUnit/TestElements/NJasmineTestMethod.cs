using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Execution;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestMethod : NJasmineNUnitTestMethod, INJasmineTest
    {
        readonly Func<SpecificationFixture> _fixtureFactory;
        readonly IGlobalSetupManager _globalSetup;

        public NJasmineTestMethod(Func<SpecificationFixture> fixtureFactory, TestPosition position, IGlobalSetupManager globalSetup)
            : base(new Action(delegate() { }).Method, position)
        {
            _fixtureFactory = fixtureFactory;
            _globalSetup = globalSetup;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            listener.TestStarted(base.TestName);
            long ticks = DateTime.Now.Ticks;
            var testResult = new TestResultShim();

            Exception existingError = null;

            _globalSetup.PrepareForTestPosition(Position, out existingError);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, TestExtensions.GetMultilineName(this), existingError, null, TestResultShim.Site.SetUp);
            }
            else
            {
                List<string> traceMessages = null;
                try
                {
                    RunTestMethod(testResult, out traceMessages);
                }
                catch (Exception e)
                {
                    var globalTraceMessages = _globalSetup.GetTraceMessages();
                    TestResultUtil.Error(testResult, TestExtensions.GetMultilineName(this), e, globalTraceMessages.Concat(traceMessages));
                }
            }

            var nunitTestResult = new TestResult(this);
            testResult.ApplyToNunitResult(nunitTestResult);
            nunitTestResult.Time = ((DateTime.Now.Ticks - ticks)) / 10000000.0;
            listener.TestFinished(nunitTestResult);
            return nunitTestResult;
        }

        public void RunTestMethod(TestResultShim testResult, out List<string> traceMessages)
        {
            RunTestMethodInner(testResult, out traceMessages);
        }

        public void RunTestMethodInner(TestResultShim testResult, out List<string> traceMessages)
        {
            traceMessages = new List<string>();

            var executionContext = new NJasmineTestRunContext(Position, _globalSetup, traceMessages);
            var runner = new NJasmineTestRunner(executionContext);

            SpecificationFixture fixture = this._fixtureFactory();

            fixture.CurrentPosition = new TestPosition(0);
            fixture.Visitor = runner;
            try
            {
                fixture.Run();
            }
            finally
            {
                executionContext.RunAllPerTestTeardowns();
            }
            testResult.Success();
        }
    }
}
