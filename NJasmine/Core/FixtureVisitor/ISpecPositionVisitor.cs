using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecPositionVisitor
    {
        void visitFork(SpecElement origin, string description, Action action, TestPosition position);

        TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position);
        void visitAfterAll(SpecElement origin, Action action, TestPosition position);

        TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position);
        void visitAfterEach(SpecElement origin, Action action, TestPosition position);

        void visitTest(SpecElement origin, string description, Action action, TestPosition position);
        void visitIgnoreBecause(string reason, TestPosition position);
    }
}
