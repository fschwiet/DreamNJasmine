using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecPositionVisitor
    {
        void visitFork(SpecElement origin, string description, Action action, TestPosition position);

        void visitEither(SpecElement origin, Action<Action>[] options, TestPosition position, out TestPosition continuingAt); 

        TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position);
        void visitAfterAll(SpecElement origin, Action action, TestPosition position);

        TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position);
        void visitAfterEach(SpecElement origin, Action action, TestPosition position);

        void visitTest(SpecElement origin, string description, Action action, TestPosition position);
        void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position);

        void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position);
        void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position);
        void visitWithCategory(SpecElement origin, string category, TestPosition position);

        void visitTrace(SpecElement origin, string message, TestPosition position);
        void visitLeakDisposable(SpecElement origin, IDisposable disposable, TestPosition position);
    }
}
