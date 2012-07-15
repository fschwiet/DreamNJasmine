using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class BeforeEachElement<T> : SpecificationElement
    {
        public readonly Func<T> Action;

        public BeforeEachElement(ActualKeyword beforeEach, Func<T> action)
            : base(beforeEach)
        {
            Action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            return visitor.visitBeforeEach(this, position);
        }
    }

    public class BeforeEachElementWithoutReturnValue : BeforeEachElement<object>
    {
        public BeforeEachElementWithoutReturnValue(ActualKeyword beforeEach, Action action)
            : base(beforeEach, delegate()
            {
                action();
                return ElementResultUnused;
            })
        {
        }
    }
}
