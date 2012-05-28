using System;

namespace NJasmine.Core.FixtureVisitor
{
    class ForkElement : SpecificationElement
    {
        readonly public string Description;
        readonly public Action Action;

        public ForkElement(SpecElement element, string description, Action action) : base(element)
        {
            Description = description;
            Action = action;
        }

        public void Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitFork(new SpecificationElement(SpecElement.describe), Description, Action, position);
        }
    }
}