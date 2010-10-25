using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class TestPositionTests : ExpectationsFixture
    {
        public void expectPositionsEqual(TestPosition a, TestPosition b)
        {
            expect(a.Coordinates).to.Equal(b.Coordinates);
        }

        [Test]
        public void GetFirstChildPosition()
        {
            expectPositionsEqual(new TestPosition(0).GetFirstChildPosition(), new TestPosition(0, 0));
            expectPositionsEqual(new TestPosition(3, 1, 0, 10, 93).GetFirstChildPosition(), new TestPosition(3, 1, 0, 10, 93, 0));
        }

        [Test]
        public void GetNextSiblingPosition()
        {
            expectPositionsEqual(new TestPosition(0).GetNextSiblingPosition(), new TestPosition(1));
            expectPositionsEqual(new TestPosition(3, 1, 0, 10, 93).GetNextSiblingPosition(), new TestPosition(3, 1, 0, 10, 94));
        }

        [Test]
        public void IsInScopeFor()
        {
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(0,1))).to.Equal(true);
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(1, 2))).to.Equal(true);
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(5))).to.Equal(true);
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(5,123))).to.Equal(true);

            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 2))).to.Equal(true);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 7))).to.Equal(true);

            expect(new TestPosition(1,2,3).IsInScopeFor(new TestPosition(0))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(0, 2, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(2, 2, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(1, 0, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(1, 3, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(2, 2, 2, 10))).to.Equal(false);
        }
    }
}
