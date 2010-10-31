using System;
using System.Linq;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.Integration
{
    public class imports_NUnit_fixture : ObservableNJasmineFixture
    {
        [Explicit, TestFixture]
        class some_Nunit_fixture_a
        {
            string _fixtureName;

            public some_Nunit_fixture_a()
            {
                Type type = this.GetType();
                _fixtureName = GetTypeShortName(type);
            }

            public static string GetTypeShortName(Type type)
            {
                return type.ToString().Split(new char[] {'+', '.'}).Last();
            }

            [TestFixtureSetUp]
            public void FixtureSetup()
            {
                ObservableNJasmineFixture.Trace("FixtureSetup " + _fixtureName);
            }

            [TestFixtureTearDown]
            public void FixtureTearDown()
            {
                ObservableNJasmineFixture.Trace("FixtureTearDown " + _fixtureName);
            }

            [SetUp]
            public void SetUp()
            {
                ObservableNJasmineFixture.Trace("SetUp " + _fixtureName);
            }

            [TearDown]
            public void TearDown()
            {
                ObservableNJasmineFixture.Trace("TearDown " + _fixtureName);
            }
        }

        [Explicit, TestFixture]
        class some_Nunit_fixture_b : some_Nunit_fixture_a
        {
        }

        [Explicit, TestFixture]
        class some_Nunit_fixture_c : some_Nunit_fixture_a
        {
        }
        
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
    }

}
