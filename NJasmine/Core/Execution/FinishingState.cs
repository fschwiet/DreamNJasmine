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

        public void visitFork(ForkElement origin, TestPosition position)
        {
        }

        public TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> origin, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(AfterAllElement origin, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> origin, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
        }

        public void visitTest(TestElement origin, TestPosition position)
        {
        }

        public void visitIgnoreBecause(IgnoreElement origin, TestPosition position)
        {
        }

        public void visitExpect(ExpectElement origin, TestPosition position)
        {
        }

        public void visitWaitUntil(WaitUntilElement origin, TestPosition position)
        {
        }

        public void visitWithCategory(WithCategoryElement origin, TestPosition position)
        {
        }

        public void visitTrace(TraceElement origin, TestPosition position)
        {
            _runContext.AddTrace(origin.Message);
        }

        public void visitLeakDisposable(LeakDisposableElement origin, TestPosition position)
        {
            _runContext.LeakDisposable(origin.Disposable);
        }
    }
}
