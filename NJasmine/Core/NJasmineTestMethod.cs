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
        readonly NUnitFixtureCollection _nUnitImports;

        VisitorPositionAdapter _visitorPositionAdapter;
        List<Action> _teardowns = new List<Action>();

        public NJasmineTestMethod(NJasmineFixture fixture, TestPosition position, NUnitFixtureCollection nUnitImports) : base(new Action(delegate() { }).Method)
        {
            _fixture = fixture;
            _position = position;
            _nUnitImports = nUnitImports;
        }
        
        public override void RunTestMethod(TestResult testResult)
        {
            this.Run();
            testResult.Success();
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public void Run()
        {
            _visitorPositionAdapter = new VisitorPositionAdapter(this);
            _fixture.PushVisitor(_visitorPositionAdapter);

            try
            {
                 _fixture.Tests();
            }
            catch (TestFinishedException)
            {
            }

            _fixture.PushVisitor(new DontVisitor(DontVisitor.SpecMethod.afterEach));

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
                _fixture.PushVisitor(new DontVisitor(DontVisitor.SpecMethod.beforeEach));
                action();
                _fixture.PopVisitor();
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
                _fixture.PushVisitor(new DontVisitor(DontVisitor.SpecMethod.it));
                action();

                throw new TestFinishedException();
            }
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            _nUnitImports.DoSetUp(position);

            _teardowns.Add(delegate
            {
                _nUnitImports.DoTearDown(position);
            });

            return _nUnitImports.GetInstance(position) as TFixture;
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
