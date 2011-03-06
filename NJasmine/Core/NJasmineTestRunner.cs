using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public class NJasmineTestRunner : ISpecPositionVisitor
    {
        NJasmineExecutionContext _executionContext;

        public NJasmineTestRunner(NJasmineExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            _executionContext.State.visitFork(origin, description, action, position);
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _executionContext.State.visitBeforeAll(origin, action, position);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _executionContext.State.visitAfterAll(origin, action, position);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
        {
            return _executionContext.State.visitBeforeEach<TArranged>(origin, description, factory, position);
        }

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _executionContext.State.visitAfterEach(origin, action, position);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            _executionContext.State.visitTest(origin, description, action, position);
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }
    }
}