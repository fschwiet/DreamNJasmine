using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.ImportNUnitFixture
{
    public class NUnitFixtureCollectionTests : NJasmineFixture
    {
        public override void Tests()
        {

describe("NUnitFixtureCollection", delegate
{
    var sut = new NUnitFixtureCollection();

    describe("a collection of fixtures", delegate
    {
        var position = new TestPosition();
        var type = typeof(SomeFixtureTypeA);

        it("can add and retrive the fixture instance at a position", delegate
        {
            sut.AddFixture(position, type);

            var instance1 = sut.GetInstance(position);
            var instance2 = sut.GetInstance(position);

            expect(instance1).to.Be.OfType(type);
            expect(instance1).to.Be.SameAs(instance2);
        });

        it("can only add one fixture per position", delegate
        {
            sut.AddFixture(position, type);

            try
            {
                sut.AddFixture(position, type);
            }
            catch (InvalidOperationException e)
            {
                return;
            }
            
            Assert.Fail("exception expected");
        });

        it("can retrieve the fixture from parent", delegate
        {
            var parent = new NUnitFixtureCollection();
            parent.AddFixture(position, type);

            sut = new NUnitFixtureCollection(parent);

            expect(sut.GetInstance(position)).to.Be.OfType(type);
        });
    });

    describe("can run one-time fixture setup and teardown", delegate
    {
        beforeEach(delegate
        {
            Observed = new StringBuilder();
        });

        it("for an empty collection", delegate
        {
            sut.DoOnetimeSetUp();
            expect(Observed.ToString()).to.Equal("");

            Observed = new StringBuilder();

            sut.DoOnetimeTearDown();
            expect(Observed.ToString()).to.Equal("");
        });

        it("for one", delegate
        {
            sut.AddFixture(new TestPosition(0), typeof(SomeFixtureTypeA));
            sut.DoOnetimeSetUp();

            expect(Observed.ToString()).to.Equal("A.TestFixtureSetUp ");

            Observed = new StringBuilder();

            sut.DoOnetimeTearDown();
            expect(Observed.ToString()).to.Equal("A.TestFixtureTearDown ");
        });

        it("and more", delegate
        {
            sut.AddFixture(new TestPosition(0), typeof(SomeFixtureTypeA));
            sut.AddFixture(new TestPosition(1), typeof(SomeFixtureTypeB));
            sut.AddFixture(new TestPosition(2), typeof(SomeFixtureTypeC));
            sut.DoOnetimeSetUp();

            expect(Observed.ToString()).to.Equal("A.TestFixtureSetUp B.TestFixtureSetUp C.TestFixtureSetUp ");

            Observed = new StringBuilder();

            sut.DoOnetimeTearDown();
            expect(Observed.ToString()).to.Equal("C.TestFixtureTearDown B.TestFixtureTearDown A.TestFixtureTearDown ");
        });
    });

    describe("can run per-test fixture setup and teardown", delegate
    {
        beforeEach(delegate
        {
            Observed = new StringBuilder();

            sut.AddFixture(new TestPosition(1, 2), typeof(SomeFixtureTypeB));
            sut.AddFixture(new TestPosition(1, 2, 3), typeof(SomeFixtureTypeC));
        });

        it("doing setup", delegate
        {
            sut.DoSetUp(new TestPosition(1, 2, 3));

            expect(Observed.ToString()).to.Equal("C.SetUp ");
        });

        it("doing teardown", delegate
        {
            sut.DoTearDown(new TestPosition(1, 2, 3));

            expect(Observed.ToString()).to.Equal("C.TearDown ");
        });

        /*
        it("for an empty collection", delegate
        {
            sut.DoSetUp(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("");

            Observed = new StringBuilder();

            sut.DoTearDown(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("");
        });

        it("for a single fixture, in scope", delegate
        {
            sut.AddFixture(new TestPosition(0), typeof(SomeFixtureTypeA));

            sut.DoSetUp(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("A.SetUp ");

            Observed = new StringBuilder();

            sut.DoTearDown(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("A.TearDown ");
        });

        it("for a single fixture, not in scope", delegate
        {
            sut.AddFixture(new TestPosition(2, 1), typeof(SomeFixtureTypeA));

            sut.DoSetUp(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("");

            Observed = new StringBuilder();

            sut.DoTearDown(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("");
        });

        it("for a mix of fixtures, some in scope.", delegate
        {
            sut.AddFixture(new TestPosition(0), typeof(SomeFixtureTypeA));  // in scope
            sut.AddFixture(new TestPosition(3, 1), typeof(SomeFixtureTypeB));  // not in scope
            sut.AddFixture(new TestPosition(1), typeof(SomeFixtureTypeC));  // in scope

            sut.DoSetUp(new TestPosition(2));
            expect(Observed.ToString()).to.Equal("A.SetUp C.SetUp ");

            Observed = new StringBuilder();

            sut.DoTearDown(new TestPosition(1));
            expect(Observed.ToString()).to.Equal("C.TearDown A.TearDown ");
        });

        it("includes parent filter", delegate
        {
            var parent = new NUnitFixtureCollection();
            parent.AddFixture(new TestPosition(0), typeof(SomeFixtureTypeA));  // in scope
            parent.AddFixture(new TestPosition(3, 1), typeof(SomeFixtureTypeB));  // not in scope
            parent.AddFixture(new TestPosition(1), typeof(SomeFixtureTypeC));  // in scope

            sut = new NUnitFixtureCollection(parent);
            sut.AddFixture(new TestPosition(1, 0), typeof(SomeFixtureTypeD));

            sut.DoSetUp(new TestPosition(1,1));
            expect(Observed.ToString()).to.Equal("A.SetUp C.SetUp D.SetUp ");

            Observed = new StringBuilder();

            sut.DoTearDown(new TestPosition(1,1));
            expect(Observed.ToString()).to.Equal("D.TearDown C.TearDown A.TearDown "); 
        });
         
         */
    });
});

        }

        class SomeFixtureTypeA
        {
            public char ShortName { get { return this.GetType().ToString().Last(); }}

            [TestFixtureSetUp]
            public void TestFixtureSetUp() { Observed.Append(ShortName + ".TestFixtureSetUp "); }

            [TestFixtureTearDown]
            public void TestFixtureTearDown() { Observed.Append(ShortName + ".TestFixtureTearDown "); }

            [SetUp]
            public void SetUp() { Observed.Append(ShortName + ".SetUp "); }

            [TearDown]
            public void TearDown() { Observed.Append(ShortName + ".TearDown "); }
        }

        class SomeFixtureTypeB : SomeFixtureTypeA { }

        class SomeFixtureTypeC : SomeFixtureTypeA { }

        static StringBuilder Observed;
    }
}
