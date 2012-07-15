using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class TestElement : SpecificationElement
    {
        public readonly string Description;
        public readonly Action Action;

        public TestElement(ActualKeyword actualKeyword, string description, Action action) : base(actualKeyword)
        {
            Description = description;
            Action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitTest(this, position);
            return ElementResultUnused;
        }
    }
}
