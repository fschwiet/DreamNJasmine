using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    class ForkElement : SpecificationElement
    {
        readonly public string Description;
        readonly public Action Action;

        public ForkElement(ActualKeyword actualKeyword, string description, Action action) : base(actualKeyword)
        {
            Description = description;
            Action = action;
        }

        public override void Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitFork(this, Description, Action, position);
        }
    }
}