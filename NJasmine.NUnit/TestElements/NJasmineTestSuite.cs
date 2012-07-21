using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestSuite
    {
        readonly SharedContext _sharedContext;
        readonly TestContext _testContext;

        public NJasmineTestSuite(SharedContext sharedContext, TestContext testContext)
        {
            _sharedContext = sharedContext;
            _testContext = testContext;
        }

        public TestBuilder RunSuiteAction(Action action, bool isOuterScopeOfSpecification, TestBuilder resultBuilder)
        {
            var builder = new DiscoveryVisitor(resultBuilder, _sharedContext, _testContext.GlobalSetupManager);

            var exception = _sharedContext.RunActionWithVisitor(_testContext.Position.GetFirstChildPosition(), action, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(v => resultBuilder.AddChildTest(v));
            }
            else
            {
                var testContext = new TestContext()
                {
                    Name = _sharedContext.NameReservations.GetReservedNameLike(resultBuilder.Name),
                    Position = _testContext.Position,
                    GlobalSetupManager = _testContext.GlobalSetupManager
                };

                var failingSuiteAsTest = new TestBuilder(_sharedContext.NativeTestFactory.ForFailingSuite(testContext, exception), testContext.Name);

                if (isOuterScopeOfSpecification)
                {
                    resultBuilder.AddChildTest(failingSuiteAsTest);
                }
                else
                {
                    return failingSuiteAsTest;
                }
            }
            return resultBuilder;
        }
    }
}
