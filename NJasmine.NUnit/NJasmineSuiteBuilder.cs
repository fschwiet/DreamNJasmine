using System;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;
using NJasmine.NUnit.TestElements;
using NUnit.Core;
using NUnit.Core.Extensibility;
using TestName = NJasmine.Core.TestName;

namespace NJasmine.NUnit
{
    [NUnitAddin(Name = "NJasmine", Description = "A BDD-style test framework inspired by the Jasmine BDD framework for Javascript.", Type = ExtensionType.Core)]
    public class NJasmineSuiteBuilder : IAddin, ISuiteBuilder
    {
        public bool Install(IExtensionHost host)
        {
            host.GetExtensionPoint("SuiteBuilders").Install(this);
            return true;
        }

        public bool CanBuildFrom(Type type)
        {
            return FixtureClassifier.IsTypeSpecification(type);
        }

        public Test BuildFrom(Type type)
        {
            NativeTestFactory nativeTestFactory = new NativeTestFactory();

            var constructor = type.GetConstructor(new Type[0]);

            Func<SpecificationFixture> fixtureFactory = delegate()
            {
                var fixture = constructor.Invoke(new object[0]) as SpecificationFixture;
                return fixture;
            };

            SharedContext buildContext = new SharedContext(nativeTestFactory, fixtureFactory, new NameReservations(), fixtureFactory());

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

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(buildContext, testContext);

            TestName name = new TestName
            {
                FullName = type.Namespace + "." + type.Name,
                Shortname = type.Name,
                MultilineName = type.Namespace + "." + type.Name
            };

            TestBuilder root = rootSuite.RunSuiteAction(buildContext.GetSpecificationRootAction(), true, new TestBuilder(buildContext.NativeTestFactory.ForSuite(testContext, () => testContext.GlobalSetupManager.Cleanup(testContext.Position)), testContext.Name));

            var result = (root.GetUnderlyingTest() as NativeTest).GetNative(root);

            NUnitFramework.ApplyCommonAttributes(type.GetCustomAttributes(false).Cast<Attribute>().ToArray(), result);

            return result;
        }

        public static void VisitAllTestElements(ITest test, Action<INJasmineTest> visitor)
        {
            if (test is INJasmineTest)
            {
                visitor(test as INJasmineTest);
            }

            if (test is TestSuite)
            {
                foreach (ITest childTest in (test as TestSuite).Tests)
                {
                    VisitAllTestElements(childTest, visitor);
                }
            }
        }
    }
}
