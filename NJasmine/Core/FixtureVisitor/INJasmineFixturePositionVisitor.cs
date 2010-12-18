using System;
using System.Collections.Generic;

namespace NJasmine.Core.FixtureVisitor
{
    public interface INJasmineFixturePositionVisitor
    {
        void visitDescribe(string description, Action action, TestPosition position);
        void visitBeforeEach(Action action, TestPosition position);
        void visitAfterEach(Action action, TestPosition position);
        void visitIt(string description, Action action, TestPosition position);
        TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new();
        TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position);
    }
}
