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
    }
}