using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class CleanupState : ArrangeState
    {
        public CleanupState(NJasmineTestRunContext runContext, SpecElement specElement)
            : base(runContext, specElement)
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