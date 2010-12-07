using System;

namespace NJasmine.Core.FixtureVisitor
{
    public interface INJasmineFixtureVisitor
    {
        void visitDescribe(string description, Action action);
        void visitBeforeEach(Action action);
        void visitAfterEach(Action action);
        void visitIt(string description, Action action);
        TFixture visitImportNUnit<TFixture>() where TFixture : class, new();
        TArranged visitArrange<TArranged>() where TArranged : class, new();
        TArranged visitArrange<TArranged>(Func<TArranged> factory) where TArranged : class;
    }
}