using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class ArrangeState : DiscoveryState
    {
        protected readonly SpecElement SpecElement;

        public ArrangeState(NJasmineTestRunContext runContext, SpecElement specElement)
            : base(runContext)
        {
            SpecElement = specElement;
        }

        public override void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public InvalidOperationException DontException(SpecElement innerSpecElement)
        {
            return new InvalidOperationException("Called " + innerSpecElement + "() within " + SpecElement + "().");
        }
    }
}