using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;
using NUnit.Core.Extensibility;

namespace NJasmine.Core
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
            
            var rootSuite = NJasmineTestSuite.CreateRootNJasmineSuite(fixtureFactory, type);

            NUnitFramework.ApplyCommonAttributes(type.GetCustomAttributes(false).Cast<Attribute>().ToArray(), rootSuite);

            return rootSuite;
        }

        public static void VisitAllTestElements<TFixture>(Action<INJasmineTest> visitor)
        {
            var rootTest = new NJasmineSuiteBuilder().BuildFrom(typeof(TFixture));

            VisitAllTestElements(rootTest, visitor);
        }

        static void VisitAllTestElements(ITest test, Action<INJasmineTest> visitor)
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

        static public Dictionary<TestPosition, INJasmineTest> LoadElementsByPosition<TFixture>()
        {
            var result = new Dictionary<TestPosition, INJasmineTest>();
            Action<INJasmineTest> visitor = t => result[t.Position] = t;

            VisitAllTestElements<TFixture>(visitor);

            return result;
        }
    }
}
