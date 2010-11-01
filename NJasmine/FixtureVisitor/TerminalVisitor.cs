using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NJasmine.FixtureVisitor
{
    public class TerminalVisitor : INJasmineFixtureVisitor
    {
        readonly SpecMethod _specMethod;
        readonly INJasmineFixturePositionVisitor _originalVisitor;

        public TerminalVisitor(SpecMethod specMethod, INJasmineFixturePositionVisitor originalVisitor)
        {
            _specMethod = specMethod;
            _originalVisitor = originalVisitor;
        }

        public void visitDescribe(string description, Action action)
        {
            throw DontException(SpecMethod.describe);
        }

        public void visitBeforeEach(Action action)
        {
            throw DontException(SpecMethod.beforeEach);
        }

        public void visitAfterEach(Action action)
        {
            throw DontException(SpecMethod.afterEach);
        }

        public void visitIt(string description, Action action)
        {
            throw DontException(SpecMethod.it);
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            throw DontException(SpecMethod.importNUnit);
        }

        public TDisposable visitUsing<TDisposable>() where TDisposable : class, IDisposable, new()
        {
            return _originalVisitor.visitUsing<TDisposable>(null);
        }

        public TDisposable visitUsing<TDisposable>(Func<TDisposable> factory) where TDisposable : class, IDisposable
        {
            return _originalVisitor.visitUsing(factory, null);
        }

        InvalidOperationException DontException(SpecMethod innerSpecMethod)
        {
            return new InvalidOperationException("Called " + innerSpecMethod + "() within " + _specMethod + "().");
        }
    }
}
