
using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod : TestMethod, INJasmineTest, INJasmineFixturePositionVisitor
    {
        public class DescribeState : INJasmineFixturePositionVisitor
        {
            NJasmineTestMethod _subject;

            public DescribeState(NJasmineTestMethod subject)
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
                    _subject.whileInState(new ArrangeState(_subject, SpecMethod.beforeEach), 
                        action);
                }
            }

            public virtual void visitAfterEach(Action action, TestPosition position)
            {
                if (position.IsInScopeFor(_subject._position))
                {
                    _subject._allTeardowns.Add(delegate()
                    {
                        _subject.whileInState(new CleanupState(_subject, SpecMethod.afterEach), action);
                    });
                }
            }

            public virtual void visitIt(string description, Action action, TestPosition position)
            {
                if (position.ToString() == _subject._position.ToString())
                {
                    _subject.whileInState(new ActState(_subject, SpecMethod.it), action);

                    throw new TestFinishedException();
                }
            }

            public virtual TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture : class, new()
            {
                _subject.whileInState(new CleanupState(_subject, SpecMethod.importNUnit), delegate
                {
                    _subject._nUnitImports.DoSetUp(position);
                });

                _subject._allTeardowns.Add(delegate
                {
                    _subject.whileInState(new CleanupState(_subject, SpecMethod.importNUnit), delegate
                    {
                        _subject._nUnitImports.DoTearDown(position);
                    });
                });

                return _subject._nUnitImports.GetInstance(position) as TFixture;
            }

            public virtual TArranged visitArrange<TArranged>(string description, IEnumerable<Func<TArranged>> factories, TestPosition position)
            {
                TArranged lastResult = default(TArranged);

                foreach (var factory in factories)
                {
                    var currentFactory = factory;
                    TArranged result = default(TArranged);

                    _subject.whileInState(new ArrangeState(_subject, SpecMethod.arrange), delegate
                    {
                        result = currentFactory();
                    });

                    if (result is IDisposable)
                    {
                        _subject._allTeardowns.Add(delegate
                        {
                            _subject.whileInState(new CleanupState(_subject, SpecMethod.arrange), delegate
                            {
                                (result as IDisposable).Dispose();
                            });
                        });
                    }

                    lastResult = result;
                }

                return lastResult;
            }
        }
    }
}
