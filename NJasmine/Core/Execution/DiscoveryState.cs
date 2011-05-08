using System;
using System.Linq.Expressions;
using System.Threading;
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
            if (_runContext.TestIsAncestorOfPosition(position))
            {
                action();
            }
        }

        public virtual TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _runContext.IncludeOneTimeSetup<TArranged>(position);
        }

        public virtual void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _runContext.IncludeOneTimeCleanup(position);
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

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
        }

        public void visitExpect(Expression<Func<bool>> expectation, TestPosition position)
        {
            PowerAssert.PAssert.IsTrue(expectation);
        }

        public void visitWaitUntil(Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            var expectationChecker = expectation.Compile();

            DateTime finishTime = DateTime.UtcNow.AddMilliseconds(totalWaitMs);

            bool passing;

            while (!(passing = expectationChecker()) && DateTime.UtcNow < finishTime)
            {
                Thread.Sleep(waitIncrementMs);
            }

            if (!passing)
                PowerAssert.PAssert.IsTrue(expectation);
        }

        public void visitWithCategory(string category, TestPosition position)
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