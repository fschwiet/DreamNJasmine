using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;

namespace NJasmine.Core
{
    public class PerFixtureSetupContext
    {
        readonly PerFixtureSetupContext _parent;
        Dictionary<TestPosition, Func<object>> _fixtureSetupMethods = new Dictionary<TestPosition, Func<object>>();
        Dictionary<TestPosition, Exception> _fixtureSetupExceptions = new Dictionary<TestPosition, Exception>();
        Dictionary<TestPosition, object> _fixtureSetupResults = new Dictionary<TestPosition, object>();
        Dictionary<TestPosition, Action> _fixtureTeardownMethods = new Dictionary<TestPosition, Action>();

        private List<TestPosition> _pendingCleanups = new List<TestPosition>();

        public PerFixtureSetupContext()
        {
            _parent = null;
        }

        public PerFixtureSetupContext(PerFixtureSetupContext parent)
        {
            _parent = parent;
        }

        public object DoOnetimeSetup(TestPosition position)
        {
            if (_fixtureSetupMethods.ContainsKey(position))
            {
                if (_fixtureSetupExceptions.ContainsKey(position))
                {
                    throw _fixtureSetupExceptions[position];
                }

                if (!_fixtureSetupResults.ContainsKey(position))
                {
                    try
                    {
                        _fixtureSetupResults[position] = _fixtureSetupMethods[position]();
                    }
                    catch (Exception e)
                    {
                        _fixtureSetupExceptions[position] = e;
                        
                        throw;
                    }
                }

                return _fixtureSetupResults[position];
            }
            else
            {
                return _parent.DoOnetimeSetup(position);
            }
        }

        public void IncludeCleanupFor(TestPosition position)
        {
            if (_fixtureTeardownMethods.ContainsKey(position))
            {
                if (!_pendingCleanups.Contains(position))
                {
                    if (!_fixtureTeardownMethods.ContainsKey(position))
                        throw new Exception("bugbug");

                    _pendingCleanups.Add(position);
                }
            }
            else
            {
                _parent.IncludeCleanupFor(position);
            }
        }

        public void DoCleanupFor(TestPosition position)
        {
            for(var i = _pendingCleanups.Count - 1; i >= 0; i--)
            {
                var positionToClean = _pendingCleanups[i];

                if (positionToClean.Parent.IsParentOf(position))
                {
                    break;
                }

                _pendingCleanups.RemoveAt(i);
                _fixtureTeardownMethods[positionToClean]();
            }
        }

        public void DoAllCleanup()
        {
            var pendingCleanups = _pendingCleanups;
            _pendingCleanups = new List<TestPosition>();

            pendingCleanups.Reverse();

            foreach (var position in pendingCleanups)
            {
                _fixtureTeardownMethods[position]();
            }
        }

        public void AddFixtureSetup<TArranged>(TestPosition position, Func<TArranged> action)
        {
            if (_fixtureSetupMethods.ContainsKey(position))
                throw new InvalidOperationException();

            _fixtureSetupMethods[position] = delegate
            {
                return action();
            };

            AddFixtureTearDown(position, delegate
            {
                _fixtureSetupExceptions.Remove(position);

                // setup won't have run if there is a prior error)
                if (_fixtureSetupResults.ContainsKey(position))
                {
                    var disposeable = _fixtureSetupResults[position] as IDisposable;

                    if (disposeable != null)
                        disposeable.Dispose();

                    _fixtureSetupResults.Remove(position);
                }
            });
        }

        public void AddFixtureTearDown(TestPosition position, Action action)
        {
            if (_fixtureTeardownMethods.ContainsKey(position))
                throw new InvalidOperationException();

            _fixtureTeardownMethods[position] = action;
        }
    }
}
