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
        readonly Expression<Func<bool>> _expectation;
        readonly int _msWaitMax;
        readonly int _msWaitIncrement;

        public WaitUntilElement(ActualKeyword actualKeyword, Expression<Func<bool>> expectation, int msWaitMax, int msWaitIncrement)
            : base(actualKeyword)
        {
            _expectation = expectation;
            _msWaitMax = msWaitMax;
            _msWaitIncrement = msWaitIncrement;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitWaitUntil(this, _expectation, _msWaitMax, _msWaitIncrement, position);
            return null;
        }
    }
}
