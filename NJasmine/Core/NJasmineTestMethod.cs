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

        List<Action> _setups = new List<Action>();
        List<Action> _teardowns = new List<Action>();
        Action _nextStep;

        public NJasmineTestMethod(MethodInfo method, NJasmineFixture fixture, TestPosition position) : base(method)
        {
            _fixture = fixture;
            _position = position;
        }
        
        public override void  RunTestMethod(TestResult testResult)
        {
            this.Run();
            testResult.Success();
        }

        public static NJasmineTestMethod Create(NJasmineFixture fixture, TestPosition position)
        {
            NJasmineTestMethod result = null;

            //  this method never gets ran
            Action testMethod = delegate()
            {
            };

            result = new NJasmineTestMethod(testMethod.Method, fixture, position);
            
            return result;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public void Run()
        {
            _fixture.SetVisitor(new VisitorPositionAdapter(this));

            _nextStep = null;
            _fixture.Tests();

            while(_nextStep != null)
            {
                var nextStep = _nextStep;
                _nextStep = null;

                nextStep();
            }

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
                _nextStep = action;
            }
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
            if (position.IsInScopeFor(_position))
            {
                action();
            }
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            if (position.IsInScopeFor(_position))
                _teardowns.Add(action);
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            if (position.ToString() == _position.ToString())
            {
                if (_nextStep != null)
                    throw new Exception("Unexpectedly found target test twice.");

                _nextStep = action;
            }
        }
    }
}
