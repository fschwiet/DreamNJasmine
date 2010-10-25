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

            var firstTest = rootTest.Tests[0] as ITest;
            var firstDescribe = rootTest.Tests[1] as ITest;
            var firstInnerTest = firstDescribe.Tests[0] as ITest;
            var secondInnerTest = firstDescribe.Tests[1] as ITest;
            var secondDescribe = firstDescribe.Tests[2] as ITest;
            var firstInnerInnerTest = secondDescribe.Tests[0] as ITest;
            var secondInnerInnerTest = secondDescribe.Tests[1] as ITest;

            Action<ITest, string> expectHasName = delegate(ITest test, string name)
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

        [Test]
        public void can_load_tests_with_correct_locations()
        {
            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(typeof(TestOnlyUsingDescribeAndIt));

            var firstTest = rootTest.Tests[0] as INJasmineTest;
            var firstDescribe = rootTest.Tests[1] as INJasmineTest;
            var firstInnerTest = firstDescribe.Tests[0] as INJasmineTest;
            var secondInnerTest = firstDescribe.Tests[1] as INJasmineTest;
            var secondDescribe = firstDescribe.Tests[2] as INJasmineTest;
            var firstInnerInnerTest = secondDescribe.Tests[0] as INJasmineTest;
            var secondInnerInnerTest = secondDescribe.Tests[1] as INJasmineTest;

            Action<INJasmineTest, TestPosition> expectHasPosition = delegate(INJasmineTest test, TestPosition position)
            {
                expect(test.Position.Coordinates).to.Equal(position.Coordinates);
            };

            expectHasPosition(firstTest, new TestPosition(0));
            expectHasPosition(firstDescribe, new TestPosition(1));
            expectHasPosition(firstInnerTest, new TestPosition(1,0));
            expectHasPosition(secondInnerTest, new TestPosition(1,1));
            expectHasPosition(secondDescribe, new TestPosition(1,2));
            expectHasPosition(firstInnerInnerTest, new TestPosition(1,2,0));
            expectHasPosition(secondInnerInnerTest, new TestPosition(1,2,1));
        }
    }
}
