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
        public static void RunTestMethodWithoutGlobalSetup(Func<SpecificationFixture> fixtureFactory, IGlobalSetupManager globalSetupManager, TestPosition targetTestPosition, out List<string> traceMessages)
        {
            traceMessages = new List<string>();

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

        public static TestResultShim RunTest(TestContext testContext, Func<SpecificationFixture> fixtureFactory)
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
                List<string> traceMessages = null;
                try
                {
                    RunTestMethodWithoutGlobalSetup(fixtureFactory, testContext.GlobalSetupManager, testContext.Position,
                                                                        out traceMessages);
                    testResult.Success();
                }
                catch (Exception e)
                {
                    var globalTraceMessages = testContext.GlobalSetupManager.GetTraceMessages();
                    TestResultUtil.Error(testResult, testContext.Name.MultilineName, e,
                                         globalTraceMessages.Concat(traceMessages));
                }
            }

            testResult.SetExecutionTime(DateTime.UtcNow - startTime);
            return testResult;
        }
    }
}
