using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class NJasmineTestRunContext
    {
        public ISpecPositionVisitor State { get; private set; }

        private readonly TestPosition _position;
        private readonly IGlobalSetupManager _globalSetup;
        private readonly List<Action> _allTeardowns;

        public NJasmineTestRunContext(TestPosition position, IGlobalSetupManager globalSetup)
        {
            _position = position;
            _globalSetup = globalSetup;
            _allTeardowns = new List<Action>();
            State = new DiscoveryState(this);
        }

        public bool PositionIsAncestorOfContext(TestPosition position)
        {
            return position.IsAncestorOf(_position);
        }

        public TArranged IncludeOneTimeSetup<TArranged>(TestPosition position)
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

        public bool TestIsAtPosition(TestPosition position)
        {
            return position.ToString() == _position.ToString();
        }
    }
}