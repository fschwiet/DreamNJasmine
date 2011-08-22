using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NJasmine.Core.GlobalSetup
{
    public class GlobalSetupVisitorBase
    {
        List<KeyValuePair<TestPosition, Action>> _cleanupResults;
        List<KeyValuePair<TestPosition, object>> _setupResults;

        protected GlobalSetupVisitorBase()
        {
            _cleanupResults = new List<KeyValuePair<TestPosition, Action>>();
            _setupResults = new List<KeyValuePair<TestPosition, object>>();
        }

        protected void CleanupToPrepareFor(TestPosition position, Action<Exception> errorHandler)
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

        protected void AddCleanupAction(TestPosition position, Action action)
        {
            _cleanupResults.Add(new KeyValuePair<TestPosition, Action>(position, action));
        }

        protected void UnwindAccumulated(Action<Exception> errorHandler)
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
                    errorHandler(e);
                }
            }

            _setupResults = new List<KeyValuePair<TestPosition, object>>();
        }

        protected void AddSetupResult(TestPosition position, object value)
        {
            _setupResults.Add(new KeyValuePair<TestPosition, object>(position, value));
        }

        protected object InternalGetSetupResultAt(TestPosition position)
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