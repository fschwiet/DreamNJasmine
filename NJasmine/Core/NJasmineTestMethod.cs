using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineTestMethod : TestMethod, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly TestPosition _position;

        VisitorPositionAdapter _visitorPositionAdapter;
        List<Action> _teardowns = new List<Action>();
        Action _nextStep;

        public NJasmineTestMethod(NJasmineFixture fixture, TestPosition position) : base(new Action(delegate() { }).Method)
        {
            _fixture = fixture;
            _position = position;
        }
        
        public override void RunTestMethod(TestResult testResult)
        {
            this.Run();
            testResult.Success();
        }

        public static NJasmineTestMethod Create(NJasmineFixture fixture, TestPosition position)
        {
            NJasmineTestMethod result = null;

            result = new NJasmineTestMethod(fixture, position);
            
            return result;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public void Run()
        {
            _visitorPositionAdapter = new VisitorPositionAdapter(this);
            _fixture.SetVisitor(_visitorPositionAdapter);

            try
            {
                 _fixture.Tests();
            }
            catch (TestFinishedException)
            {
            }

            _fixture.SetVisitor(new DontVisitor(DontVisitor.SpecMethod.afterEach));

            _teardowns.Reverse();
            foreach(var action in _teardowns)
            {
                action();
            }
        }

        public void visitDescribe(string description, Action action, TestPosition position)
        {
            if (_position.ToString().StartsWith(position.ToString()))
            {
                action();
            }
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
            if (position.IsInScopeFor(_position))
            {
                _fixture.SetVisitor(new DontVisitor(DontVisitor.SpecMethod.beforeEach));
                action();
                _fixture.SetVisitor(_visitorPositionAdapter);
            }
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            if (position.IsInScopeFor(_position))
            {
                _teardowns.Add(action);
            }
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            if (position.ToString() == _position.ToString())
            {
                _fixture.SetVisitor(new DontVisitor(DontVisitor.SpecMethod.it));
                action();

                throw new TestFinishedException();
            }
        }

        public class TestFinishedException : Exception
        {
        }

        private void Ignored() { }
    }
}
