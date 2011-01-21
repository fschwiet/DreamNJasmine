using System;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineSuiteBuilder_suite_discovery : ExpectationsFixture
    {
        [Test]
        public void doesnt_handle_most_test_fixtures()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(sut.CanBuildFrom(typeof (Object))).to.Equal(false);
        }

        public class SomeNestedClass : NJasmineFixture
        {
            public override void Specify() { }
        }


        [Test]
        public void will_handle_subclasses_of_NJasmineFixture()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(sut.CanBuildFrom(typeof(SomeNestedClass))).to.Equal(true);
            expect(sut.CanBuildFrom(typeof(SampleTest))).to.Equal(true);
        }

        public abstract class SomeAbstractClass : NJasmineFixture
        {
        }

        [Test]
        public void will_not_build_abstract_classes()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(sut.CanBuildFrom(typeof(SomeAbstractClass))).to.Equal(false);
        }

        protected class ANonpublicFixture : NJasmineFixture
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

            expect(sut.CanBuildFrom(typeof(ANonpublicFixture))).not.to.Equal(true);
        }
    }
}