using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class ExpectElement : SpecificationElement
    {
        readonly Expression<Func<bool>> _expectation;

        public ExpectElement(ActualKeyword actualKeyword, Expression<Func<bool>> expectation)
            : base(actualKeyword)
        {
            _expectation = expectation;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitExpect(this, _expectation, position);
            return ElementResultUnused;
        }
    }
}
