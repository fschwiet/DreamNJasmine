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

            specificationBuilder.ExtendSpec(s => fixture = s.visitBeforeAll(SpecElement.importNUnit, delegate
            {
                fixtureDuringDiscovery = new T();
                RunMethodsWithAttribute(fixtureDuringDiscovery, NUnitFramework.FixtureSetUpAttribute);
                return fixtureDuringDiscovery;
            }));

            specificationBuilder.ExtendSpec(s => s.visitBeforeEach(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.SetUpAttribute);
                return fixtureDuringDiscovery;
            }));

            specificationBuilder.ExtendSpec(s => s.visitAfterEach(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.TearDownAttribute);
            }));

            specificationBuilder.ExtendSpec(s => s.visitAfterAll(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixtureDuringDiscovery, NUnitFramework.FixtureTearDownAttribute);
            }));

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