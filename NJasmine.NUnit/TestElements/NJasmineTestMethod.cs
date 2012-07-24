using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Execution;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        public TestPosition Position { get; private set; }

        readonly Func<SpecificationFixture> _fixtureFactory;
        readonly IGlobalSetupManager _globalSetup;

        public NJasmineTestMethod(Func<SpecificationFixture> fixtureFactory, TestPosition position, IGlobalSetupManager globalSetup)
            : base(new Action(delegate() { }).Method)
        {
            _fixtureFactory = fixtureFactory;
            _globalSetup = globalSetup;
            Position = position;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            listener.TestStarted(base.TestName);
            long ticks = DateTime.Now.Ticks;
            var testResult = new TestResultShim();

            Exception existingError = _globalSetup.PrepareForTestPosition(Position);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, TestExtensions.GetMultilineName(this), existingError, null, TestResultShim.Site.SetUp);
            }
            else
            {
                List<string> traceMessages = null;
                try
                {
                    RunTestMethodInner(this, testResult, out traceMessages);
                }
                catch (Exception e)
                {
                    var globalTraceMessages = _globalSetup.GetTraceMessages();
                    TestResultUtil.Error(testResult, TestExtensions.GetMultilineName(this), e, globalTraceMessages.Concat(traceMessages));
                }
            }

            var nunitTestResult = new TestResult(this);
            NativeTestResult.ApplyToNunitResult(testResult, nunitTestResult);
            nunitTestResult.Time = ((DateTime.Now.Ticks - ticks)) / 10000000.0;
            listener.TestFinished(nunitTestResult);
            return nunitTestResult;
        }

        public static void RunTestMethodInner(NJasmineTestMethod nJasmineTestMethod, TestResultShim testResult, out List<string> traceMessages)
        {
            traceMessages = new List<string>();

            var executionContext = new NJasmineTestRunContext(nJasmineTestMethod.Position, nJasmineTestMethod._globalSetup, traceMessages);
            var runner = new NJasmineTestRunner(executionContext);

            SpecificationFixture fixture = nJasmineTestMethod._fixtureFactory();

            fixture.CurrentPosition = TestPosition.At(0);
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
