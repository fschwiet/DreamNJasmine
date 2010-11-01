using System;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.Integration
{
    [Explicit, TestFixture]
    public class some_Nunit_fixture_a
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            imports_NUnit_fixture.Trace("FixtureSetup " + ObservableNJasmineFixture.GetTypeShortName(this.GetType()));
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            imports_NUnit_fixture.Trace("FixtureTearDown " + ObservableNJasmineFixture.GetTypeShortName(this.GetType()));
        }

        [SetUp]
        public void SetUp()
        {
            imports_NUnit_fixture.Trace("SetUp " + ObservableNJasmineFixture.GetTypeShortName(this.GetType()));
        }

        [TearDown]
        public void TearDown()
        {
            imports_NUnit_fixture.Trace("TearDown " + ObservableNJasmineFixture.GetTypeShortName(this.GetType()));
        }
    }

    [Explicit, TestFixture]
    public class some_Nunit_fixture_b : some_Nunit_fixture_a
    {
    }

    [Explicit, TestFixture]
    public class some_Nunit_fixture_c : some_Nunit_fixture_a
    {
    }

    public class imports_NUnit_fixture : ObservableNJasmineFixture
    {
        public override void Tests()
        {
            Trace("test started, before include of a");
            var firstA = importNUnit<some_Nunit_fixture_a>();
            Trace("after include of a");

            describe("a block", () =>
            {
                Trace("first describe, before include of b");
                var firstB = importNUnit<some_Nunit_fixture_b>();
                Trace("after include of b");

                it("check fixtures", () =>
                {
                    Trace("first test");
                });

                describe("a sub block", () =>
                {
                    Trace("before include of c");
                    var secondA = importNUnit<some_Nunit_fixture_c>();
                    Trace("after include of c");

                    it("check fixtures again", () =>
                    {
                        Trace("second test test");
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
