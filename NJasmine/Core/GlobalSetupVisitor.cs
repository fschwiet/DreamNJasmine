using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core
{
    public class GlobalSetupVisitor : ISpecPositionVisitor
    {
        private readonly AutoResetEvent _runningLock;
        TestPosition _targetPosition;
        SpecElement? _executingPastDiscovery;
        TestPosition _currentTestPosition;
        List<KeyValuePair<TestPosition, Action>> _cleanupResults;
        List<KeyValuePair<TestPosition, object>> _setupResults;

        TestPosition _existingErrorPosition;

        Exception _existingError;

        public GlobalSetupVisitor(AutoResetEvent runningLock)
        {
            _runningLock = runningLock;
            _executingPastDiscovery = null;
            _cleanupResults = new List<KeyValuePair<TestPosition, Action>>();
            _setupResults = new List<KeyValuePair<TestPosition, object>>();
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
            var toCleanup = _cleanupResults;
            _cleanupResults = new List<KeyValuePair<TestPosition, Action>>();

            toCleanup.Reverse();

            foreach(var kvp in toCleanup)
            {
                try
                {
                    kvp.Value();
                }
                catch (Exception e)
                {
                    _existingError = e;
                    _existingErrorPosition = new TestPosition(0);
                    ReportError();
                }
            }

            _setupResults = new List<KeyValuePair<TestPosition, object>>();

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
                _runningLock.Set();
                Thread.Sleep(0);
                _runningLock.WaitOne(-1);
            }
            CleanupToPrepareFor(_targetPosition);
        }

        private void CleanupToPrepareFor(TestPosition position)
        {
            List<Action> toRun = new List<Action>();

            for (var i = _cleanupResults.Count() - 1; i >= 0; i--)
            {
                var kvp = _cleanupResults[i];

                if (!kvp.Key.IsOnPathTo(position))
                {
                    toRun.Add(kvp.Value);
                    _cleanupResults.RemoveAt(i);
                }
            }

            try
            {
                foreach (var action in toRun)
                    action();
            }
            catch (Exception e)
            {
                _existingError = e;
                _existingErrorPosition = new TestPosition(0);
                ReportError();
            }

            for(var i = _setupResults.Count() - 1; i >= 0; i--)
            {
                var kvp = _setupResults[i];

                if (!kvp.Key.IsOnPathTo(position))
                {
                    _setupResults.RemoveAt(i);
                }
            }
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
                        _cleanupResults.Add(new KeyValuePair<TestPosition, Action>(
                            position, 
                            delegate
                            {
                                (result as IDisposable).Dispose();
                            }));

                    }

                    _setupResults.Add(new KeyValuePair<TestPosition, object>(position, result));
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
                _cleanupResults.Add(new KeyValuePair<TestPosition, Action>(position, action));
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
                _runningLock.Set();
                Thread.Sleep(0);
                _runningLock.WaitOne(-1);
            }
            CleanupToPrepareFor(_targetPosition);
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
        }

        public void visitWithCategory(SpecElement origin, string category, TestPosition position)
        {
            CheckNotAlreadyPastDiscovery(origin);
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

            try
            {
                return _setupResults.First(kvp => kvp.Key != null && kvp.Key.Equals(position)).Value;
            }
            catch (Exception e)
            {
                throw new InvalidProgramException(String.Format("Could not find setup result for position {0}, had results for {1}.",
                    position.ToString() ?? "null", String.Join(", ", _setupResults.Select(sr => sr.Key.ToString()).ToArray())), e);
            }
        }

        void ReportError()
        {
            while(_existingError != null 
                && _existingErrorPosition != null
                && _existingErrorPosition.IsOnPathTo(_targetPosition))
            {
                _runningLock.Set();
                Thread.Sleep(0);
                _runningLock.WaitOne(-1);
            }
        }
    }
}
