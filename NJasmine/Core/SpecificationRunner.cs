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
        public static TestResultShim RunTest(TestContext testContext, List<string> traceMessages)
        {
            var startTime = DateTime.UtcNow;
            var testResult = new TestResultShim();

            Exception existingError = testContext.FixtureContext.GlobalSetupManager.PrepareForTestPosition(testContext.Position);

            if (existingError != null)
            {
                TestResultUtil.Error(testResult, testContext.Name.MultilineName, existingError, null,
                                     TestResultShim.Site.SetUp);
            }
            else
            {
                traceMessages.AddRange(testContext.FixtureContext.GlobalSetupManager.GetTraceMessages());
                try
                {
                    List<string> traceMessages1 = traceMessages;
                    traceMessages1 = traceMessages1 ?? new List<string>();

                    var executionContext = new NJasmineTestRunContext(testContext.Position, testContext.FixtureContext.GlobalSetupManager, traceMessages1);
                    var runner = new NJasmineTestRunner(executionContext);

                    SpecificationFixture fixture = testContext.FixtureContext.FixtureFactory();

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
