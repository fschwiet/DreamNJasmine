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
    public class TestPositionTests : PowerAssertFixture
    {
        [Test]
        public void can_be_used_with_dictionaries()
        {
            var position1 = TestPosition.At(1, 2, 3);
            var position2 = TestPosition.At(4, 5, 6);
            var position3 = TestPosition.At(7, 8, 9);

            expect(() => position1.Equals(TestPosition.At(1, 2, 3)));

            Dictionary<TestPosition, int> dictionary = new Dictionary<TestPosition, int>();

            dictionary[position1] = 1;
            dictionary[position2] = 2;
            dictionary[position3] = 3;

            expect(() => dictionary[TestPosition.At(1,2,3)] == 1);
            expect(() => dictionary[TestPosition.At(4,5,6)] == 2);
            expect(() => dictionary[TestPosition.At(7, 8, 9)] == 3);
        }

        [Test]
        public void test_Parent()
        {
            var position = TestPosition.At(1, 2, 3);

            expect(() => position.Parent.Equals(TestPosition.At(1, 2)));
        }

        [Test]
        public void test_IsAncestorOf()
        {
            var position = TestPosition.At(1, 2, 3);

            expect(() => position.IsAncestorOf(TestPosition.At(1, 2, 3, 4)));
            expect(() => position.IsAncestorOf(TestPosition.At(1, 2, 3, 4, 0, 1, 2)));
            expect(() => !position.IsAncestorOf(TestPosition.At(1, 2)));
            expect(() => !position.IsAncestorOf(TestPosition.At(3, 2, 1, 4)));
        }

        [Test]
        public void GetFirstChildPosition()
        {
            expect(() => TestPosition.At(0).GetFirstChildPosition().Equals(TestPosition.At(0, 0)));
            expect(() => TestPosition.At(3, 1, 0, 10, 93).GetFirstChildPosition().Equals(TestPosition.At(3, 1, 0, 10, 93, 0)));
        }

        [Test]
        public void GetNextSiblingPosition()
        {
            expect(() => TestPosition.At(0).GetNextSiblingPosition().Equals(TestPosition.At(1)));
            expect(() => TestPosition.At(3, 1, 0, 10, 93).GetNextSiblingPosition().Equals(TestPosition.At(3, 1, 0, 10, 94)));
        }

        [Test]
        public void IsInScopeFor()
        {
            expect(() => !TestPosition.At(0).IsOnPathTo(TestPosition.At()));

            expect(() => TestPosition.At(0).IsOnPathTo(TestPosition.At(0, 1)));
            expect(() => TestPosition.At(0).IsOnPathTo(TestPosition.At(1, 2)));
            expect(() => TestPosition.At(0).IsOnPathTo(TestPosition.At(5)));
            expect(() => TestPosition.At(0).IsOnPathTo(TestPosition.At(5, 123)));

            expect(() => !TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 2)));
            expect(() => TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 5)));
            expect(() => TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 5, 0)));
            expect(() => TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 5, 3)));
            expect(() => TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 7)));
            expect(() => TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 7, 0)));
            expect(() => TestPosition.At(0, 5).IsOnPathTo(TestPosition.At(0, 7, 3)));

            expect(() => !TestPosition.At(1, 2, 3).IsOnPathTo(TestPosition.At(0)));
            expect(() => !TestPosition.At(1, 2, 3).IsOnPathTo(TestPosition.At(0, 2, 3)));
            expect(() => !TestPosition.At(1, 2, 3).IsOnPathTo(TestPosition.At(2, 2, 3)));
            expect(() => !TestPosition.At(1, 2, 3).IsOnPathTo(TestPosition.At(1, 0, 3)));
            expect(() => !TestPosition.At(1, 2, 3).IsOnPathTo(TestPosition.At(1, 3, 3)));
            expect(() => !TestPosition.At(1, 2, 3).IsOnPathTo(TestPosition.At(2, 2, 2, 10)));
        }
    }
}
