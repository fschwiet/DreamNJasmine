using System;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Extras
{
    public class NUnitFixtureDriver
    {
        public static T IncludeFixture<T>(SkeleFixture specificationBuilder) where T : new()
        {
            T fixtureDuringDiscovery = default(T);
            T fixture = default(T);

            fixture = specificationBuilder.Visitor.visitBeforeAll(SpecElement.importNUnit, delegate
            {
                fixtureDuringDiscovery = new T();
                RunMethodsWithAttribute(fixtureDuringDiscovery, NUnitFramework.FixtureSetUpAttribute);
                return fixtureDuringDiscovery;
            });

            specificationBuilder.Visitor.visitBeforeEach(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.SetUpAttribute);
                return fixtureDuringDiscovery;
            });

            specificationBuilder.Visitor.visitAfterEach(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.TearDownAttribute);
            });

            specificationBuilder.Visitor.visitAfterAll(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixtureDuringDiscovery, NUnitFramework.FixtureTearDownAttribute);
            });

            return fixture;
        }

        static void RunMethodsWithAttribute(object instance, string attribute)
        {
            var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                     attribute, true);

            foreach (var method in methods)
            {
                method.Invoke(instance, EmptyObjectArray);
            }
        }

        readonly static object[] EmptyObjectArray = new object[0];
    }
}