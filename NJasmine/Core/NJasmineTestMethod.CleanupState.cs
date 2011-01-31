using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod
    {
        public class CleanupState : ArrangeState
        {
            public CleanupState(NJasmineTestMethod subject, SpecElement specElement)
                : base(subject, specElement)
            {
            }

            public override void visitAfterEach(SpecElement origin, Action action, TestPosition position)
            {
                throw DontException(origin);
            }

            public override TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
            {
                throw DontException(origin);
            }
        }
    }
}
