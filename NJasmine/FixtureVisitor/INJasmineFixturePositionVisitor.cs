using System;
using NJasmine.Core;

namespace NJasmine.FixtureVisitor
{
    public interface INJasmineFixturePositionVisitor
    {
        void visitDescribe(string description, Action action, TestPosition position);
        void visitBeforeEach(Action action, TestPosition position);
        void visitAfterEach(Action action, TestPosition position);
        void visitIt(string description, Action action, TestPosition position);
        TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new();
        TDisposable visitUsing<TDisposable>(TestPosition position) where TDisposable : class, IDisposable, new();
        TDisposable visitUsing<TDisposable>(Func<TDisposable> factory, TestPosition position) where TDisposable : class, IDisposable;
    }
}
