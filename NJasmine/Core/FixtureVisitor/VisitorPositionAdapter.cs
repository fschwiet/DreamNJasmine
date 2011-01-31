using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NJasmine.Core.FixtureVisitor
{
    public class VisitorPositionAdapter : INJasmineFixtureVisitor
    {
        readonly INJasmineFixturePositionVisitor _visitor;
        protected TestPosition _position;

        public VisitorPositionAdapter(INJasmineFixturePositionVisitor visitor)
        {
            _visitor = visitor;
            _position = new TestPosition(0);
        }

        public VisitorPositionAdapter(TestPosition position, INJasmineFixturePositionVisitor visitor)
        {
            _visitor = visitor;
            _position = position;
        }

        private void DoThenAdvancePosition(Action action)
        {
            var nextSibling = _position.GetNextSiblingPosition();

            action();

            _position = nextSibling;
        }

        private Action WrapActionToRunAtFirstchildPosition(Action action)
        {
            if (action == null)
                return null;

            var firstChildPosition = _position.GetFirstChildPosition();

            return delegate
            {
                _position = firstChildPosition;

                action();
            };
        }

        private IEnumerable<Func<T>> WrapFunctionToRunAtChildPosition<T>(IEnumerable<Func<T>> funcs)
        {
            List<Func<T>> result = new List<Func<T>>();
            var nextChildPosition = _position.GetFirstChildPosition();

            foreach(var func in funcs)
            {
                var thisPosition = nextChildPosition;
                var thisFunc = func;

                result.Add(delegate
                {
                    _position = thisPosition.GetFirstChildPosition();
                    return thisFunc();
                });

                nextChildPosition = nextChildPosition.GetNextSiblingPosition();
            }

            return result;
        }

        public void visitDescribe(string description, Action action)
        {
            DoThenAdvancePosition(() => 
                _visitor.visitDescribe(description, WrapActionToRunAtFirstchildPosition(action), _position));
        }

        public void visitAfterEach(Action action)
        {
            DoThenAdvancePosition(() => 
                _visitor.visitAfterEach(WrapActionToRunAtFirstchildPosition(action), _position));
        }

        public void visitIt(string description, Action action)
        {
            DoThenAdvancePosition(() => 
                _visitor.visitIt(description, WrapActionToRunAtFirstchildPosition(action), _position));
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            TFixture result = null;

            DoThenAdvancePosition(() => 
                result = _visitor.visitImportNUnit<TFixture>(_position));

            return result;
        }

        public TArranged visitArrange<TArranged>(SpecMethod origin, string description, IEnumerable<Func<TArranged>> factories)
        {
            TArranged result = default(TArranged);

            factories = WrapFunctionToRunAtChildPosition(factories);

            DoThenAdvancePosition(() => result = 
                _visitor.visitArrange(origin, description, factories, _position));

            return result;
        }
    }
}