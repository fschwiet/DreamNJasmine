using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class TraceElement : SpecificationElement
    {
        public readonly string Message;

        public TraceElement(ActualKeyword actualKeyword, string message) : base(actualKeyword)
        {
            Message = message;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitTrace(this, position);
            return ElementResultUnused;
        }
    }
}
