using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;
using NJasmine.Core.NativeWrappers;

namespace NJasmine.Core
{
    public class SpecificationBuilder
    {
        public static GlobalSetupManager BuildTestFixture(Type type, INativeTestFactory nativeTestFactory)
        {
            if (nativeTestFactory is ValidatingNativeTestFactory)
                throw new InvalidOperationException("Do not pass a ValidatingNativeTestFactory here.");
    
            nativeTestFactory = new ValidatingNativeTestFactory(nativeTestFactory);

            var constructor = type.GetConstructor(new Type[0]);

            Func<SpecificationFixture> fixtureFactory = delegate()
            {
                var fixture = constructor.Invoke(new object[0]) as SpecificationFixture;
                return fixture;
            };

            SharedContext sharedContext = new SharedContext(nativeTestFactory, fixtureFactory, new NameReservations());

            var setupManager  = new GlobalSetupManager(fixtureFactory);

            var testContext = new TestContext()
            {
                Position = TestPosition.At(),
                GlobalSetupManager = setupManager ,
                Name = new TestName
                {
                    FullName = type.Namespace + "." + type.Name,
                    Shortname = type.Name,
                    MultilineName = type.Namespace + "." + type.Name
                }
            };

            var explicitReason = ExplicitAttributeReader.GetFor(type);

            var result = BuildSuiteForTextContext(sharedContext, testContext, sharedContext.GetSpecificationRootAction(), true, explicitReason);

            nativeTestFactory.SetRoot(result);

            return setupManager;
        }

        public static INativeTest BuildSuiteForTextContext(SharedContext sharedContext, TestContext testContext1, Action invoke, bool isRootSuite, string explicitReason = null)
        {
            var result = sharedContext.NativeTestFactory.ForSuite(sharedContext, testContext1);

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
