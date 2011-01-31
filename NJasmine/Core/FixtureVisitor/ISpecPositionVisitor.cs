using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public interface ISpecPositionVisitor
    {
        void visitFork(string description, Action action, TestPosition position);
        TArranged visitBeforeEach<TArranged>(SpecMethod origin, string description, IEnumerable<Func<TArranged>> factories, TestPosition position);
        void visitAfterEach(Action action, TestPosition position);
        void visitTest(string description, Action action, TestPosition position);
        TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new();
    }
}
