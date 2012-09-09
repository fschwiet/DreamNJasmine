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

        public void visitFork(ForkElement element, TestPosition position)
        {
            _runContext.State.visitFork(element, position);
        }

        public TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position)
        {
            return _runContext.State.visitBeforeAll(element, position);
        }

        public void visitAfterAll(AfterAllElement element, TestPosition position)
        {
            _runContext.State.visitAfterAll(element, position);
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            return _runContext.State.visitBeforeEach<TArranged>(element, position);
        }

        public void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
            _runContext.State.visitAfterEach(element, action, position);
        }

        public void visitWith<T>(WithElement<T> element, Action<T> action) where T : SharedFixture, new()
        {
            throw new NotImplementedException();
        }

        public void visitTest(TestElement element, TestPosition position)
        {
            _runContext.State.visitTest(element, position);
        }

        public void visitIgnoreBecause(IgnoreElement element, TestPosition position)
        {
            _runContext.State.visitIgnoreBecause(element, position);
        }

        public void visitExpect(ExpectElement element, TestPosition position)
        {
            _runContext.State.visitExpect(element, position);
        }

        public void visitWaitUntil(WaitUntilElement element, TestPosition position)
        {
            _runContext.State.visitWaitUntil(element, position);
        }

        public void visitWithCategory(WithCategoryElement element, TestPosition position)
        {
            _runContext.State.visitWithCategory(element, position);
        }

        public void visitTrace(TraceElement element, TestPosition position)
        {
            _runContext.State.visitTrace(element, position);
        }

        public void visitLeakDisposable(LeakDisposableElement element, TestPosition position)
        {
            _runContext.State.visitLeakDisposable(element, position);
        }
    }
}