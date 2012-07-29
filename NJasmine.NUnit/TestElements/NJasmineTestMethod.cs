using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
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
            return RunTest(listener);
        }

        TestResult RunTest(EventListener listener)
        {
            listener.TestStarted(base.TestName);
            
            var startTime = DateTime.UtcNow;
            var testResult = new TestResultShim();

            Exception existingError = _globalSetup.PrepareForTestPosition(Position);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, TestExtensions.GetMultilineName(this), existingError, null,
                                     TestResultShim.Site.SetUp);
            }
            else
            {
                List<string> traceMessages = null;
                try
                {
                    SpecificationRunner.RunTestMethodWithoutGlobalSetup(_fixtureFactory, _globalSetup, Position,
                                                                        out traceMessages);
                    testResult.Success();
                }
                catch (Exception e)
                {
                    var globalTraceMessages = _globalSetup.GetTraceMessages();
                    TestResultUtil.Error(testResult, TestExtensions.GetMultilineName(this), e,
                                         globalTraceMessages.Concat(traceMessages));
                }
            }

            testResult.SetExecutionTime(DateTime.UtcNow - startTime);

            var nunitTestResult = new TestResult(this);

            NativeTestResult.ApplyToNunitResult(testResult, nunitTestResult);

            listener.TestFinished(nunitTestResult);

            return nunitTestResult;
        }
    }
}
