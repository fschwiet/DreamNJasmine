using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class AfterAllElement : SpecificationElement
    {
        readonly Action _action;

        public AfterAllElement(ActualKeyword actualKeyword, Action action)
            : base(actualKeyword)
        {
            _action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitAfterAll(this, _action, position);
            return ElementResultUnused;
        }
    }
}
