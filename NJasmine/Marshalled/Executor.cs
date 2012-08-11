using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Extras;

namespace NJasmine.Marshalled
{
    public class Executor : MarshalByRefObject
    {
        public class SpecEnumerator : MarshalByRefObject
        {
            public string[] GetTestNames(string assemblyName)
            {
                return GetTestNames(Assembly.Load(assemblyName), t => true);
            }

            public static string[] GetTestNames(Assembly assembly, Func<Type, bool> typeFilter)
            {
                List<string> results = new List<string>();

                var filteredTypes = assembly.GetTypes().Where(t => FixtureClassifier.IsTypeSpecification(t)).Where(typeFilter);

                foreach (var type in filteredTypes)
                {
                    TracingTestFactory nativeTestFactory = new TracingTestFactory();

                    SpecificationBuilder.BuildTestFixture(type, nativeTestFactory);

                    results.AddRange(nativeTestFactory.Names.ToArray());
                }

                return results.ToArray();
            }
        }

        public class SpecRunner : MarshalByRefObject
        {
            public void RunTests(string[] testNames, ITestResultListener listener)
            {
                foreach (var name in testNames)
                {
                    listener.NotifyStart(name);
                    listener.NotifyEnd(name);
                }
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
