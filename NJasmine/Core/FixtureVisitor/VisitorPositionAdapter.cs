using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace NJasmine.Core.FixtureVisitor
{
    public class VisitorPositionAdapter : ISpecVisitor
    {
        readonly ISpecPositionVisitor _visitor;
        protected TestPosition _position;

        public VisitorPositionAdapter(ISpecPositionVisitor visitor)
        {
            _visitor = visitor;
            _position = new TestPosition(0);
        }

        public VisitorPositionAdapter(TestPosition position, ISpecPositionVisitor visitor)
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
        
        public void visitFork(SpecElement origin, string description, Action action)
        {
            DoThenAdvancePosition(() => 
                _visitor.visitFork(origin, description, WrapActionToRunAtFirstchildPosition(action), _position));
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action)
        {
            TArranged arranged = default(TArranged);
            DoThenAdvancePosition(() => arranged = _visitor.visitBeforeAll(origin, action, _position));
            return arranged;
        }

        public void visitAfterAll(SpecElement origin, Action action)
        {
            DoThenAdvancePosition(() => _visitor.visitAfterAll(origin, action, _position));
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory)
        {
            TArranged result = default(TArranged);

            DoThenAdvancePosition(() => result =
                _visitor.visitBeforeEach(origin, description, factory, _position));

            return result;
        }

        public void visitAfterEach(SpecElement origin, Action action)
        {
            DoThenAdvancePosition(() => 
                _visitor.visitAfterEach(origin, WrapActionToRunAtFirstchildPosition(action), _position));
        }

        public void visitTest(SpecElement origin, string description, Action action)
        {
            DoThenAdvancePosition(() =>
                _visitor.visitTest(origin, description, WrapActionToRunAtFirstchildPosition(action), _position));
        }

        public TFixture visitImportNUnit<TFixture>() where TFixture: class, new()
        {
            TFixture result = null;

            DoThenAdvancePosition(() => 
                result = _visitor.visitImportNUnit<TFixture>(_position));

            return result;
        }
    }
}