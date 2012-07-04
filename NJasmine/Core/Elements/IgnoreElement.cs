using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class IgnoreElement : SpecificationElement
    {
        public readonly string Reason;

        public IgnoreElement(ActualKeyword actualKeyword, string reason) : base(actualKeyword)
        {
            Reason = reason;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitIgnoreBecause(this, position);
            return ElementResultUnused;
        }
    }
}
