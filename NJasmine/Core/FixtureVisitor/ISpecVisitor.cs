using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecVisitor
    {
        void visitFork(SpecElement origin, string description, Action action);
        TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory);
        void visitAfterEach(SpecElement origin, Action action);
        void visitTest(SpecElement origin, string description, Action action);
        TFixture visitImportNUnit<TFixture>() where TFixture : class, new();
    }
}