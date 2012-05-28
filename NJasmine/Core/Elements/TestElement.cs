using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class TestElement : SpecificationElement
    {
        readonly string _description;
        readonly Action _action;

        public TestElement(ActualKeyword actualKeyword, string description, Action action) : base(actualKeyword)
        {
            _description = description;
            _action = action;
        }

        public override void Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitTest(this, _description, _action, position);
        }
    }
}
