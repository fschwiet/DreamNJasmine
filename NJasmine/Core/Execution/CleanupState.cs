using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    public class CleanupState : ArrangeState
    {
        public CleanupState(NJasmineExecutionContext executionContext, SpecElement specElement)
            : base(executionContext, specElement)
        {
        }

        public override void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            throw DontException(origin);
        }
    }
}