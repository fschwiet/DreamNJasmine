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
                var nativeTestFactory = RunTestDiscovery(Assembly.Load(assemblyName), t => true);

                return nativeTestFactory.Names.ToArray();
            }
        }

        public class SpecRunner : MarshalByRefObject
        {
            public void RunTests(string assemblyName, string[] testNames, ITestResultListener listener)
            {
                var nativeTestFactory = RunTestDiscovery(Assembly.Load(assemblyName), t => true);

                foreach (var testContext in testNames.Select(name => nativeTestFactory.Contexts[name]))
                {
                    listener.NotifyStart(testContext);

                    List<string> traceMessages = new List<string>();

                    var result = SpecificationRunner.RunTest(testContext, null, traceMessages);

                    listener.NotifyEnd(testContext, result);
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

        protected static TracingTestFactory RunTestDiscovery(Assembly assembly, Func<Type, bool> typeFilter)
        {
            TracingTestFactory nativeTestFactory = new TracingTestFactory();

            var filteredTypes = assembly.GetTypes().Where(t => FixtureClassifier.IsTypeSpecification(t)).Where(typeFilter);

            foreach (var type in filteredTypes)
            {
                SpecificationBuilder.BuildTestFixture(type, nativeTestFactory);
            }
            return nativeTestFactory;
        }
    }
}
