using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class LeakDisposableElement : SpecificationElement
    {
        readonly IDisposable _disposable;

        public LeakDisposableElement(ActualKeyword actualKeyword, IDisposable disposable) : base(actualKeyword)
        {
            _disposable = disposable;
        }

        public virtual object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitLeakDisposable(this, _disposable, position);
            return ElementResultUnused;
        }
    }
}
