using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod : TestMethod, INJasmineTest, INJasmineFixturePositionVisitor
    {
        readonly NJasmineFixture _fixture;
        readonly TestPosition _position;
        readonly NUnitFixtureCollection _nUnitImports;

        List<Action> _allTeardowns = null;
        INJasmineFixturePositionVisitor _state = null;

        public NJasmineTestMethod(NJasmineFixture fixture, TestPosition position, NUnitFixtureCollection nUnitImports) : base(new Action(delegate() { }).Method)
        {
            _fixture = fixture;
            _position = position;
            _nUnitImports = nUnitImports;
            _state = new State(this);
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public override void RunTestMethod(TestResult testResult)
        {
            this._allTeardowns = new List<Action>();

            this._fixture.UseVisitor(new VisitorPositionAdapter(this));

            try
            {
                this._fixture.Tests();
            }

            catch (TestFinishedException)
            {
            }
            finally
            {
                this._allTeardowns.Reverse();

                foreach (var action in this._allTeardowns)
                {
                    action();
                }
            }
            testResult.Success();
        }

        public void whileInState(INJasmineFixturePositionVisitor state, Action action)
        {
            var originalState = _state;
            _state = state;
            try
            {
                action();
            }
            finally
            {
                _state = originalState;
            }
        }

        public void visitDescribe(string description, Action action, TestPosition position)
        {
            _state.visitDescribe(description, action, position);
        }

        public void visitBeforeEach(Action action, TestPosition position)
        {
            _state.visitBeforeEach(action, position);
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            _state.visitAfterEach(action, position);
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            _state.visitIt(description, action, position);
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            return _state.visitImportNUnit<TFixture>(position);
        }

        public TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
        {
            return _state.visitArrange<TArranged>(description, factories, position);
        }

        public class TestFinishedException : Exception
        {
        }

        public class State : INJasmineFixturePositionVisitor
        {
            NJasmineTestMethod _subject;

            public State(NJasmineTestMethod subject)
            {
                _subject = subject;
            }

            public virtual void visitDescribe(string description, Action action, TestPosition position)
            {
                if (_subject._position.ToString().StartsWith(position.ToString()))
                {
                    action();
                }
            }

            public virtual void visitBeforeEach(Action action, TestPosition position)
            {
                if (_subject._position.IsInScopeFor(_subject._position))
                {
                    _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.beforeEach), 
                        action);
                }
            }

            public virtual void visitAfterEach(Action action, TestPosition position)
            {
                if (position.IsInScopeFor(_subject._position))
                {
                    _subject._allTeardowns.Add(delegate()
                    {
                        _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.afterEach), action);
                    });
                }
            }

            public virtual void visitIt(string description, Action action, TestPosition position)
            {
                if (position.ToString() == _subject._position.ToString())
                {
                    _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.it), action);

                    throw new TestFinishedException();
                }
            }

            public virtual TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture : class, new()
            {
                _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.importNUnit), delegate
                {
                    _subject._nUnitImports.DoSetUp(position);
                });

                _subject._allTeardowns.Add(delegate
                {
                    _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.importNUnit), delegate
                    {
                        _subject._nUnitImports.DoTearDown(position);
                    });
                });

                return _subject._nUnitImports.GetInstance(position) as TFixture;
            }

            public virtual TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
            {
                TArranged lastResult = default(TArranged);

                //_subject.whileInState(new TerminalVisitor(_subject, SpecMethod.arrange), delegate
                //{
                    foreach (var factory in factories)
                    {
                        var currentFactory = factory;
                        TArranged result = default(TArranged);

                        _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.arrange), delegate
                        {
                            result = currentFactory();
                        });

                        if (result is IDisposable)
                        {
                            _subject._allTeardowns.Add(delegate
                            {
                                _subject.whileInState(new TerminalVisitor(_subject, SpecMethod.arrange), delegate
                                {
                                    (result as IDisposable).Dispose();
                                });
                            });
                        }

                        lastResult = result;
                    }
                //});

                return lastResult;
            }
        }
    }
}
