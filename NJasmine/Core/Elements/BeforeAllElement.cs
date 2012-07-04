using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class BeforeAllElement<T> : SpecificationElement
    {
        public readonly Func<T> Action;

        public BeforeAllElement(ActualKeyword actualKeyword, Func<T> action)
            : base(actualKeyword)
        {
            Action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            return visitor.visitBeforeAll(this, position);
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
