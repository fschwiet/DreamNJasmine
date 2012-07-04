using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class ForkElement : SpecificationElement
    {
        readonly public string Description;
        readonly public Action Action;

        public ForkElement(ActualKeyword actualKeyword, string description, Action action) : base(actualKeyword)
        {
            Description = description;
            Action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitFork(this, position);
            return ElementResultUnused;
        }
    }
}