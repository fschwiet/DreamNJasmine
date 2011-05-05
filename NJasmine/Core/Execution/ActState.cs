using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class ActState : ArrangeState
    {
        public ActState(NJasmineTestRunContext runContext, SpecElement specElement)
            : base(runContext, specElement)
        {
        }
    }
}