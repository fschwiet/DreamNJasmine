using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NJasmine.Core.FixtureVisitor
{
    public class DoNothingFixtureVisitor : ISpecPositionVisitor
    {
        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
        }

        public virtual void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
        }

        public virtual void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
        }

        public void visitExpect(Expression<Func<bool>> expectation, TestPosition position)
        {
        }

        public void visitWaitUntil(Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            return default(TArranged);
        }
    }
}
