using System;
using System.Linq.Expressions;
using NJasmine.Core.Elements;
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

        public void visitFork(ForkElement origin, TestPosition position)
        {
            _runContext.State.visitFork(origin, position);
        }

        public TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> origin, TestPosition position)
        {
            return _runContext.State.visitBeforeAll(origin, position);
        }

        public void visitAfterAll(AfterAllElement origin, TestPosition position)
        {
            _runContext.State.visitAfterAll(origin, position);
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> origin, TestPosition position)
        {
            return _runContext.State.visitBeforeEach<TArranged>(origin, position);
        }

        public void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
            _runContext.State.visitAfterEach(origin, action, position);
        }

        public void visitTest(TestElement origin, TestPosition position)
        {
            _runContext.State.visitTest(origin, position);
        }

        public void visitIgnoreBecause(IgnoreElement origin, TestPosition position)
        {
            _runContext.State.visitIgnoreBecause(origin, position);
        }

        public void visitExpect(ExpectElement origin, TestPosition position)
        {
            _runContext.State.visitExpect(origin, position);
        }

        public void visitWaitUntil(WaitUntilElement origin, TestPosition position)
        {
            _runContext.State.visitWaitUntil(origin, position);
        }

        public void visitWithCategory(WithCategoryElement origin, TestPosition position)
        {
            _runContext.State.visitWithCategory(origin, position);
        }

        public void visitTrace(TraceElement origin, TestPosition position)
        {
            _runContext.State.visitTrace(origin, position);
        }

        public void visitLeakDisposable(LeakDisposableElement origin, TestPosition position)
        {
            _runContext.State.visitLeakDisposable(origin, position);
        }
    }
}