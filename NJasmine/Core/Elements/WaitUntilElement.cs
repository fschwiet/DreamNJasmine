using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class WaitUntilElement : SpecificationElement
    {
        public readonly Expression<Func<bool>> Expectation;
        public readonly int WaitMaxMS;
        public readonly int WaitIncrementMS;

        public WaitUntilElement(ActualKeyword actualKeyword, Expression<Func<bool>> expectation, int waitMaxMs, int waitIncrementMs)
            : base(actualKeyword)
        {
            Expectation = expectation;
            WaitMaxMS = waitMaxMs;
            WaitIncrementMS = waitIncrementMs;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitWaitUntil(this, position);
            return null;
        }
    }
}
