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

            var setupManager  = new GlobalSetupManager(fixtureFactory);

            FixtureContext fixtureContext = new FixtureContext(nativeTestFactory, fixtureFactory, new NameReservations(), setupManager);
           
            var testContext = new TestContext()
            {
                Position = TestPosition.At(),
                FixtureContext = fixtureContext,
                Name = new TestName
                {
                    FullName = type.Namespace + "." + type.Name,
                    Shortname = type.Name,
                    MultilineName = type.Namespace + "." + type.Name
                }
            };

            var explicitReason = ExplicitAttributeReader.GetFor(type);

            var result = BuildSuiteForTextContext(fixtureContext, testContext, fixtureContext.GetSpecificationRootAction(), true, explicitReason);

            nativeTestFactory.SetRoot(result);

            return setupManager;
        }

        public static INativeTest BuildSuiteForTextContext(FixtureContext fixtureContext, TestContext testContext1, Action invoke, bool isRootSuite, string explicitReason = null)
        {
            var result = fixtureContext.NativeTestFactory.ForSuite(fixtureContext, testContext1);

            if (explicitReason != null)
                result.MarkTestIgnored(explicitReason);

            var builder = new DiscoveryVisitor(result, fixtureContext, testContext1.FixtureContext.GlobalSetupManager);

            var exception = fixtureContext.RunActionWithVisitor(testContext1.Position.GetFirstChildPosition(), invoke, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(result.AddChild);
            }
            else
            {
                var failedTestPositon = isRootSuite
                                            ? testContext1.Position.GetFirestUnexpectedChildPosition()
                                            : testContext1.Position;

                var testContext = new TestContext()
                {
                    Name = fixtureContext.NameReservations.GetReservedNameLike(result.Name),
                    Position = failedTestPositon,
                    FixtureContext = testContext1.FixtureContext
                };

                var failingSuiteAsTest = fixtureContext.NativeTestFactory.ForTest(fixtureContext, testContext);
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
