using System;
using System.Linq.Expressions;
using NJasmine.Core.Discovery;
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

        public virtual void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (_runContext.PositionIsAncestorOfContext(position))
            {
                action();
            }
        }

        public virtual TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _runContext.GetSetupResultAt<TArranged>(position);
        }

        public virtual void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
        }

        public virtual void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _runContext.AddTeardownAction(delegate()
            {
                _runContext.whileInState(new CleanupState(_runContext, origin), action);
            });
        }

        public virtual void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (_runContext.TestIsAtPosition(position))
            {
                _runContext.whileInState(new ActState(_runContext, origin), action);

                _runContext.GotoStateFinishing();
            }
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
        }

        public void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
            Expect.That(expectation);
        }

        public void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            Expect.Eventually(expectation, totalWaitMs, waitIncrementMs);
        }

        public void visitWithCategory(SpecElement origin, string category, TestPosition position)
        {
        }

        public void visitTrace(SpecElement origin, string message, TestPosition position)
        {
            _runContext.AddTrace(message);
        }

        public void visitLeakDisposable(SpecElement origin, IDisposable disposable, TestPosition position)
        {
            _runContext.LeakDisposable(disposable);
        }

        public virtual TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            TArranged result = default(TArranged);

            _runContext.whileInState(new ArrangeState(_runContext, origin), delegate
            {
                result = factory();
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