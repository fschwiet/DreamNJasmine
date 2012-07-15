using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Elements
{
    public class LeakDisposableElement : SpecificationElement
    {
        public readonly IDisposable Disposable;

        public LeakDisposableElement(ActualKeyword actualKeyword, IDisposable disposable) : base(actualKeyword)
        {
            Disposable = disposable;
        }

        public override object Run(ISpecPositionVisitor visitor, TestPosition position)
        {
            visitor.visitLeakDisposable(this, position);
            return ElementResultUnused;
        }
    }
}
