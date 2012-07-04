using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core.Elements;

namespace NJasmine.Core.FixtureVisitor
{
    class DoNothingFixtureVisitor : ISpecPositionVisitor
    {
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

        public virtual void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
        }

        public virtual void visitTest(TestElement element, TestPosition position)
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
        }

        public void visitLeakDisposable(LeakDisposableElement element, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            return default(TArranged);
        }
    }
}
