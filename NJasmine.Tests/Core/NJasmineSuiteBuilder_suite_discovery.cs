using System;
using NJasmine;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.NUnit;
using NUnit.Framework;

 
namespace NJasmineTests.Core
{
    public class FixtureClassifierSpecification : GivenWhenThenFixture
    {
        public override void Specify()
        {
            describe("FixtureClassifier detects classes that are specifications", () =>
            {
                it("excludes non specification classes", () =>
                {
                    expect(() => !FixtureClassifier.IsTypeSpecification(typeof(Object)));
                });

                it("includes classes derived from GivenWhenThenFixture", () =>
                {
                    expect(() => FixtureClassifier.IsTypeSpecification(typeof(FixtureClassifierSpecification.SomeNestedClass)));
                    expect(() => FixtureClassifier.IsTypeSpecification(typeof(SampleTest)));
                });

                describe("some classes derived from GivenWhenThenFixture still are not specifications", () =>
                {
                    it("excludes abstract classes", () =>
                    {
                        expect(() => !FixtureClassifier.IsTypeSpecification(typeof(FixtureClassifierSpecification.SomeAbstractClass)));
                    });

                    it("excludes non-public classes", () =>
                    {
                        expect(() => !FixtureClassifier.IsTypeSpecification(typeof(FixtureClassifierSpecification.ANonpublicFixture)));
                    });
                });
            });
        }

        class SomeNestedClass : GivenWhenThenFixture
        {
            public override void Specify() { }
        }

        abstract class SomeAbstractClass : GivenWhenThenFixture
        {
        }

        class ANonpublicFixture : GivenWhenThenFixture
        {
            public override void Specify()
            {
                throw new NotImplementedException();
            }
        }
    }
}