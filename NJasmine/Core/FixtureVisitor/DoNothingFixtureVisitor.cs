using System;

namespace NJasmine.Core.FixtureVisitor
{
    public class DoNothingFixtureVisitor : INJasmineFixtureVisitor
    {
        public void visitDescribe(string description, Action action)
        {
        }

        public virtual void visitBeforeEach(Action action)
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

        public TArranged visitArrange<TArranged>(string description, Func<TArranged> factory)
        {
            return default(TArranged);
        }
    }
}
