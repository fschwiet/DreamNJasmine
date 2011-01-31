using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public class DoNothingFixtureVisitor : ISpecVisitor
    {
        public void visitFork(string description, Action action)
        {
        }

        public virtual void visitAfterEach(Action action)
        {
        }

        public virtual void visitTest(string description, Action action)
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
