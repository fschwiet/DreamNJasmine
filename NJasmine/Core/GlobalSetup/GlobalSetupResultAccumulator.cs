using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NJasmine.Core.GlobalSetup
{
    public class GlobalSetupResultAccumulator
    {
        List<KeyValuePair<TestPosition, object>> _setupResults;
        List<KeyValuePair<TestPosition, Action>> _cleanupActions;

        public GlobalSetupResultAccumulator()
        {
            _cleanupActions = new List<KeyValuePair<TestPosition, Action>>();
            _setupResults = new List<KeyValuePair<TestPosition, object>>();
        }

        public void UnwindForPosition(TestPosition position, Action<Exception> errorHandler)
        {
            List<Action> toRun = new List<Action>();

            for (var i = _cleanupActions.Count() - 1; i >= 0; i--)
            {
                var kvp = _cleanupActions[i];

                if (!kvp.Key.IsOnPathTo(position))
                {
                    toRun.Add(kvp.Value);
                    _cleanupActions.RemoveAt(i);
                }
            }

            try
            {
                foreach (var action in toRun)
                    action();
            }
            catch (Exception e)
            {
                errorHandler(e);
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

        public void UnwindAll(Action<Exception> errorHandler)
        {
            var toCleanup = _cleanupActions;
            _cleanupActions = new List<KeyValuePair<TestPosition, Action>>();

            toCleanup.Reverse();

            foreach(var kvp in toCleanup)
            {
                try
                {
                    kvp.Value();
                }
                catch (Exception e)
                {
                    errorHandler(e);
                }
            }

            _setupResults = new List<KeyValuePair<TestPosition, object>>();
        }

        public void AddCleanupAction(TestPosition position, Action action)
        {
            _cleanupActions.Add(new KeyValuePair<TestPosition, Action>(position, action));
        }

        public void AddSetupResult(TestPosition position, object value)
        {
            if (value is IDisposable)
            {
                AddCleanupAction(
                    position,
                    delegate
                    {
                        (value as IDisposable).Dispose();
                    });
            }

            _setupResults.Add(new KeyValuePair<TestPosition, object>(position, value));
        }

        public object InternalGetSetupResultAt(TestPosition position)
        {
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
    }
}