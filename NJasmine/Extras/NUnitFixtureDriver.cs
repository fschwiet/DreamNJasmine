using System;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Extras
{
    public class NUnitFixtureDriver
    {
        public static T IncludeFixture<T>(GivenWhenThenFixture specBuilder) where T : new()
        {
            T fixtureDuringDiscovery = default(T);
            T fixture = default(T);

            fixture = specBuilder.beforeAll(delegate
            {
                fixtureDuringDiscovery = new T();
                RunMethodsWithAttribute(fixtureDuringDiscovery, NUnitFramework.FixtureSetUpAttribute);
                return fixtureDuringDiscovery;
            });

            specBuilder.arrange(delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.SetUpAttribute);
                return fixtureDuringDiscovery;
            });

            specBuilder.afterEach(delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.TearDownAttribute);
            });

            specBuilder.afterAll(delegate
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