using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecVisitor
    {
        void visitFork(string description, Action action);
        TArranged visitBeforeEach<TArranged>(SpecMethod origin, string description, IEnumerable<Func<TArranged>> factories);
        void visitAfterEach(Action action);
        void visitTest(string description, Action action);
        TFixture visitImportNUnit<TFixture>() where TFixture : class, new();
    }
}