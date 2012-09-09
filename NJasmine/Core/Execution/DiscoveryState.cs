using System;
using System.Linq.Expressions;
using NJasmine.Core.Discovery;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine.Core.Execution
{
    class DiscoveryState : ISpecPositionVisitor
    {
        private NJasmineTestRunContext _runContext;

        public DiscoveryState(NJasmineTestRunContext runContext)
        {
            _runContext = runContext;
        }

        public virtual void visitFork(ForkElement element, TestPosition position)
        {
            if (_runContext.PositionIsAncestorOfContext(position))
            {
                element.Action();
            }
        }

        public virtual TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position)
        {
            return _runContext.GetSetupResultAt<TArranged>(position);
        }

        public virtual void visitAfterAll(AfterAllElement element, TestPosition position)
        {
        }

        public virtual void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
            _runContext.AddTeardownAction(delegate()
            {
                _runContext.whileInState(new CleanupState(_runContext, element), action);
            });
        }

        public void visitWith<T>(WithElement<T> element, Action<T> action) where T : SharedFixture, new()
        {
            throw new NotImplementedException();
        }

        public virtual void visitTest(TestElement element, TestPosition position)
        {
            if (_runContext.TestIsAtPosition(position))
            {
                _runContext.whileInState(new ActState(_runContext, element), element.Action);

                _runContext.GotoStateFinishing();
            }
        }

        public void visitIgnoreBecause(IgnoreElement element, TestPosition position)
        {
        }

        public void visitExpect(ExpectElement element, TestPosition position)
        {
            Expect.That(element.Expectation);
        }

        public void visitWaitUntil(WaitUntilElement element, TestPosition position)
        {
            Expect.Eventually(element.Expectation, element.WaitMaxMS, element.WaitIncrementMS);
        }

        public void visitWithCategory(WithCategoryElement element, TestPosition position)
        {
        }

        public void visitTrace(TraceElement element, TestPosition position)
        {
            _runContext.AddTrace(element.Message);
        }

        public void visitLeakDisposable(LeakDisposableElement element, TestPosition position)
        {
            _runContext.LeakDisposable(element.Disposable);
        }

        public virtual TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            TArranged result = default(TArranged);

            _runContext.whileInState(new ArrangeState(_runContext, element), delegate
            {
                result = element.Action();
            });

            if (result is IDisposable)
            {
                _runContext.AddTeardownAction(delegate
                {
                    _runContext.whileInState(new CleanupState(_runContext, element), delegate
                    {
                        _runContext.DisposeIfNotLeaked(result as IDisposable);
                    });
                });
            }

            return result;
        }
    }
}