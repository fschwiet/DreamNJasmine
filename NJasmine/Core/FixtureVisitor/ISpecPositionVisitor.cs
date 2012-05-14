using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecPositionVisitor
    {
        void visitFork(SpecificationElement origin, string description, Action action, TestPosition position);

        TArranged visitBeforeAll<TArranged>(SpecificationElement origin, Func<TArranged> action, TestPosition position);
        void visitAfterAll(SpecificationElement origin, Action action, TestPosition position);

        TArranged visitBeforeEach<TArranged>(SpecificationElement origin, Func<TArranged> factory, TestPosition position);
        void visitAfterEach(SpecificationElement origin, Action action, TestPosition position);

        void visitTest(SpecificationElement origin, string description, Action action, TestPosition position);
        void visitIgnoreBecause(SpecificationElement origin, string reason, TestPosition position);

        void visitExpect(SpecificationElement origin, Expression<Func<bool>> expectation, TestPosition position);
        void visitWaitUntil(SpecificationElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position);
        void visitWithCategory(SpecificationElement origin, string category, TestPosition position);

        void visitTrace(SpecificationElement origin, string message, TestPosition position);
        void visitLeakDisposable(SpecificationElement origin, IDisposable disposable, TestPosition position);
    }
}
