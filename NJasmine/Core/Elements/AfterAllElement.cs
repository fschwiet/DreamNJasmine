using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class AfterAllElement : SpecificationElement
    {
        public readonly Action Action;

        public AfterAllElement(ActualKeyword actualKeyword, Action action)
            : base(actualKeyword)
        {
            Action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitAfterAll(this, position);
            return ElementResultUnused;
        }
    }
}
