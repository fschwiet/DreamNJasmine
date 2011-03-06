using System;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    public class DescribeState : ISpecPositionVisitor
    {
        private NJasmineExecutionContext _executionContext;

        public DescribeState(NJasmineExecutionContext executionContext)
        {
            _executionContext = executionContext;
        }

        public virtual void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (_executionContext.TestIsAncestorOfPosition(position))
            {
                action();
            }
        }

        public virtual TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _executionContext.IncludeOneTimeSetup<TArranged>(position);
        }

        public virtual void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _executionContext.IncludeOneTimeCleanup(position);
        }

        public virtual void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _executionContext.AddTeardownAction(delegate()
            {
                _executionContext.whileInState(new CleanupState(_executionContext, origin), action);
            });
        }

        public virtual void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (_executionContext.TestIsAtPosition(position))
            {
                _executionContext.whileInState(new ActState(_executionContext, origin), action);

                throw new NJasmineTestMethod.TestFinishedException();
            }
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }

        public virtual TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            TArranged result = default(TArranged);

            _executionContext.whileInState(new ArrangeState(_executionContext, origin), delegate
            {
                result = factory();
            });

            if (result is IDisposable)
            {
                _executionContext.AddTeardownAction(delegate
                {
                    _executionContext.whileInState(new CleanupState(_executionContext, origin), delegate
                    {
                        (result as IDisposable).Dispose();
                    });
                });
            }

            return result;
        }
    }
}