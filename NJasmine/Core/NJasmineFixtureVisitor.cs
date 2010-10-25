using System;

namespace NJasmine.Core
{
    public class NJasmineFixtureVisitor : INJasmineFixtureVisitor
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
    }
}
