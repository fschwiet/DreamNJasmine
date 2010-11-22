using System;

namespace NJasmine.Core.FixtureVisitor
{
    public class VisitorPositionAdapter : INJasmineFixtureVisitor
    {
        readonly INJasmineFixturePositionVisitor _visitor;
        protected TestPosition _nextPosition;

        public VisitorPositionAdapter(INJasmineFixturePositionVisitor visitor)
        {
            _visitor = visitor;
            _nextPosition = new TestPosition(0);
        }

        public VisitorPositionAdapter(TestPosition position, INJasmineFixturePositionVisitor visitor)
        {
            _visitor = visitor;
            _nextPosition = position;
        }

        public void visitDescribe(string description, Action action)
        {
            TestPosition thisPosition = _nextPosition;

            _visitor.visitDescribe(description, delegate()
            {
                _nextPosition = thisPosition.GetFirstChildPosition();
                action();
            }, thisPosition);

            _nextPosition = thisPosition.GetNextSiblingPosition();
        }

        public void visitBeforeEach(Action action)
        {
            _visitor.visitBeforeEach(action, _nextPosition);

            _nextPosition = _nextPosition.GetNextSiblingPosition();
        }

        public void visitAfterEach(Action action)
        {
            _visitor.visitAfterEach(action, _nextPosition);

            _nextPosition = _nextPosition.GetNextSiblingPosition();
        }

        public void visitIt(string description, Action action)
        {
            _visitor.visitIt(description, action, _nextPosition);

            _nextPosition = _nextPosition.GetNextSiblingPosition();
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            var result = _visitor.visitImportNUnit<TFixture>(_nextPosition);

            _nextPosition = _nextPosition.GetNextSiblingPosition();

            return result;
        }

        public TDisposable visitDisposing<TDisposable>() where TDisposable : class, IDisposable, new()
        {
            var result = _visitor.visitDisposing<TDisposable>(_nextPosition);

            _nextPosition = _nextPosition.GetNextSiblingPosition();

            return result;
        }

        public TDisposable visitDisposing<TDisposable>(Func<TDisposable> factory) where TDisposable : class, IDisposable
        {
            var result = _visitor.visitDisposing(factory, _nextPosition);

            _nextPosition = _nextPosition.GetNextSiblingPosition();

            return result;
        }
    }
}