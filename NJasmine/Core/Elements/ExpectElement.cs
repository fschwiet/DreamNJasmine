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
        public readonly Expression<Func<bool>> Expectation;

        public ExpectElement(ActualKeyword actualKeyword, Expression<Func<bool>> expectation)
            : base(actualKeyword)
        {
            Expectation = expectation;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitExpect(this, position);
            return ElementResultUnused;
        }
    }
}
