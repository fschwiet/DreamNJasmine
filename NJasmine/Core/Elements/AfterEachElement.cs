using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class AfterEachElement : SpecificationElement
    {
        readonly Action _action;

        public AfterEachElement(ActualKeyword actualKeyword, Action action) : base(actualKeyword)
        {
            _action = action;
        }

        public override void Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitAfterEach(this, _action, position);
        }
    }
}
