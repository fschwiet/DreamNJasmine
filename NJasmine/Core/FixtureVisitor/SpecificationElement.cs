using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.FixtureVisitor
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
