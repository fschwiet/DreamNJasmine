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

            SpecificationBuilder.BuildTestFixture(type, nativeTestFactory);

            var result = nativeTestFactory.RootTest.GetNative() as Test;

            NUnitFramework.ApplyCommonAttributes(type.GetCustomAttributes(false).Cast<Attribute>().ToArray(), result);

            return result;
        }
    }
}
