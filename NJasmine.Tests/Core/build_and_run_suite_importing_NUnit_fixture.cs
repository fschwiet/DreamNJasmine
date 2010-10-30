using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    public class build_and_run_suite_importing_NUnit_fixture
    {
        [Explicit, TestFixture]
        public class some_Nunit_fixture_a
        {
            [TestFixtureSetUp]
            public void FixtureSetup()
            {
                imports_NUnit_fixture.Trace("FixtureSetup " + this.GetType());
            }

            [TestFixtureTearDown]
            public void FixtureTearDown()
            {
                imports_NUnit_fixture.Trace("FixtureTearDown" + this.GetType());
            }

            [SetUp]
            public void SetUp()
            {
                imports_NUnit_fixture.Trace("SetUp" + this.GetType());
            }

            [TearDown]
            public void TearDown()
            {
                imports_NUnit_fixture.Trace("TearDown" + this.GetType());
            }
        }

        [Explicit, TestFixture]
        public class some_Nunit_fixture_b : some_Nunit_fixture_a
        {
        }

        public class imports_NUnit_fixture : ObservableNJasmineFixture
        {
            public override void Tests()
            {
                Trace("test started, before first include of a");
                var firstA = importNUnit<some_Nunit_fixture_a>();
                Trace("after first include of a");

                describe("a block", () =>
                {
                    Trace("first describe, before include of b");
                    var firstB = importNUnit<some_Nunit_fixture_b>();
                    Trace("after include of b");

                    it("check fixtures", () =>
                    {
                        Trace("first test");
                        //expect(firstA.Observations).to.Equal(new List<string>()
                        //{
                        //    "FixtureSetup",
                        //    "SetUp"
                        //});

                        //expect(firstB.Observations).to.Equal(new List<string>()
                        //{
                        //    "FixtureSetup",
                        //    "SetUp"
                        //});
                    });

                    describe("a sub block", () =>
                    {
                        Trace("before second a");
                        var secondA = importNUnit<some_Nunit_fixture_a>();
                        Trace("after second a");

                        it("check fixtures again", () =>
                        {
                            Trace("second test test");

                            //expect(firstA.Observations).to.Equal(new List<string>()
                            //{
                            //    "FixtureSetup",
                            //    "SetUp",
                            //    "TearDown",
                            //    "SetUp"
                            //});

                            //expect(firstB.Observations).to.Equal(new List<string>()
                            //{
                            //    "FixtureSetup",
                            //    "SetUp",
                            //    "TearDown",
                            //    "SetUp"
                            //});

                            //expect(secondA.Observations).to.Equal(new List<string>()
                            //{
                            //    "FixtureSetup",
                            //    "SetUp"
                            //});
                        });
                    });
                });
            }

            public static void Trace(string value)
            {
                Console.WriteLine("<<{{" + value + "}}>>");
            }
        }
    }
}
