using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NJasmine.NUnit;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class build_and_run_suite_with_loops : PowerAssertFixture
    {
        public class has_test_in_loop : ObservableNJasmineFixture
        {
            public override void Specify()
            {
                Observe("1");

                foreach(var i in Enumerable.Range(1,3))
                {
                    Observe("a" + i);

                    it("a" + i, () =>
                    {
                        Observe("ai" + i);
                    });
                }

                Observe("2");

                describe("nested", () =>
                {
                    Observe("3");

                    foreach(var i in Enumerable.Range(1,3))
                    {
                        Observe("b" + i);

                        it("b" + i, () =>
                        {
                            Observe("bi" + i);
                        });
                    }

                    Observe("4");
                });

                Observe("5");

                it("c", delegate()
                {
                    Observe("c");
                });
            }
        }

        [Test]
        public void can_load_tests()
        {
            var result = new Dictionary<TestPosition, string>();
            Action<INJasmineTest> visitor = t => result[t.Position] = (t as Test).TestName.Name;

            var rootTest = new NJasmineSuiteBuilder().BuildFrom(typeof(has_test_in_loop));

            NJasmineSuiteBuilder.VisitAllTestElements(rootTest, visitor);
            var elements = result;

            expect(() => elements[new TestPosition(0)] == "a1");
            expect(() => elements[new TestPosition(1)] == "a2");
            expect(() => elements[new TestPosition(2)] == "a3");
            expect(() => elements[new TestPosition(3)] == "nested");
            expect(() => elements[new TestPosition(3, 0)] == "b1");
            expect(() => elements[new TestPosition(3, 1)] == "b2");
            expect(() => elements[new TestPosition(3, 2)] == "b3");
        }

        void expect_test_to_observe(TestPosition testPosition, List<string> expected)
        {
            var fixture = new has_test_in_loop();
            var method = new NJasmineTestMethod(() => fixture, testPosition, new FakeGlobalSetupManager());

            TestResultShim result = new TestResultShim();
            List<string> ignored;
            method.RunTestMethodInner(result, out ignored);

            expect(() => result.IsSuccess);

            Assert.That(fixture.Observations, Is.EquivalentTo(expected));
        }

        [Test]
        public void can_run_tests_a1()
        {
            expect_test_to_observe(new TestPosition(0), new List<string>()
            {
                "1",
                "a1",
                "ai1",
                "a2",
                "a3",
                "2",
                "5"
            });
        }

        [Test]
        public void can_run_tests_a3()
        {
            expect_test_to_observe(new TestPosition(2), new List<string>()
            {
                "1",
                "a1",
                "a2",
                "a3",
                "ai3",
                "2",
                "5"
            });
        }

        [Test]
        public void can_run_tests_b1()
        {
            expect_test_to_observe(new TestPosition(3, 0), new List<string>()
            {
                "1",
                "a1",
                "a2",
                "a3",
                "2",
                "3",
                "b1",
                "bi1",
                "b2",
                "b3",
                "4",
                "5"
            });
        }

        [Test]
        public void can_run_tests_b3()
        {
            expect_test_to_observe(new TestPosition(3, 2), new List<string>()
            {
                "1",
                "a1",
                "a2",
                "a3",
                "2",
                "3",
                "b1",
                "b2",
                "b3",
                "bi3",
                "4",
                "5"
            });
        }
        [Test]
        public void can_run_tests_c()
        {
            expect_test_to_observe(new TestPosition(4), new List<string>()
            {
                "1",
                "a1",
                "a2",
                "a3",
                "2",
                "5",
                "c"
            });
        }

    }
}
