using System;
using System.Linq.Expressions;
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

        public void visitFork(SpecificationElement origin, string description, Action action, TestPosition position)
        {
            _runContext.State.visitFork(origin, description, action, position);
        }

        public TArranged visitBeforeAll<TArranged>(SpecificationElement origin, Func<TArranged> action, TestPosition position)
        {
            return _runContext.State.visitBeforeAll(origin, action, position);
        }

        public void visitAfterAll(SpecificationElement origin, Action action, TestPosition position)
        {
            _runContext.State.visitAfterAll(origin, action, position);
        }

        public TArranged visitBeforeEach<TArranged>(SpecificationElement origin, Func<TArranged> factory, TestPosition position)
        {
            return _runContext.State.visitBeforeEach<TArranged>(origin, factory, position);
        }

        public void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
            _runContext.State.visitAfterEach(origin, action, position);
        }

        public void visitTest(SpecificationElement origin, string description, Action action, TestPosition position)
        {
            _runContext.State.visitTest(origin, description, action, position);
        }

        public void visitIgnoreBecause(SpecificationElement origin, string reason, TestPosition position)
        {
            _runContext.State.visitIgnoreBecause(origin, reason, position);
        }

        public void visitExpect(SpecificationElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
            _runContext.State.visitExpect(origin, expectation, position);
        }

        public void visitWaitUntil(SpecificationElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            _runContext.State.visitWaitUntil(origin, expectation, totalWaitMs, waitIncrementMs, position);
        }

        public void visitWithCategory(SpecificationElement origin, string category, TestPosition position)
        {
            _runContext.State.visitWithCategory(origin, category, position);
        }

        public void visitTrace(SpecificationElement origin, string message, TestPosition position)
        {
            _runContext.State.visitTrace(origin, message, position);
        }

        public void visitLeakDisposable(SpecificationElement origin, IDisposable disposable, TestPosition position)
        {
            _runContext.State.visitLeakDisposable(origin, disposable, position);
        }
    }
}