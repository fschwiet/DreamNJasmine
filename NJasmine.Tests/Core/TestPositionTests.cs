using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Core;
using NUnit.Framework;
using Should.Fluent;

namespace NJasmineTests.Core
{
    [TestFixture]
    public class TestPositionTests
    {
        public void expectPositionsEqual(TestPosition a, TestPosition b)
        {
            a.Coordinates.Should().Equal(b.Coordinates);
        }

        [Test]
        public void can_be_used_with_dictionaries()
        {
            var position1 = new TestPosition(1, 2, 3);
            var position2 = new TestPosition(4, 5, 6);
            var position3 = new TestPosition(7, 8, 9);

            position1.Equals(new TestPosition(1, 2, 3)).Should().Equal(true);

            Dictionary<TestPosition, int> dictionary = new Dictionary<TestPosition, int>();

            dictionary[position1] = 1;
            dictionary[position2] = 2;
            dictionary[position3] = 3;

            dictionary[new TestPosition(1,2,3)].Should().Equal(1);
            dictionary[new TestPosition(4,5,6)].Should().Equal(2);
            dictionary[new TestPosition(7,8,9)].Should().Equal(3);
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

        /*
        [Test]
        public void IsInScopeFor()
        {
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(0, 1))).to.Equal(true);
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(1, 2))).to.Equal(true);
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(5))).to.Equal(true);
            expect(new TestPosition(0).IsInScopeFor(new TestPosition(5,123))).to.Equal(true);

            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 2))).to.Equal(false);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 5))).to.Equal(true);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 5, 0))).to.Equal(true);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 5, 3))).to.Equal(true);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 7))).to.Equal(true);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 7, 0))).to.Equal(true);
            expect(new TestPosition(0, 5).IsInScopeFor(new TestPosition(0, 7, 3))).to.Equal(true);

            expect(new TestPosition(1,2,3).IsInScopeFor(new TestPosition(0))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(0, 2, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(2, 2, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(1, 0, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(1, 3, 3))).to.Equal(false);
            expect(new TestPosition(1, 2, 3).IsInScopeFor(new TestPosition(2, 2, 2, 10))).to.Equal(false);
        }
         */
    }
}
