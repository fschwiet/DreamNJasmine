using System;

namespace NJasmine.FixtureVisitor
{
    public interface INJasmineFixtureVisitor
    {
        void visitDescribe(string description, Action action);
        void visitBeforeEach(Action action);
        void visitAfterEach(Action action);
        void visitIt(string description, Action action);
        TFixture visitImportNUnit<TFixture>() where TFixture : class, new();
        TDisposable visitUsing<TDisposable>() where TDisposable : class, IDisposable, new();
        TDisposable visitUsing<TDisposable>(Func<TDisposable> factory) where TDisposable : class, IDisposable;
    }
}