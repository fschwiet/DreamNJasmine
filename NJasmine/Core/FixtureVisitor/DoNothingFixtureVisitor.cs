using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public class DoNothingFixtureVisitor : INJasmineFixtureVisitor
    {
        public void visitDescribe(string description, Action action)
        {
        }

        public virtual void visitAfterEach(Action action)
        {
        }

        public virtual void visitIt(string description, Action action)
        {
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            return default(TFixture);
        }

        public TArranged visitArrange<TArranged>(SpecMethod origin, string description, IEnumerable<Func<TArranged>> factories)
        {
            return default(TArranged);
        }
    }
}
