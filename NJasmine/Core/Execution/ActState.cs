using NJasmine.Core.Discovery;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class ActState : ArrangeState
    {
        public ActState(NJasmineTestRunContext runContext, SpecificationElement specElement)
            : base(runContext, specElement)
        {
        }
    }
}