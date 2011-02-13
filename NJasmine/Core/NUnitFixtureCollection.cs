using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NUnitFixtureCollection
    {
        readonly NUnitFixtureCollection _parent;
        List<TestPosition> _positions = new List<TestPosition>();  // storing position keys separately by order of existence
        List<TestPosition> _setupPositions = new List<TestPosition>();
        List<TestPosition> _teardownPositions = new List<TestPosition>();
        Dictionary<TestPosition, Func<object>> _fixtureSetupMethods = new Dictionary<TestPosition, Func<object>>();
        Dictionary<TestPosition, object> _fixtureSetupResults = new Dictionary<TestPosition, object>();
        Dictionary<TestPosition, Action> _fixtureTeardownMethods = new Dictionary<TestPosition, Action>();

        
        Dictionary<TestPosition, Type> _fixtures = new Dictionary<TestPosition, Type>();
        Dictionary<TestPosition, object> _instances = new Dictionary<TestPosition, object>();

        public Exception ExceptionFromOnetimeSetup { get; private set; }

        public NUnitFixtureCollection()
        {
            _parent = null;
        }

        public NUnitFixtureCollection(NUnitFixtureCollection parent)
        {
            _parent = parent;
        }

        public void DoOnetimeSetUp()
        {
            try
            {
                foreach (var record in _setupPositions.Select(p => 
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
            foreach (var action in _teardownPositions.Select(p => _fixtureTeardownMethods[p]).Reverse())
            {
                action();
            }
        }

        public void DoSetUp(TestPosition position)
        {
            var instance = GetInstance(position);

            RunMethodsWithAttribute(instance, NUnitFramework.SetUpAttribute);
        }

        public void DoTearDown(TestPosition position)
        {
            var instance = GetInstance(position);

            RunMethodsWithAttribute(instance, NUnitFramework.TearDownAttribute);
        }

        public void AddFixture(TestPosition position, Type type)
        {

            object fixtureInstance = null;
                        if (_positions.Contains(position))                throw new InvalidOperationException();
            _positions.Add(position);

            AddFixtureSetup(position, delegate
            {
                fixtureInstance = type.GetConstructor(new Type[0]).Invoke(EmptyObjectArray);
                _instances[position] = fixtureInstance;
                RunMethodsWithAttribute(fixtureInstance, NUnitFramework.FixtureSetUpAttribute);
                return fixtureInstance;
            });

            AddFixtureTearDown(position, delegate
            {
                RunMethodsWithAttribute(fixtureInstance, NUnitFramework.FixtureTearDownAttribute);
            });
        }

        public void AddFixtureSetup<TArranged>(TestPosition position, Func<TArranged> action)
        {
            _setupPositions.Add(position);
            _fixtureSetupMethods[position] = delegate
            {
                return action();
            };
        }

        public void AddFixtureTearDown(TestPosition position, Action action)
        {
            _teardownPositions.Add(position);
            _fixtureTeardownMethods[position] = action;
        }

        public object GetInstance(TestPosition position)
        {
            if (_positions.Contains(position))
            {
                return _instances[position];
            }
            else if (_parent != null)
            {
                return _parent.GetInstance(position);
            }
            else
            {
                throw new InvalidOperationException("NUnit fixture instance requested not found.");
            }
        }

        void RunMethodsWithAttribute(object instance, string attribute)
        {
            var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                     attribute, true);

            foreach (var method in methods)
            {
                method.Invoke(instance, EmptyObjectArray);
            }
        }

        readonly static object[] EmptyObjectArray = new object[0];

        public object GetSetupResult(TestPosition position)
        {
            if (_setupPositions.Contains(position))
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
