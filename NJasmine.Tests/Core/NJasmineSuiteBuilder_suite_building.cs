using System;
using NJasmine;
using NJasmine.Core;
using NJasmineTests.FailingFixtures;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class NJasmineSuiteBuilder_suite_building : ExpectationsFixture
    {
        public class TestOnlyUsingDescribeAndIt : NJasmineFixture
        {
            public override void Tests()
            {
                it("first test", delegate() { });

                describe("first describe", delegate()
                {
                    it("first inner test", delegate() { });

                    it("second inner test", delegate() { });

                    describe("second describe", delegate()
                    {
                        it("first inner-inner test", delegate() { });

                        it("second inner-inner test", delegate() { });
                    });
                });
            }
        }

        [Test]
        public void suite_has_name()
        {
            Type suiteType = typeof(TestOnlyUsingDescribeAndIt);

            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(suiteType);

            expect(rootTest.TestName.Name).to.Equal(suiteType.Name);
            expect(rootTest.TestName.FullName).to.Equal(suiteType.Namespace + "." + suiteType.Name);
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


        [Test]
        public void can_load_test_with_error_in_describe()
        {
            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(typeof(FailingFixtures.ExceptionThrownInFirstDescribe));

            var brokenDescribe = rootTest.Tests[1] as NJasmineInvalidTestSuite;
        }

        [Test]
        public void can_load_test_with_error_in_outer_scope()
        {
            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(typeof(FailingFixtures.ExceptionThrownAtTopLevel));

            var broken = rootTest as NJasmineInvalidTestSuite;
        }
    }
}
