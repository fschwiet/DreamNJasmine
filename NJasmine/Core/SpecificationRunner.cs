using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
