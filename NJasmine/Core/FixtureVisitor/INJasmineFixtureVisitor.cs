using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public interface INJasmineFixtureVisitor
    {
        void visitDescribe(string description, Action action);
        void visitAfterEach(Action action);
        void visitIt(string description, Action action);
        TFixture visitImportNUnit<TFixture>() where TFixture : class, new();
        TArranged visitArrange<TArranged>(SpecMethod origin, string description, IEnumerable<Func<TArranged>> factories);
    }
}