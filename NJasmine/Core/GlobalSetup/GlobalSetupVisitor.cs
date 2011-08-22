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
        SpecElement? _executingPastDiscovery;
        TestPosition _currentTestPosition;

        protected TestPosition _targetPosition;
        protected TestPosition _existingErrorPosition;
        protected Exception _existingError;

        GlobalSetupResultAccumulator _setupResultAccumulator;

        public GlobalSetupVisitor(LolMutex runningLock)
        {
            _runningLock = runningLock;
            _executingPastDiscovery = null;
            _setupResultAccumulator = new GlobalSetupResultAccumulator();
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
                _existingError = e;
                _existingErrorPosition = new TestPosition(0);
                ReportError();
            }
        }

        public bool SetTargetPosition(TestPosition position, out Exception existingError)
        {
            existingError = null;

            if (_existingError != null && _existingErrorPosition != null && _existingErrorPosition.IsOnPathTo(position))
                existingError = _existingError;

            _targetPosition = position;
            return existingError != null || _targetPosition.Equals(_currentTestPosition);
        }

        public void FinishCleanup()
        {
            _setupResultAccumulator.UnwindAll(e =>
            {
                _existingError = e;
                _existingErrorPosition = new TestPosition(0);
                ReportError();
            });

            _currentTestPosition = new TestPosition();
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (position.IsAncestorOf(_targetPosition))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    _existingError = e;
                    _existingErrorPosition = position;
                    ReportError();
                }
            }

            while (position.Equals(_targetPosition))
            {
                _currentTestPosition = position;

                _runningLock.PassAndWaitForTurn();
            }

            _setupResultAccumulator.UnwindForPosition(_targetPosition, HandleError);
        }

        protected void ReportError()
        {
            while (_existingError != null
                  && _existingErrorPosition != null
                  && _existingErrorPosition.IsOnPathTo(_targetPosition))
            {
                _runningLock.PassAndWaitForTurn();
            }
        }

        private void HandleError(Exception e)
        {
            _existingError = e;
            _existingErrorPosition = new TestPosition(0);
            ReportError();
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
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
                        _existingError = e;
                        _existingErrorPosition = position;
                        ReportError();
                    }

                    if (result is IDisposable)
                    {
                        _setupResultAccumulator.AddCleanupAction(
                            position, 
                            delegate
                            {
                                (result as IDisposable).Dispose();
                            });
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

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);

            if (position.IsOnPathTo(_targetPosition))
            {
                _setupResultAccumulator.AddCleanupAction(position, action);
            }
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
            return default(TArranged);
        }

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);

            _currentTestPosition = position;

            while (position.Equals(_targetPosition))
            {
                _runningLock.PassAndWaitForTurn();
            }

            _setupResultAccumulator.UnwindForPosition(_targetPosition, HandleError);
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
            if (_executingPastDiscovery.HasValue)
            {
                Expect.That(expectation);
            }
        }

        public void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            if (_executingPastDiscovery.HasValue)
            {
                Expect.Eventually(expectation, totalWaitMs, waitIncrementMs);
            }
        }

        public void visitWithCategory(SpecElement origin, string category, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitTrace(SpecElement origin, string message, TestPosition position)
        {
        }

        private void CheckNotAlreadyPastDiscovery(SpecElement origin)
        {
            if (_executingPastDiscovery.HasValue)
                throw new Exception("Attempted to call " + origin + " within " + _executingPastDiscovery.Value + ".");
        }

        public object GetSetupResultAt(TestPosition position)
        {
            if (!position.IsOnPathTo(_targetPosition))
                throw new InvalidProgramException();

            return _setupResultAccumulator.InternalGetSetupResultAt(position);
        }
    }
}
