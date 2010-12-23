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

            try
            {
                using(_fixture.UseVisitor(new VisitorPositionAdapter(this)))
                {
                    _fixture.Tests();
                }
            }

            catch (TestFinishedException)
            {
            }
            finally
            {
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
                using(_fixture.UseVisitor(new TerminalVisitor(SpecMethod.beforeEach, this)))
                {
                    action();
                }
            }
        }

        public void visitAfterEach(Action action, TestPosition position)
        {
            if (position.IsInScopeFor(_position))
            {
                _allTeardowns.Add(delegate()
                {
                    using (_fixture.UseVisitor(new TerminalVisitor(SpecMethod.afterEach, this)))
                    {
                        action();
                    }
                });
            }
        }

        public void visitIt(string description, Action action, TestPosition position)
        {
            if (position.ToString() == _position.ToString())
            {
                using (_fixture.UseVisitor(new TerminalVisitor(SpecMethod.it, this)))
                {
                    action();
                }
                
                throw new TestFinishedException();
            }
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            using (_fixture.UseVisitor(new TerminalVisitor(SpecMethod.importNUnit, this)))
            {
                _nUnitImports.DoSetUp(position);
            }

            _allTeardowns.Add(delegate
            {
                using (_fixture.UseVisitor(new TerminalVisitor(SpecMethod.importNUnit, this)))
                {
                    _nUnitImports.DoTearDown(position);
                }
            });

            return _nUnitImports.GetInstance(position) as TFixture;
        }

        public TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
        {
            TArranged result = default(TArranged);

            using (_fixture.UseVisitor(new TerminalVisitor(SpecMethod.arrange, this)))
            {
                foreach (var factory in factories)
                {
                    result = factory();

                    if (result is IDisposable)
                    {
                        _allTeardowns.Add(delegate
                        {
                            using (_fixture.UseVisitor(new TerminalVisitor(SpecMethod.arrange, this)))
                            {
                                (result as IDisposable).Dispose();
                            }
                        });
                    }
                }
            }

            return result;
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
