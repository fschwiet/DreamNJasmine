using System;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;
using NUnit.Core.Extensibility;

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
            if (!type.IsSubclassOf(typeof(SpecificationFixture)))
                return false;

            if (!(type.IsPublic || type.IsNestedPublic))
                return false;

            if (type.GetConstructor(new Type[0]) == null)  // expression really can be false, don't believe Resharper
                return false;

            return true;
        }

        public Test BuildFrom(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);

            Func<SpecificationFixture> fixtureFactory = delegate()
            {
                var fixture = constructor.Invoke(new object[0]) as SpecificationFixture;
                return fixture;
            };

            FixtureDiscoveryContext buildContext = new FixtureDiscoveryContext(fixtureFactory, new NameGenerator(), fixtureFactory());

            var globalSetup = new GlobalSetupManager();

            globalSetup.Initialize(fixtureFactory);

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(new TestPosition(), globalSetup);

            TestBuilder root = rootSuite.BuildNJasmineTestSuite(type.Namespace, type.Name, buildContext, globalSetup, buildContext.GetSpecificationRootAction(), true);

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
