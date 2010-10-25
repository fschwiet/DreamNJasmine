using System;
using NJasmine;
using NJasmine.Core;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineSuiteBuilderTests : ExpectationsFixture
    {
        [Test]
        public void doesnt_handle_most_test_fixtures()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(sut.CanBuildFrom(typeof (Object))).to.Equal(false);
        }

        [Test]
        public void will_handle_subclasses_of_NJasmineFixture()
        {
            var sut = new NJasmineSuiteBuilder();

            expect(sut.CanBuildFrom(typeof(SampleTest))).to.Equal(true);
        }

        [Test]
        public void can_load_tests_with_correct_names()
        {
            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(typeof (TestOnlyUsingDescribeAndIt));

            var firstTest = rootTest.Tests[0] as Test;
            var firstDescribe = rootTest.Tests[1] as Test;
            var firstInnerTest = firstDescribe.Tests[0] as Test;
            var secondInnerTest = firstDescribe.Tests[1] as Test;
            var secondDescribe = firstDescribe.Tests[2] as Test;
            var firstInnerInnerTest = secondDescribe.Tests[0] as Test;
            var secondInnerInnerTest = secondDescribe.Tests[1] as Test;

            Action<Test, string> expectHasName = delegate(Test test, string name)
            {
                expect(test.TestName.Name).to.Equal(name);
            };

            expectHasName(rootTest, "TestOnlyUsingDescribeAndIt");
            expectHasName(firstTest, "first test");
            expectHasName(firstDescribe, "first describe");
            expectHasName(firstInnerTest, "first inner test");
            expectHasName(secondInnerTest, "second inner test");
            expectHasName(secondDescribe, "second describe");
            expectHasName(firstInnerInnerTest, "first inner-inner test");
            expectHasName(secondInnerInnerTest, "second inner-inner test");
        }

    }
}
