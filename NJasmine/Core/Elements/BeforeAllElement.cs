using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class BeforeAllElement<T> : SpecificationElement
    {
        readonly Func<T> _action;

        public BeforeAllElement(ActualKeyword actualKeyword, Func<T> action)
            : base(actualKeyword)
        {
            _action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            return visitor.visitBeforeAll(this, _action, position);
        }
    }

    public class BeforeAllElementWithoutReturnValue : BeforeAllElement<object>
    {
        public BeforeAllElementWithoutReturnValue(ActualKeyword actualKeyword, Action action)
            : base(actualKeyword, delegate()
            {
                action();
                return ElementResultUnused;
            })
        {
        }
    }
}
