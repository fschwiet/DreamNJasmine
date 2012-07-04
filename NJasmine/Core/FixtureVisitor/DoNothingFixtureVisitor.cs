using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core.Elements;

namespace NJasmine.Core.FixtureVisitor
{
    class DoNothingFixtureVisitor : ISpecPositionVisitor
    {
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

        public virtual void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
        }

        public virtual void visitTest(TestElement origin, TestPosition position)
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
        }

        public void visitLeakDisposable(LeakDisposableElement origin, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> origin, TestPosition position)
        {
            return default(TArranged);
        }
    }
}
