using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public abstract class SpecificationElement
    {
        public readonly ActualKeyword ActualKeyword;

        public SpecificationElement(ActualKeyword actualKeyword)
        {
            ActualKeyword = actualKeyword;
        }

        public override string ToString()
        {
            return ActualKeyword.ToString();
        }

        public abstract object Run(ISpecPositionVisitor visitor, TestPosition position);

        protected static object ElementResultUnused = new Object();
    }
}
