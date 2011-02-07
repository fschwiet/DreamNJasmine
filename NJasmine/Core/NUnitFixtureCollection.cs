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
                foreach (var instance in _positions.Select(p => GetInstance(p)))
                {
                    var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                             NUnitFramework.FixtureSetUpAttribute, true);

                    foreach (var method in methods)
                    {
                        method.Invoke(instance, EmptyObjectArray);
                    }
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
            foreach (var instance in _positions.Select(p => GetInstance(p)).Reverse())
            {
                var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                         NUnitFramework.FixtureTearDownAttribute, true);

                foreach (var method in methods)
                {
                    method.Invoke(instance, EmptyObjectArray);
                }
            }
        }

        public void DoSetUp(TestPosition position)
        {
            var instance = GetInstance(position);

            var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                        NUnitFramework.SetUpAttribute, true);

            foreach (var method in methods)
            {
                method.Invoke(instance, EmptyObjectArray);
            }
        }

        public void DoTearDown(TestPosition position)
        {
            var instance = GetInstance(position);

            var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                         NUnitFramework.TearDownAttribute, true);
            foreach (var method in methods)
            {
                method.Invoke(instance, EmptyObjectArray);
            }
        }


        public void AddFixture(TestPosition position, Type type)
        {
            if (_fixtures.ContainsKey(position))
            {
                throw new InvalidOperationException();
            }

            _positions.Add(position);
            _fixtures[position] = type;
        }

        public object GetInstance(TestPosition position)
        {
            object instance = null; 

            if (_positions.Contains(position))
            {
                if (!_instances.TryGetValue(position, out instance))
                {
                    var type = _fixtures[position];

                    instance = type.GetConstructor(new Type[0]).Invoke(EmptyObjectArray);
                    _instances[position] = instance;
                }

                return instance;
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
        
        readonly static object[] EmptyObjectArray = new object[0];
    }
}
