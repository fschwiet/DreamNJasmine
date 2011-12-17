using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public class TestPosition
    {
        public IEnumerable<int> Coordinates { get; private set; }

        public TestPosition(params int[] coordinates)
        {
            Coordinates = coordinates;
        }

        public TestPosition Parent
        {
            get
            {
                return new TestPosition()
                {
                    Coordinates = Coordinates.Take(Coordinates.Count() - 1)
                };
            }
        }

        public bool IsAncestorOf(TestPosition position)
        {
            if (Coordinates.Count() < position.Coordinates.Count())
            {
                return (position.Coordinates.Take(Coordinates.Count()).SequenceEqual(Coordinates));
            }

            return false;
        }

        public TestPosition GetFirstChildPosition()
        {
            return new TestPosition()
            {
                Coordinates = Coordinates.Concat(new int[] { 0 })
            };
        }

        public TestPosition GetNextSiblingPosition()
        {
            var copy = Coordinates.ToArray();
            
            ++copy[copy.Length - 1];

            return new TestPosition()
            {
                Coordinates = copy
            };
        }

        public bool IsOnPathTo(TestPosition testPosition)
        {
            var thisCoordinateCount = this.Coordinates.Count();

            if (thisCoordinateCount > testPosition.Coordinates.Count())
                return false;

            for(var i = 0; i < thisCoordinateCount; i++)
            {
                int thisCoordinate = this.Coordinates.Skip(i).First();
                int testCoordinate = testPosition.Coordinates.Skip(i).First();

                if (thisCoordinate != testCoordinate)
                {
                    if (i == thisCoordinateCount - 1)
                        return thisCoordinate <= testCoordinate;
                    else
                        return false;
                }
            }

            return true;
        }

        public override string ToString()
        {
            // Note: using StartsWith() to determine if a describe is in scope for a test.
            // So don't add stuff to the end of ToString() result without adjusting NJasmineTestMethod.visitDescribe

            return "TestPosition:" + String.Join(", ", Coordinates.Select(c => c.ToString()).ToArray());
        }

        public bool Equals(TestPosition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.ToString().Equals(this.ToString());
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (TestPosition)) return false;
            return Equals((TestPosition) obj);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}