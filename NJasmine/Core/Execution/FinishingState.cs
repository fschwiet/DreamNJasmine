using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class FinishingState : ISpecPositionVisitor
    {
        private readonly NJasmineTestRunContext _runContext;

        public FinishingState(NJasmineTestRunContext runContext)
        {
            _runContext = runContext;
        }

        public void visitFork(ForkElement element, TestPosition position)
        {
        }

        public TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(AfterAllElement element, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
        }

        public void visitTest(TestElement element, TestPosition position)
        {
        }

        public void visitIgnoreBecause(IgnoreElement element, TestPosition position)
        {
        }

        public void visitExpect(ExpectElement element, TestPosition position)
        {
        }

        public void visitWaitUntil(WaitUntilElement element, TestPosition position)
        {
        }

        public void visitWithCategory(WithCategoryElement element, TestPosition position)
        {
        }

        public void visitTrace(TraceElement element, TestPosition position)
        {
            _runContext.AddTrace(element.Message);
        }

        public void visitLeakDisposable(LeakDisposableElement element, TestPosition position)
        {
            _runContext.LeakDisposable(element.Disposable);
        }
    }
}
