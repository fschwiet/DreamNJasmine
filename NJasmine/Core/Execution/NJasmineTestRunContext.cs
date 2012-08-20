using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core.Execution
{
    public class NJasmineTestRunContext
    {
        public ISpecPositionVisitor State { get; private set; }

        private readonly TestPosition _targetTestPosition;
        private readonly IGlobalSetupManager _globalSetup;
        private readonly List<Action> _allTeardowns;
        private readonly List<string> _traces;
        private readonly List<IDisposable> _leakedDisposables;

        public NJasmineTestRunContext(TestPosition targetTestPosition, IGlobalSetupManager globalSetup, List<string> traceMessages)
        {
            _targetTestPosition = targetTestPosition;
            _globalSetup = globalSetup;
            _allTeardowns = new List<Action>();
            _traces = traceMessages;
            _leakedDisposables = new List<IDisposable>();

            State = new DiscoveryState(this);
        }

        public bool PositionIsAncestorOfContext(TestPosition position)
        {
            return position.IsAncestorOf(_targetTestPosition);
        }

        public TArranged GetSetupResultAt<TArranged>(TestPosition position)
        {
            return _globalSetup.GetSetupResultAt<TArranged>(position);
        }

        public void IncludeOneTimeCleanup(TestPosition position)
        {
        }

        public void AddTeardownAction(Action action)
        {
            _allTeardowns.Add(action);
        }

        public void AddTrace(string message)
        {
            _traces.Add(message);
        }

        public void RunAllPerTestTeardowns()
        {
            _allTeardowns.Reverse();

            foreach (var action in _allTeardowns)
            {
                action();
            }

            _allTeardowns.Clear();
        }

        public void whileInState(ISpecPositionVisitor state, Action action)
        {
            var originalState = State;
            State = state;
            try
            {
                action();
            }
            finally
            {
                State = originalState;
            }
        }

        public void GotoStateFinishing()
        {
            State = new FinishingState(this);
        }

        public bool TestIsAtPosition(TestPosition position)
        {
            return position.ToString() == _targetTestPosition.ToString();
        }

        public void LeakDisposable(IDisposable disposable)
        {
            _leakedDisposables.Add(disposable);
        }

        public void DisposeIfNotLeaked(IDisposable disposable)
        {
            if (disposable != null && !_leakedDisposables.Contains(disposable))
            {
                disposable.Dispose();
            }
        }
    }
}