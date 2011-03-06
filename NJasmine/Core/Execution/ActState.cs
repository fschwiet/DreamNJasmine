using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    public class ActState : ArrangeState
    {
        public ActState(NJasmineExecutionContext ExecutionContext, SpecElement specElement)
            : base(ExecutionContext, specElement)
        {
        }
    }
}