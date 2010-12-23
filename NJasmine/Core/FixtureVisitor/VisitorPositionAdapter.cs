using System;
using System.Collections.Generic;

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

        private void DoThenAdvancePosition(Action action)
        {
            var nextSibling = _nextPosition.GetNextSiblingPosition();

            action();

            _nextPosition = nextSibling;
        }

        public void visitDescribe(string description, Action action)
        {
            DoThenAdvancePosition(delegate
            {
                if (action == null)
                {
                    _visitor.visitDescribe(description, null, _nextPosition);
                }
                else
                {
                    var firstChildPosition = _nextPosition.GetFirstChildPosition();

                    _visitor.visitDescribe(description, delegate()
                    {
                        _nextPosition = firstChildPosition;
                        action();
                    }, _nextPosition);
                }
            });
        }

        public void visitBeforeEach(Action action)
        {
            DoThenAdvancePosition(() => _visitor.visitBeforeEach(action, _nextPosition));
        }

        public void visitAfterEach(Action action)
        {
            DoThenAdvancePosition(() => _visitor.visitAfterEach(action, _nextPosition));
        }

        public void visitIt(string description, Action action)
        {
            DoThenAdvancePosition(() => _visitor.visitIt(description, action, _nextPosition));
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            TFixture result = null;

            DoThenAdvancePosition(() => result = _visitor.visitImportNUnit<TFixture>(_nextPosition));

            return result;
        }

        public TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories)
        {
            TArranged result = default(TArranged);

            DoThenAdvancePosition(() => result = _visitor.visitArrange(description, factories, _nextPosition));

            return result;
        }
    }
}