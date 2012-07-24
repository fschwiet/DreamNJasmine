using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Discovery;
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
                    SpecificationRunner.RunTestMethodWithoutGlobalSetup(_fixtureFactory, _globalSetup, Position, out traceMessages);
                    testResult.Success();
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
    }
}
