using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.Elements;
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

        public override void visitFork(ForkElement origin, TestPosition position)
        {
            throw DontException(origin);
        }

        public override TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> origin, TestPosition position)
        {
            throw DontException(origin);
        }

        public override void visitAfterAll(AfterAllElement origin, TestPosition position)
        {
            throw DontException(origin);
        }

        public override void visitTest(TestElement origin, TestPosition position)
        {
            throw DontException(origin);
        }

        public InvalidOperationException DontException(SpecificationElement innerSpecElement)
        {
            return new InvalidOperationException("Called " + innerSpecElement + "() within " + SpecElement + "().");
        }
    }
}