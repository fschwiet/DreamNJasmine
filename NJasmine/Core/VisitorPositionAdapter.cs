using System;

namespace NJasmine.Core
{
    public class VisitorPositionAdapter : INJasmineFixtureVisitor
    {
        readonly INJasmineFixturePositionVisitor _visitor;
        protected TestPosition _nextPosition = null;

        public VisitorPositionAdapter(INJasmineFixturePositionVisitor visitor)
        {
            _visitor = visitor;
            _nextPosition = new TestPosition(0);
        }

        public void visitDescribe(string description, Action action)
        {
            TestPosition thisPosition = _nextPosition;

            _nextPosition = thisPosition.GetFirstChildPosition();

            _visitor.visitDescribe(description, action, thisPosition);

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
    }
}