using System;
using System.Collections.Generic;
using System.Linq;

namespace NJasmine.Core
{
    public class TestPosition
    {
        public IEnumerable<int> Coordinates { get; private set; }

        public TestPosition(params int[] coordinates)
        {
            Coordinates = coordinates;
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

        public override string ToString()
        {
            return String.Join(", ", Coordinates.Select(c => c.ToString()).ToArray());
        }

        public bool IsInScopeFor(TestPosition testPosition)
        {
            var thisCoordinateCount = this.Coordinates.Count();

            if (thisCoordinateCount > testPosition.Coordinates.Count())
                return false;

            for(var i = 0; i < thisCoordinateCount; i++)
            {
                if (this.Coordinates.Skip(i).First() != testPosition.Coordinates.Skip(i).First())
                {
                    if (i == thisCoordinateCount - 1)
                        return true;
                    else
                        return false;
                }
            }

            return true;
        }
    }
}