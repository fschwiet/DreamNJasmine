using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;

namespace NJasmine.Core.GlobalSetup
{
    public class GlobalSetupVisitor : ISpecPositionVisitor
    {
        readonly LolMutex _runningLock;
        SpecificationElement _executingPastDiscovery;
        SpecificationElement _executingCleanup;
        TestPosition _currentTestPosition;
        ErrorAccumulator _errorAccumulator;
        protected TestPosition _targetPosition;

        GlobalSetupResultAccumulator _setupResultAccumulator;
        TraceTracker _traceTracker;

        public GlobalSetupVisitor(LolMutex runningLock)
        {
            _runningLock = runningLock;
            _executingPastDiscovery = null;
            _executingCleanup = null;
            _errorAccumulator = new ErrorAccumulator();
            _setupResultAccumulator = new GlobalSetupResultAccumulator();
            _traceTracker = new TraceTracker();
        }

        public void RunFixture(Func<SpecificationFixture> fixtureFactory)
        {
            var fixture = fixtureFactory();
            fixture.CurrentPosition = TestPosition.At(0);
            fixture.Visitor = this;
            try
            {
                fixture.Run();
            }
            catch (Exception e)
            {
                ReportError(TestPosition.At(0), e);
            }
        }

        public bool SetTargetPosition(TestPosition position, out Exception existingError)
        {
            _targetPosition = position;

            existingError = _errorAccumulator.GetErrorForPosition(position);

            return existingError != null || _targetPosition.Equals(_currentTestPosition);
        }

        public void FinishCleanup()
        {
            _setupResultAccumulator.UnwindAll(e =>
            {
                ReportError(TestPosition.At(0), e);
            });

            _traceTracker.UnwindAll();

            _currentTestPosition = TestPosition.At();
        }

        protected void ReportError(TestPosition position, Exception error)
        {
            _errorAccumulator.AddError(position, error);

            while (error != null
                   && position != null
                   && position.IsOnPathTo(_targetPosition))
            {
                _runningLock.PassAndWaitForTurn();
            }
        }

        private void CheckNotAlreadyPastDiscovery(SpecificationElement origin)
        {
            if (_executingPastDiscovery != null)
                throw new Exception("Attempted to call " + origin + " within " + _executingPastDiscovery + ".");
        }

        public object GetSetupResultAt(TestPosition position)
        {
            if (!position.IsOnPathTo(_targetPosition))
                throw new InvalidProgramException();

            return _setupResultAccumulator.InternalGetSetupResultAt(position);
        }

        public IEnumerable<string> GetCurrentTraceMessages()
        {
            return _traceTracker.GetCurrentTraceMessages();
        }

        public void visitFork(ForkElement element, TestPosition position)
        {
            if (position.IsAncestorOf(_targetPosition))
            {
                try
                {
                    element.Action();
                }
                catch (Exception e)
                {
                    ReportError(position, e);
                }
            }

            while (position.Equals(_targetPosition))
            {
                _currentTestPosition = position;

                _runningLock.PassAndWaitForTurn();
            }

            _setupResultAccumulator.UnwindForPosition(_targetPosition, e => ReportError(TestPosition.At(0), e));
            _traceTracker.UnwindToPosition(_targetPosition);
        }

        public TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);

            TArranged result = default(TArranged);

            if (position.IsOnPathTo(_targetPosition))
            {
                _executingPastDiscovery = element;

                try
                {
                    try
                    {
                        result = element.Action();
                    }
                    catch (Exception e)
                    {
                        ReportError(position, e);
                    }

                    _setupResultAccumulator.AddSetupResult(position, result);
                }
                finally
                {
                    _executingPastDiscovery = null;
                }
            }

            return result;
        }

        public void visitAfterAll(AfterAllElement element, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);

            if (position.IsOnPathTo(_targetPosition))
            {
                _setupResultAccumulator.AddCleanupAction(position, delegate {
                    _executingCleanup = element;
                    element.Action();
                    _executingCleanup = null;
                });
            }
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);
            return default(TArranged);
        }

        public void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);
        }

        public void visitTest(TestElement element, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);

            _currentTestPosition = position;

            while (position.Equals(_targetPosition))
            {
                _runningLock.PassAndWaitForTurn();
            }

            _setupResultAccumulator.UnwindForPosition(_targetPosition, e => {
                                                                                ReportError(TestPosition.At(0), e);
            });

            _traceTracker.UnwindToPosition(_targetPosition);
        }

        public void visitIgnoreBecause(IgnoreElement element, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);
        }

        public void visitExpect(ExpectElement element, TestPosition position)
        {
            if (_executingPastDiscovery != null)
            {
                Expect.That(element.Expectation);
            }
        }

        public void visitWaitUntil(WaitUntilElement element, TestPosition position)
        {
            if (_executingPastDiscovery != null)
            {
                Expect.Eventually(element.Expectation, element.WaitMaxMS, element.WaitIncrementMS);
            }
        }

        public void visitWithCategory(WithCategoryElement element, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(element);
        }

        public void visitTrace(TraceElement element, TestPosition position)
        {
            if (_executingPastDiscovery != null)
            {
                _traceTracker.AddTraceEntry(position, element.Message);
            }

            if (_executingCleanup != null)
            {
                throw new Exception("Attempted to call " + element + "() from within " + _executingCleanup);
            }
        }

        public void visitLeakDisposable(LeakDisposableElement element, TestPosition position)
        {
            _setupResultAccumulator.LeakDisposable(element.Disposable);
        }
    }
}
