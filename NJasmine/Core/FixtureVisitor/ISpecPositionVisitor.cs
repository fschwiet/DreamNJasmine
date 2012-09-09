using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core.Elements;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecPositionVisitor
    {
        void visitFork(ForkElement element, TestPosition position);

        TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position);
        void visitAfterAll(AfterAllElement element, TestPosition position);

        TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position);
        void visitAfterEach(SpecificationElement element, Action action, TestPosition position);
        void visitWith<T>(WithElement<T> element, Action<T> action) where T : SharedFixture;

        void visitTest(TestElement element, TestPosition position);
        void visitIgnoreBecause(IgnoreElement element, TestPosition position);

        void visitExpect(ExpectElement element, TestPosition position);
        void visitWaitUntil(WaitUntilElement element, TestPosition position);
        void visitWithCategory(WithCategoryElement element, TestPosition position);

        void visitTrace(TraceElement element, TestPosition position);
        void visitLeakDisposable(LeakDisposableElement element, TestPosition position);
    }
}
