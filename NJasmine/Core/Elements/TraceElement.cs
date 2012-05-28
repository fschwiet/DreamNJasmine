using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class TraceElement : SpecificationElement
    {
        readonly string _message;

        public TraceElement(ActualKeyword actualKeyword, string message) : base(actualKeyword)
        {
            _message = message;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitTrace(this, _message, position);
            return ElementResultUnused;
        }
    }
}
