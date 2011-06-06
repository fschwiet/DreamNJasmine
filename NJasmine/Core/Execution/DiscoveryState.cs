using System;
using System.Linq.Expressions;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;

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

                throw new NJasmineTestMethod.TestFinishedException();
            }
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
        }

        public void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
            ExpectationChecker.Expect(expectation);
        }

        public void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            ExpectationChecker.WaitUntil(expectation, totalWaitMs, waitIncrementMs);
        }

        public void visitWithCategory(SpecElement origin, string category, TestPosition position)
        {
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
                        (result as IDisposable).Dispose();
                    });
                });
            }

            return result;
        }
    }
}