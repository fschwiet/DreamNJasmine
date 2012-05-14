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

        public override string ToString()
        {
            return _element.ToString();
        }
    }

    class ForkElement : SpecificationElement
    {
        readonly public string Description;
        readonly public Action Action;

        public ForkElement(SpecElement element, string description, Action action) : base(element)
        {
            Description = description;
            Action = action;
        }

        public void Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitFork(new SpecificationElement(SpecElement.describe), Description, Action, position);
        }
    }
}
