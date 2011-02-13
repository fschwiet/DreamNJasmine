using System;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Extras
{
    public class NUnitFixtureDriver
    {
        public static T IncludeFixture<T>(ISpecVisitor specVisitor) where T : new()
        {
            T createdFixture = default(T);

            T fixture = specVisitor.visitBeforeAll(SpecElement.importNUnit, delegate
            {
                createdFixture = new T();
                RunMethodsWithAttribute(createdFixture, NUnitFramework.FixtureSetUpAttribute);
                return createdFixture;
            });

            specVisitor.visitBeforeEach(SpecElement.importNUnit, null, delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.SetUpAttribute);
                return fixture;
            });

            specVisitor.visitAfterEach(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(fixture, NUnitFramework.TearDownAttribute);
            });

            specVisitor.visitAfterAll(SpecElement.importNUnit, delegate
            {
                RunMethodsWithAttribute(createdFixture, NUnitFramework.FixtureTearDownAttribute);
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