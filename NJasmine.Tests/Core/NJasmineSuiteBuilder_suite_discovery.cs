using System;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

 
namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineSuiteBuilder_suite_discovery : PowerAssertFixture
    {
        [Test]
        public void doesnt_handle_most_test_fixtures()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(() => !sut.CanBuildFrom(typeof(Object)));
        }

        public class SomeNestedClass : GivenWhenThenFixture
        {
            public override void Specify() { }
        }


        [Test]
        public void will_handle_subclasses_of_NJasmineFixture()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(() => sut.CanBuildFrom(typeof(SomeNestedClass)));
            expect(() => sut.CanBuildFrom(typeof(SampleTest)));
        }

        public abstract class SomeAbstractClass : GivenWhenThenFixture
        {
        }

        [Test]
        public void will_not_build_abstract_classes()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(() => !sut.CanBuildFrom(typeof(SomeAbstractClass)));
        }

        protected class ANonpublicFixture : GivenWhenThenFixture
        {
            public override void Specify()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void will_not_handle_nonpublic_subclasses_of_NJasmineFixture()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(() => !sut.CanBuildFrom(typeof(ANonpublicFixture)));
        }
    }
}