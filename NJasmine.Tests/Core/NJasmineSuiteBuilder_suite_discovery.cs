using System;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineSuiteBuilder_suite_discovery
    {
        [Test]
        public void doesnt_handle_most_test_fixtures()
        {
            var sut = new NJasmineSuiteBuilder();

            sut.CanBuildFrom(typeof (Object)).Should().Equal(false);
        }

        public class SomeNestedClass : GivenWhenThenFixture
        {
            public override void Specify() { }
        }


        [Test]
        public void will_handle_subclasses_of_NJasmineFixture()
        {
            var sut = new NJasmineSuiteBuilder();

            sut.CanBuildFrom(typeof(SomeNestedClass)).Should().Equal(true);
            sut.CanBuildFrom(typeof(SampleTest)).Should().Equal(true);
        }

        public abstract class SomeAbstractClass : GivenWhenThenFixture
        {
        }

        [Test]
        public void will_not_build_abstract_classes()
        {
            var sut = new NJasmineSuiteBuilder();

            sut.CanBuildFrom(typeof(SomeAbstractClass)).Should().Equal(false);
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

            sut.CanBuildFrom(typeof(ANonpublicFixture)).Should().Not.Equal(true);
        }
    }
}