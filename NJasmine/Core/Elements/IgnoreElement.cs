using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class IgnoreElement : SpecificationElement
    {
        readonly string _reason;

        public IgnoreElement(ActualKeyword actualKeyword, string reason) : base(actualKeyword)
        {
            _reason = reason;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitIgnoreBecause(this, _reason, position);
            return ElementResultUnused;
        }
    }
}
