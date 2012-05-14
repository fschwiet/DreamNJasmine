using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class ArrangeState : DiscoveryState
    {
        protected readonly SpecificationElement SpecElement;

        public ArrangeState(NJasmineTestRunContext runContext, SpecificationElement specElement)
            : base(runContext)
        {
            SpecElement = specElement;
        }

        public override void visitFork(SpecificationElement origin, string description, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override TArranged visitBeforeAll<TArranged>(SpecificationElement origin, Func<TArranged> action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override void visitAfterAll(SpecificationElement origin, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public override void visitTest(SpecificationElement origin, string description, Action action, TestPosition position)
        {
            throw DontException(origin);
        }

        public InvalidOperationException DontException(SpecificationElement innerSpecElement)
        {
            return new InvalidOperationException("Called " + innerSpecElement + "() within " + SpecElement + "().");
        }
    }
}