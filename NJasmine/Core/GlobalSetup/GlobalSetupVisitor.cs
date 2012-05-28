using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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

        protected TestPosition _targetPosition;
        protected TestPosition _existingErrorPosition;
        protected Exception _existingError;

        GlobalSetupResultAccumulator _setupResultAccumulator;
        TraceTracker _traceTracker;

        public GlobalSetupVisitor(LolMutex runningLock)
        {
            _runningLock = runningLock;
            _executingPastDiscovery = null;
            _executingCleanup = null;
            _setupResultAccumulator = new GlobalSetupResultAccumulator();
            _traceTracker = new TraceTracker();
        }

        public void RunFixture(Func<SpecificationFixture> fixtureFactory)
        {
            var fixture = fixtureFactory();
            fixture.CurrentPosition = new TestPosition(0);
            fixture.Visitor = this;
            try
            {
                fixture.Run();
            }
            catch (Exception e)
            {
                ReportError(new TestPosition(0), e);
            }
        }

        public bool SetTargetPosition(TestPosition position, out Exception existingError)
        {
            _targetPosition = position;

            existingError = null;

            if (_existingError != null && _existingErrorPosition != null && _existingErrorPosition.IsOnPathTo(position))
                existingError = _existingError;

            return existingError != null || _targetPosition.Equals(_currentTestPosition);
        }

        public void FinishCleanup()
        {
            _setupResultAccumulator.UnwindAll(e =>
            {
                ReportError(new TestPosition(0), e);
            });

            _traceTracker.UnwindAll();

            _currentTestPosition = new TestPosition();
        }

        public void visitFork(SpecificationElement origin, string description, Action action, TestPosition position)
        {
            if (position.IsAncestorOf(_targetPosition))
            {
                try
                {
                    action();
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

            _setupResultAccumulator.UnwindForPosition(_targetPosition, e => ReportError(new TestPosition(0), e));
            _traceTracker.UnwindToPosition(_targetPosition);
        }

        protected void ReportError(TestPosition position, Exception error)
        {
            _existingError = error;
            _existingErrorPosition = position;

            while (error != null
                  && position != null
                  && position.IsOnPathTo(_targetPosition))
            {
                _runningLock.PassAndWaitForTurn();
            }
        }

        public TArranged visitBeforeAll<TArranged>(SpecificationElement origin, Func<TArranged> action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);

            TArranged result = default(TArranged);

            if (position.IsOnPathTo(_targetPosition))
            {
                _executingPastDiscovery = origin;

                try
                {
                    try
                    {
                        result = action();
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

        public void visitAfterAll(SpecificationElement origin, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);

            if (position.IsOnPathTo(_targetPosition))
            {
                _setupResultAccumulator.AddCleanupAction(position, delegate {
                    _executingCleanup = origin;
                    action();
                    _executingCleanup = null;
                });
            }
        }

        public TArranged visitBeforeEach<TArranged>(SpecificationElement origin, Func<TArranged> factory, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
            return default(TArranged);
        }

        public void visitAfterEach(SpecificationElement origin, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitTest(SpecificationElement origin, string description, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);

            _currentTestPosition = position;

            while (position.Equals(_targetPosition))
            {
                _runningLock.PassAndWaitForTurn();
            }

            _setupResultAccumulator.UnwindForPosition(_targetPosition, e => {
                                                                                ReportError(new TestPosition(0), e);
            });

            _traceTracker.UnwindToPosition(_targetPosition);
        }

        public void visitIgnoreBecause(SpecificationElement origin, string reason, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitExpect(SpecificationElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
            if (_executingPastDiscovery != null)
            {
                Expect.That(expectation);
            }
        }

        public void visitWaitUntil(SpecificationElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            if (_executingPastDiscovery != null)
            {
                Expect.Eventually(expectation, totalWaitMs, waitIncrementMs);
            }
        }

        public void visitWithCategory(SpecificationElement origin, string category, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitTrace(SpecificationElement origin, string message, TestPosition position)
        {
            if (_executingPastDiscovery != null)
            {
                _traceTracker.AddTraceEntry(position, message);
            }

            if (_executingCleanup != null)
            {
                throw new Exception("Attempted to call " + origin + "() from within " + _executingCleanup);
            }
        }

        public void visitLeakDisposable(SpecificationElement origin, IDisposable disposable, TestPosition position)
        {
            _setupResultAccumulator.LeakDisposable(disposable);
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
    }
}
