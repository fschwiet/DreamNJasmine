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

        List<Action> _allTeardowns = new List<Action>();
        List<Action> _resourceTeardowns = new List<Action>();
        bool _haveClippedTeardownsAfterFailure = false;

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
            _fixture.PushVisitor(new VisitorPositionAdapter(this));

            _resourceTeardowns = new List<Action>();

            try
            {
                 _fixture.Tests();
            }
            catch (TestFinishedException)
            {
            }
            catch
            {
                if (!_haveClippedTeardownsAfterFailure)
                {
                    _allTeardowns = new List<Action>();
                    _allTeardowns.AddRange(_resourceTeardowns);
                    _haveClippedTeardownsAfterFailure = true;
                }

                throw;
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
                var existingTeardowns = _allTeardowns.ToArray();
                var lastDiposeTeardowns = _resourceTeardowns.ToArray();
                _resourceTeardowns = new List<Action>();
                
                try
                {
                    action();
                }
                catch (TestFinishedException)
                {
                    throw;
                }
                catch
                {
                    if (!_haveClippedTeardownsAfterFailure)
                    {
                        _allTeardowns = new List<Action>(existingTeardowns);
                        _allTeardowns.AddRange(lastDiposeTeardowns);
                        _haveClippedTeardownsAfterFailure = true;
                    }

                    throw;
                }
                finally
                {
                    _resourceTeardowns = new List<Action>(lastDiposeTeardowns);
                }
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

            Action disposeAction = delegate
            {
                _nUnitImports.DoTearDown(position);
            };

            _allTeardowns.Add(disposeAction);
            _resourceTeardowns.Add(disposeAction);

            return _nUnitImports.GetInstance(position) as TFixture;
        }

        public TDisposable visitDisposing<TDisposable>(TestPosition position) where TDisposable : class, IDisposable, new()
        {
            var result = new TDisposable();

            Action disposeAction = delegate
            {
                result.Dispose();
            };

            _allTeardowns.Add(disposeAction);
            _resourceTeardowns.Add(disposeAction);

            return result;
        }

        public TDisposable visitDisposing<TDisposable>(Func<TDisposable> factory, TestPosition position) where TDisposable : class, IDisposable
        {
            var result = factory();

            Action disposeAction = delegate
            {
                result.Dispose();
            };

            _allTeardowns.Add(disposeAction);
            _resourceTeardowns.Add(disposeAction);

            return result;
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
