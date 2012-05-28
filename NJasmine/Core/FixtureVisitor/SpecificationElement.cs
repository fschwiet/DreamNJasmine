using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.Core.FixtureVisitor
{
    public class SpecificationElement
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

        public virtual object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            throw new NotImplementedException();
        }

        protected static object ElementResultUnused = new Object();
    }
}
