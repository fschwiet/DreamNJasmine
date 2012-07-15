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

        public override void visitFork(ForkElement element, TestPosition position)
        {
            throw DontException(element);
        }

        public override TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position)
        {
            throw DontException(element);
        }

        public override void visitAfterAll(AfterAllElement element, TestPosition position)
        {
            throw DontException(element);
        }

        public override void visitTest(TestElement element, TestPosition position)
        {
            throw DontException(element);
        }

        public InvalidOperationException DontException(SpecificationElement innerSpecElement)
        {
            return new InvalidOperationException("Called " + innerSpecElement + "() within " + SpecElement + "().");
        }
    }
}