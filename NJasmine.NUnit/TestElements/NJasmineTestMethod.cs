using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        public TestPosition Position { get
            {
                return _testContext.Position;
            } 
        }

        readonly Func<SpecificationFixture> _fixtureFactory;
        readonly TestContext _testContext;

        public NJasmineTestMethod(Func<SpecificationFixture> fixtureFactory, TestContext testContext)
            : base(new Action(delegate() { }).Method)
        {
            _fixtureFactory = fixtureFactory;
            _testContext = testContext;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            return RunTest(listener);
        }

        TestResult RunTest(EventListener listener)
        {
            listener.TestStarted(base.TestName);
            
            var testResult = RunTest();

            var nunitTestResult = new TestResult(this);

            NativeTestResult.ApplyToNunitResult(testResult, nunitTestResult);

            listener.TestFinished(nunitTestResult);

            return nunitTestResult;
        }

        TestResultShim RunTest()
        {
            var startTime = DateTime.UtcNow;
            var testResult = new TestResultShim();

            Exception existingError = _testContext.GlobalSetupManager.PrepareForTestPosition(Position);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, _testContext.Name.MultilineName, existingError, null,
                                     TestResultShim.Site.SetUp);
            }
            else
            {
                List<string> traceMessages = null;
                try
                {
                    SpecificationRunner.RunTestMethodWithoutGlobalSetup(_fixtureFactory, _testContext.GlobalSetupManager, Position,
                                                                        out traceMessages);
                    testResult.Success();
                }
                catch (Exception e)
                {
                    var globalTraceMessages = _testContext.GlobalSetupManager.GetTraceMessages();
                    TestResultUtil.Error(testResult, _testContext.Name.MultilineName, e,
                                         globalTraceMessages.Concat(traceMessages));
                }
            }

            testResult.SetExecutionTime(DateTime.UtcNow - startTime);
            return testResult;
        }
    }
}
