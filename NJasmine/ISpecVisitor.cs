using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine
{
    public interface ISpecVisitor
    {
        void visitFork(SpecElement origin, string description, Action action);

        TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action);
        void visitAfterAll(SpecElement origin, Action action);

        TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory);
        void visitAfterEach(SpecElement origin, Action action);

        void visitTest(SpecElement origin, string description, Action action);
    }
}