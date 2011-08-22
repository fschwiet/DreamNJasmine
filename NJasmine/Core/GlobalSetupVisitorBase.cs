using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NJasmine.Core
{
    public class GlobalSetupVisitorBase
    {
        protected AutoResetEvent _runningLock;
        protected TestPosition _targetPosition;
        protected List<KeyValuePair<TestPosition, Action>> _cleanupResults;
        protected List<KeyValuePair<TestPosition, object>> _setupResults;
        protected TestPosition _existingErrorPosition;
        protected Exception _existingError;

        protected GlobalSetupVisitorBase(AutoResetEvent runningLock)
        {
            _runningLock = runningLock;
            _cleanupResults = new List<KeyValuePair<TestPosition, Action>>();
            _setupResults = new List<KeyValuePair<TestPosition, object>>();
        }

        protected void CleanupToPrepareFor(TestPosition position)
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

        protected void ReportError()
        {
            while(_existingError != null 
                  && _existingErrorPosition != null
                  && _existingErrorPosition.IsOnPathTo(_targetPosition))
            {
                WaitForTurn();
            }
        }

        protected void WaitForTurn()
        {
            _runningLock.Set();
            Thread.Sleep(0);
            _runningLock.WaitOne(-1);
        }
    }
}