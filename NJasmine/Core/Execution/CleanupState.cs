using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class CleanupState : ArrangeState
    {
        public CleanupState(NJasmineTestRunContext runContext, SpecificationElement specElement)
            : base(runContext, specElement)
        {
        }

        public override void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
            throw DontException(element);
        }

        public override TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            throw DontException(element);
        }
    }
}