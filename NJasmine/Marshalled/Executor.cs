using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.Discovery;

namespace NJasmine.Marshalled
{
    public class Executor : MarshalByRefObject
    {
        public class SpecEnumerator : MarshalByRefObject
        {
            public string[] GetTestNames(string assemblyName)
            {
                var assembly = Assembly.Load(assemblyName);

                List<string> results = new List<string>();

                foreach (var type in assembly.GetTypes().Where(t => FixtureClassifier.IsTypeSpecification(t)))
                {
                    Console.WriteLine("type " + type.Name);

                    TracingTestFactory nativeTestFactory = new TracingTestFactory();

                    SpecificationBuilder.BuildTestFixture(type, nativeTestFactory);

                    results.AddRange(nativeTestFactory.Names.ToArray());
                }

                return results.ToArray();
            }
        }

        public class AppSettingLoader : MarshalByRefObject
        {
            public string Get(string name)
            {
                return ConfigurationManager.AppSettings.Get(name);
            }
        }
    }
}
