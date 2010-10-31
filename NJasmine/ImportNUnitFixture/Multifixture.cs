using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core;
using NUnit.Core;

namespace NJasmine.ImportNUnitFixture
{
    public class Multifixture
    {
        List<TestPosition> _positions = new List<TestPosition>();  // storing position keys separately by order of existence
        Dictionary<TestPosition, Type> _fixtures = new Dictionary<TestPosition, Type>();
        Dictionary<TestPosition, object> _instances = new Dictionary<TestPosition, object>();
        
        public void DoOnetimeSetUp()
        {
            foreach(var instance in _positions.Select(p => GetInstance(p)))
            {
                var methods = NUnit.Core.Reflect.GetMethodsWithAttribute(instance.GetType(),
                                                                         NUnitFramework.FixtureSetUpAttribute, true);

                foreach(var method in methods)
                {
                    method.Invoke(instance, EmptyObjectArray);
                }
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

        public void AddFixture(TestPosition position, Type type)
        {
            if (_fixtures.ContainsKey(position))
            {
                throw new InvalidOperationException();
            }

            _positions.Add(position);
            _fixtures[position] = type;
        }

        public Type GetFixture(TestPosition position)
        {
            return _fixtures[position];
        }

        public object GetInstance(TestPosition position)
        {
            object instance = null; 

            if (!_instances.TryGetValue(position, out instance))
            {
                var type = GetFixture(position);

                instance = type.GetConstructor(new Type[0]).Invoke(EmptyObjectArray);
                _instances[position] = instance;
            }

            return instance;
        }

        readonly static object[] EmptyObjectArray = new object[0];
    }
}
