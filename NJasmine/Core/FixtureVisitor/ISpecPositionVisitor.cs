using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core.Elements;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecPositionVisitor
    {
        void visitFork(ForkElement origin, TestPosition position);

        TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> origin, TestPosition position);
        void visitAfterAll(AfterAllElement origin, TestPosition position);

        TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> origin, TestPosition position);
        void visitAfterEach(SpecificationElement origin, Action action, TestPosition position);

        void visitTest(TestElement origin, TestPosition position);
        void visitIgnoreBecause(IgnoreElement origin, TestPosition position);

        void visitExpect(ExpectElement origin, TestPosition position);
        void visitWaitUntil(WaitUntilElement origin, TestPosition position);
        void visitWithCategory(WithCategoryElement origin, TestPosition position);

        void visitTrace(TraceElement origin, TestPosition position);
        void visitLeakDisposable(LeakDisposableElement origin, TestPosition position);
    }
}
