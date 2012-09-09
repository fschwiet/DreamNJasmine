using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class WithElement<T> : SpecificationElement where T : SharedFixture
    {
        private readonly Action<T> _action;

        public WithElement(ActualKeyword actualKeyword, Action<T> action) : base(actualKeyword)
        {
            _action = action;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            throw new NotImplementedException();
        }
    }
}
