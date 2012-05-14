using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.FixtureVisitor
{
    public class SpecificationElement
    {
        readonly SpecElement _element;

        public SpecificationElement(SpecElement element)
        {
            _element = element;
        }

        public SpecElement Value { 
            get { return _element; } 
        }

        public override string ToString()
        {
            return _element.ToString();
        }
    }
}
