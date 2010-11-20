using System;
using System.Collections.Generic;
using NJasmine;
using NJasmine.Core;
using NJasmineTests.FailingFixtures;
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
        public void can_load_tests_with_correct_names_and_positions()
        {
            var elements = NJasmineSuiteBuilder.LoadElementsByPosition<TestOnlyUsingDescribeAndIt>();

            Action<TestPosition, string> expectHasName = delegate(TestPosition position, string name)
            {
                expect(elements[position].TestName.Name).to.Equal(name);
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
            var elements = NJasmineSuiteBuilder.LoadElementsByPosition<reports_exception_within_describe>();

            expect(elements[new TestPosition(1)]).to.Be.OfType<NJasmineInvalidTestSuite>();
        }

        [Test]
        public void can_load_test_with_error_in_outer_scope()
        {
            var elements = NJasmineSuiteBuilder.LoadElementsByPosition<reports_exception_at_outermost_scope>();

            expect(elements[new TestPosition()]).to.Be.OfType<NJasmineInvalidTestSuite>();
        }
    }
}
