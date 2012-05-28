using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.FixtureVisitor
{
    public class TestElement : SpecificationElement
    {
        readonly string _description;
        readonly Action _action;

        public TestElement(SpecElement element) : base(element)
        {
        }

        public TestElement(SpecElement element, string description, Action action) : base(element)
        {
            _description = description;
            _action = action;
        }

        public override void Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitFork(new SpecificationElement(SpecElement.describe), _description, _action, position);
        }
    }
}
