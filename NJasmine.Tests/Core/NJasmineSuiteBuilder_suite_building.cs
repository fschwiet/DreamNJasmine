using System;
using System.Collections.Generic;
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
        public void LoadTestsByPosition(Dictionary<TestPosition, INJasmineTest> tests, ITest test)
        {
            if (test is INJasmineTest)
            {
                tests[(test as INJasmineTest).Position] = test as INJasmineTest;
            }

            if (test is TestSuite)
            {
                foreach (ITest childTest in (test as TestSuite).Tests)
                {
                    LoadTestsByPosition(tests, childTest);
                }
            }
        }

        Dictionary<TestPosition, INJasmineTest> GetTestsByPosition<TFixture>()
        {
            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(typeof(TFixture));

            Dictionary<TestPosition, INJasmineTest> testsByPosition = new Dictionary<TestPosition, INJasmineTest>();

            LoadTestsByPosition(testsByPosition, rootTest);
            return testsByPosition;
        }

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
        public void can_load_tests_with_correct_names_and_positions()
        {
            Dictionary<TestPosition, INJasmineTest> testsByPosition = GetTestsByPosition <TestOnlyUsingDescribeAndIt>();

            Action<TestPosition, string> expectHasName = delegate(TestPosition position, string name)
            {
                expect(testsByPosition[position].TestName.Name).to.Equal(name);
            };

            expectHasName(new TestPosition(0), "first test");
            expectHasName(new TestPosition(1), "first describe");
            expectHasName(new TestPosition(1,0), "first inner test");
            expectHasName(new TestPosition(1,1), "second inner test");
            expectHasName(new TestPosition(1,2), "second describe");
            expectHasName(new TestPosition(1, 2, 0), "first inner-inner test");
            expectHasName(new TestPosition(1, 2, 1), "second inner-inner test");
        }

        [Test]
        public void can_load_test_with_error_in_describe()
        {
            Dictionary<TestPosition, INJasmineTest> testsByPosition = GetTestsByPosition<ExceptionThrownInFirstDescribe>();

            expect(testsByPosition[new TestPosition(1)]).to.Be.OfType<NJasmineInvalidTestSuite>();
        }

        [Test]
        public void can_load_test_with_error_in_outer_scope()
        {
            Dictionary<TestPosition, INJasmineTest> testsByPosition = GetTestsByPosition<ExceptionThrownAtTopLevel>();

            expect(testsByPosition[new TestPosition(0)]).to.Be.OfType<NJasmineInvalidTestSuite>();
        }
    }
}
