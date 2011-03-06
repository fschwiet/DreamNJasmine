using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    public class NJasmineTestRunner : ISpecPositionVisitor
    {
        NJasmineTestRunContext _runContext;

        public NJasmineTestRunner(NJasmineTestRunContext runContext)
        {
            _runContext = runContext;
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            _runContext.State.visitFork(origin, description, action, position);
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _runContext.State.visitBeforeAll(origin, action, position);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _runContext.State.visitAfterAll(origin, action, position);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            return _runContext.State.visitBeforeEach<TArranged>(origin, factory, position);
        }

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _runContext.State.visitAfterEach(origin, action, position);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            _runContext.State.visitTest(origin, description, action, position);
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            _runContext.State.visitIgnoreBecause(reason, position);
        }
    }
}