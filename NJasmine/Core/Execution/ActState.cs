using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    public class ActState : ArrangeState
    {
        public ActState(NJasmineTestRunContext runContext, SpecElement specElement)
            : base(runContext, specElement)
        {
        }
    }
}