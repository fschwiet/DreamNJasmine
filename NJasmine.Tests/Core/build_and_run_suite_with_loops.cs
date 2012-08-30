using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.NativeWrappers;
using NJasmine.NUnit.TestElements;
using NUnit.Core;
using NUnit.Framework;
using TestContext = NJasmine.Core.Discovery.TestContext;
using TestName = NJasmine.Core.TestName;

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

        public class TrackingTestFactory : INativeTestFactory
        {
            public Dictionary<TestPosition, string> Results = new Dictionary<TestPosition, string>();

            public class NoopTest : INativeTest {
                public NoopTest(TestName name)
                {
                    Name = name;
                }

                public TestName Name { get; private set; }
                public void AddCategory(string category) { }
                public void AddChild(INativeTest test) { }
                public void MarkTestIgnored(string reasonIgnored) { }
                public void MarkTestInvalid(string reason) { }
                public void MarkTestFailed(Exception exception) { }
                public object GetNative() { return null; }
            }

            public void SetRoot(INativeTest test)
            {
            }

            public INativeTest ForSuite(FixtureContext fixtureContext, TestContext testContext)
            {
                Results[testContext.Position] = testContext.Name.Shortname;
                return new NoopTest(testContext.Name);
            }

            public INativeTest ForTest(FixtureContext fixtureContext, TestContext testContext)
            {
                Results[testContext.Position] = testContext.Name.Shortname;
                return new NoopTest(testContext.Name);
            }
        }

        [Test]
        public void can_load_tests()
        {
            Type type = typeof(has_test_in_loop);

            var nativeTestFactory = new TrackingTestFactory();

            using (SpecificationBuilder.BuildTestFixture(type, nativeTestFactory))
            {
                expect(() => nativeTestFactory.Results[TestPosition.At(0)] == "a1");
                expect(() => nativeTestFactory.Results[TestPosition.At(1)] == "a2");
                expect(() => nativeTestFactory.Results[TestPosition.At(2)] == "a3");
                expect(() => nativeTestFactory.Results[TestPosition.At(3)] == "nested");
                expect(() => nativeTestFactory.Results[TestPosition.At(3, 0)] == "b1");
                expect(() => nativeTestFactory.Results[TestPosition.At(3, 1)] == "b2");
                expect(() => nativeTestFactory.Results[TestPosition.At(3, 2)] == "b3");
            }
        }

        void expect_test_to_observe(TestPosition testPosition, List<string> expected)
        {
            var fixture = new has_test_in_loop();

            var traceMessages = new List<string>();

            SpecificationRunner.RunTest(new TestContext()
            {
                GlobalSetupManager = new FakeGlobalSetupManager(),
                Name = new TestName(),
                Position = testPosition
            }, () => fixture, traceMessages);


            Assert.That(fixture.Observations, Is.EquivalentTo(expected));
        }

        [Test]
        public void can_run_tests_a1()
        {
            expect_test_to_observe(TestPosition.At(0), new List<string>()
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
            expect_test_to_observe(TestPosition.At(2), new List<string>()
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
            expect_test_to_observe(TestPosition.At(3, 0), new List<string>()
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
            expect_test_to_observe(TestPosition.At(3, 2), new List<string>()
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
            expect_test_to_observe(TestPosition.At(4), new List<string>()
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
