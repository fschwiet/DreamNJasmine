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
        public class ExecutionContext : IDisposable
        {
            public GlobalSetupManager SetupManager;

            public void Dispose()
            {
                if (SetupManager != null)
                {
                    SetupManager.Close();
                    SetupManager = null;
                }
            }
        }

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

            var result = TestBuilder.BuildSuiteForTextContext(sharedContext, testContext, sharedContext.GetSpecificationRootAction(), true, explicitReason);

            nativeTestFactory.SetRoot(result);

            return setupManager;
        }
    }
}
