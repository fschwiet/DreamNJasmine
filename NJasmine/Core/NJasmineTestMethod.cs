using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineTestMethod : TestMethod, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly TestPosition _position;
        readonly NUnitFixtureCollection _nUnitImports;

        List<Action> _allTeardowns = null;

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
            _allTeardowns = new List<Action>();

            _fixture.PushVisitor(new VisitorPositionAdapter(this));

            try
            {
                 _fixture.Tests();
            }
            catch (TestFinishedException)
            {
            }
            finally
            {
                _fixture.PushVisitor(new TerminalVisitor(SpecMethod.afterEach, this));

                _allTeardowns.Reverse();
                foreach (var action in _allTeardowns)
                {
                    action();
                }
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
                _fixture.PushVisitor(new TerminalVisitor(SpecMethod.beforeEach, this));
                action();
                _fixture.PopVisitor();
            }
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            if (position.IsInScopeFor(_position))
            {
                _allTeardowns.Add(action);
            }
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            if (position.ToString() == _position.ToString())
            {
                _fixture.PushVisitor(new TerminalVisitor(SpecMethod.it, this));
                action();

                throw new TestFinishedException();
            }
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            _nUnitImports.DoSetUp(position);

            _allTeardowns.Add(delegate
            {
                _nUnitImports.DoTearDown(position);
            });

            return _nUnitImports.GetInstance(position) as TFixture;
        }

        public TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
        {
            TArranged result = default(TArranged);

            foreach(var factory in factories)
            {
                result = factory();

                if (result is IDisposable)
                {
                    _allTeardowns.Add(delegate
                    {
                        (result as IDisposable).Dispose();
                    });
                }
            }

            return result;
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
