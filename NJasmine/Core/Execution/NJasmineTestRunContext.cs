using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;

namespace NJasmine.Core.Execution
{
    class NJasmineTestRunContext
    {
        public ISpecPositionVisitor State { get; private set; }

        private readonly TestPosition _position;
        private PerFixtureSetupContext _fixtureSetupTeardown;
        private readonly List<Action> _allTeardowns;

        public NJasmineTestRunContext(TestPosition position, PerFixtureSetupContext fixtureSetupContext)
        {
            _position = position;
            _fixtureSetupTeardown = fixtureSetupContext;
            _allTeardowns = new List<Action>();
            State = new DiscoveryState(this);
        }

        public bool TestIsAncestorOfPosition(TestPosition position)
        {
            return _position.ToString().StartsWith(position.ToString());
        }

        public TArranged IncludeOneTimeSetup<TArranged>(TestPosition position)
        {
            var result = (TArranged)_fixtureSetupTeardown.DoOnetimeSetup(position);

            _fixtureSetupTeardown.IncludeCleanupFor(position);

            return result;
        }

        public void IncludeOneTimeCleanup(TestPosition position)
        {
            _fixtureSetupTeardown.IncludeCleanupFor(position);
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