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

        public virtual void visitFork(ForkElement origin, TestPosition position)
        {
            if (_runContext.PositionIsAncestorOfContext(position))
            {
                origin.Action();
            }
        }

        public virtual TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> origin, TestPosition position)
        {
            return _runContext.GetSetupResultAt<TArranged>(position);
        }

        public virtual void visitAfterAll(AfterAllElement origin, TestPosition position)
        {
        }

        public virtual void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
            _runContext.AddTeardownAction(delegate()
            {
                _runContext.whileInState(new CleanupState(_runContext, origin), action);
            });
        }

        public virtual void visitTest(TestElement origin, TestPosition position)
        {
            if (_runContext.TestIsAtPosition(position))
            {
                _runContext.whileInState(new ActState(_runContext, origin), origin.Action);

                _runContext.GotoStateFinishing();
            }
        }

        public void visitIgnoreBecause(IgnoreElement origin, TestPosition position)
        {
        }

        public void visitExpect(ExpectElement origin, TestPosition position)
        {
            Expect.That(origin.Expectation);
        }

        public void visitWaitUntil(WaitUntilElement origin, TestPosition position)
        {
            Expect.Eventually(origin.Expectation, origin.WaitMaxMS, origin.WaitIncrementMS);
        }

        public void visitWithCategory(WithCategoryElement origin, TestPosition position)
        {
        }

        public void visitTrace(TraceElement origin, TestPosition position)
        {
            _runContext.AddTrace(origin.Message);
        }

        public void visitLeakDisposable(LeakDisposableElement origin, TestPosition position)
        {
            _runContext.LeakDisposable(origin.Disposable);
        }

        public virtual TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> origin, TestPosition position)
        {
            TArranged result = default(TArranged);

            _runContext.whileInState(new ArrangeState(_runContext, origin), delegate
            {
                result = origin.Action();
            });

            if (result is IDisposable)
            {
                _runContext.AddTeardownAction(delegate
                {
                    _runContext.whileInState(new CleanupState(_runContext, origin), delegate
                    {
                        _runContext.DisposeIfNotLeaked(result as IDisposable);
                    });
                });
            }

            return result;
        }
    }
}