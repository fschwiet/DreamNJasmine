using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public class DoNothingFixtureVisitor : ISpecVisitor
    {
        public void visitFork(SpecElement origin, string description, Action action)
        {
        }

        public void visitBeforeAll(SpecElement origin, Action action)
        {
        }

        public void visitAfterAll(SpecElement origin, Action action)
        {
        }

        public virtual void visitAfterEach(SpecElement origin, Action action)
        {
        }

        public virtual void visitTest(SpecElement origin, string description, Action action)
        {
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            return default(TFixture);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory)
        {
            return default(TArranged);
        }
    }
}
