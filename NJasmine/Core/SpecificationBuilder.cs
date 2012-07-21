using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public class SpecificationBuilder
    {
        public static TestBuilder BuildTestFixture(Type type, INativeTestFactory nativeTestFactory)
        {
            var constructor = type.GetConstructor(new Type[0]);

            Func<SpecificationFixture> fixtureFactory = delegate()
            {
                var fixture = constructor.Invoke(new object[0]) as SpecificationFixture;
                return fixture;
            };

            SharedContext buildContext = new SharedContext(nativeTestFactory, fixtureFactory, new NameReservations(),
                                                           fixtureFactory());

            var testContext = new TestContext()
            {
                Position = new TestPosition(),
                GlobalSetupManager = buildContext.GetGlobalSetupManager(),
                Name = new TestName
                {
                    FullName = type.Namespace + "." + type.Name,
                    Shortname = type.Name,
                    MultilineName = type.Namespace + "." + type.Name
                }
            };

            TestSuite rootSuite = new TestSuite(buildContext, testContext);

            var resultBuilder = new TestBuilder(buildContext.NativeTestFactory.ForSuite(testContext, () => testContext.GlobalSetupManager.Cleanup(testContext.Position)), testContext.Name);

            return rootSuite.RunSuiteAction(buildContext.GetSpecificationRootAction(), true, resultBuilder);
        }
    }
}
