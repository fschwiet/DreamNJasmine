using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core.Discovery;
using NJasmine.Core.NativeWrappers;


namespace NJasmine.Core
{
    public class TestBuilder
    {
        public static INativeTest BuildSuiteForTextContext(SharedContext sharedContext, TestContext testContext1, Action invoke, bool isRootSuite, string explicitReason = null)
        {
            var result = sharedContext.NativeTestFactory.ForSuite(testContext1);

            if (explicitReason != null)
                result.MarkTestIgnored(explicitReason);

            var builder = new DiscoveryVisitor(result, sharedContext, testContext1.GlobalSetupManager);

            var exception = sharedContext.RunActionWithVisitor(testContext1.Position.GetFirstChildPosition(), invoke, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(result.AddChild);
            }
            else
            {
                var testContext = new TestContext()
                {
                    Name = sharedContext.NameReservations.GetReservedNameLike(result.Name),
                    Position = testContext1.Position,
                    GlobalSetupManager = testContext1.GlobalSetupManager
                };

                var failingSuiteAsTest = sharedContext.NativeTestFactory.ForTest(sharedContext, testContext);
                failingSuiteAsTest.MarkTestFailed(exception);

                if (isRootSuite)
                {
                    result.AddChild(failingSuiteAsTest);
                }
                else
                {
                    return failingSuiteAsTest;
                }
            }

            return result;
        }
    }
}
