using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;
using NJasmine.Core.Execution;

namespace NJasmine.Core
{
    public class SpecificationRunner
    {
        public static void RunTestMethodWithoutGlobalSetup(Func<SpecificationFixture> fixtureFactory, IGlobalSetupManager globalSetupManager, TestPosition targetTestPosition, List<string> traceMessages = null)
        {
            traceMessages = traceMessages ?? new List<string>();

            var executionContext = new NJasmineTestRunContext(targetTestPosition, globalSetupManager, traceMessages);
            var runner = new NJasmineTestRunner(executionContext);

            SpecificationFixture fixture = fixtureFactory();

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
        }

        public static void RunTestMethodWithoutGlobalSetup2(Func<SpecificationFixture> fixtureFactory, IGlobalSetupManager globalSetupManager, TestPosition targetTestPosition, List<string> traceMessages = null)
        {
            traceMessages = traceMessages ??  new List<string>();
            
            var testContext = new TestContext()
            {
                GlobalSetupManager = globalSetupManager,
                Name = new TestName(),
                Position = targetTestPosition
            };

            RunTest(testContext, fixtureFactory, traceMessages);
        }

        public static TestResultShim RunTest(TestContext testContext, Func<SpecificationFixture> fixtureFactory, List<string> traceMessages)
        {
            var startTime = DateTime.UtcNow;
            var testResult = new TestResultShim();

            Exception existingError = testContext.GlobalSetupManager.PrepareForTestPosition(testContext.Position);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, testContext.Name.MultilineName, existingError, null,
                                     TestResultShim.Site.SetUp);
            }
            else
            {
                traceMessages.AddRange(testContext.GlobalSetupManager.GetTraceMessages());
                try
                {
                    RunTestMethodWithoutGlobalSetup(fixtureFactory, testContext.GlobalSetupManager, testContext.Position, traceMessages);
                    testResult.Success();
                }
                catch (Exception e)
                {
                    TestResultUtil.Error(testResult, testContext.Name.MultilineName, e, traceMessages);
                }
            }

            testResult.SetExecutionTime(DateTime.UtcNow - startTime);
            return testResult;
        }
    }
}
