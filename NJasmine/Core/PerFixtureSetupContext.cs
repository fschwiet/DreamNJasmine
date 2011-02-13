using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;

namespace NJasmine.Core
{
    public class PerFixtureSetupContext
    {
        readonly PerFixtureSetupContext _parent;
        List<TestPosition> _fixtureSetupPositions = new List<TestPosition>();
        List<TestPosition> _fixtureTeardownPositions = new List<TestPosition>();
        Dictionary<TestPosition, Func<object>> _fixtureSetupMethods = new Dictionary<TestPosition, Func<object>>();
        Dictionary<TestPosition, object> _fixtureSetupResults = new Dictionary<TestPosition, object>();
        Dictionary<TestPosition, Action> _fixtureTeardownMethods = new Dictionary<TestPosition, Action>();

        public Exception ExceptionFromOnetimeSetup { get; private set; }

        public PerFixtureSetupContext()
        {
            _parent = null;
        }

        public PerFixtureSetupContext(PerFixtureSetupContext parent)
        {
            _parent = parent;
        }

        public void DoOnetimeSetUp()
        {
            try
            {
                foreach (var record in _fixtureSetupPositions.Select(p => 
                    new { Position = p, Action = _fixtureSetupMethods[p]}))
                {
                    _fixtureSetupResults[record.Position] = record.Action();
                }
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                ExceptionFromOnetimeSetup = e.InnerException;
            }
            catch (Exception e)
            {
                ExceptionFromOnetimeSetup = e;
            }
        }

        public void DoOnetimeTearDown()
        {
            foreach (var action in _fixtureTeardownPositions.Select(p => _fixtureTeardownMethods[p]).Reverse())
            {
                action();
            }
        }

        public void AddFixtureSetup<TArranged>(TestPosition position, Func<TArranged> action)
        {
            if (_fixtureSetupPositions.Contains(position))
                throw new InvalidOperationException();

            _fixtureSetupPositions.Add(position);
            _fixtureSetupMethods[position] = delegate
            {
                return action();
            };

            AddFixtureTearDown(position, delegate
            {
                // setup won't have run if there is a prior error
                if (!_fixtureSetupResults.ContainsKey(position))
                    return;

                var disposeable = _fixtureSetupResults[position] as IDisposable;

                if (disposeable != null)
                    disposeable.Dispose();
            });
        }

        public void AddFixtureTearDown(TestPosition position, Action action)
        {
            if (_fixtureTeardownPositions.Contains(position))
                throw new InvalidOperationException();

            _fixtureTeardownPositions.Add(position);
            _fixtureTeardownMethods[position] = action;
        }

        public object GetSetupResult(TestPosition position)
        {
            if (_fixtureSetupPositions.Contains(position))
            {
                return _fixtureSetupResults[position];
            }
            else if (_parent != null)
            {
                return _parent.GetSetupResult(position);
            }
            else
            {
                throw new InvalidOperationException("Attempted to find undefined setup result.");
            }
        }
    }
}
