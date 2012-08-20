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
                using(var nativeTestFactory = RunTestDiscovery(Assembly.Load(assemblyName), t => true))
                {
                    return nativeTestFactory.Names.ToArray();
                }
            }
        }

        public class SpecRunner : MarshalByRefObject
        {
            public void RunTests(string assemblyName, string[] testNames, ITestResultListener listener)
            {
                using(var nativeTestFactory = RunTestDiscovery(Assembly.Load(assemblyName), t => true))
                {
                    foreach(var name in testNames)
                    {
                        var testContext = nativeTestFactory.Contexts[name];
                        var testFixtureBuilder = nativeTestFactory.FixtureBuilders[name];

                        listener.NotifyStart(testContext.Name.FullName);

                        List<string> traceMessages = new List<string>();

                        var result = SpecificationRunner.RunTest(testContext, testFixtureBuilder, traceMessages);

                        listener.NotifyEnd(testContext.Name.FullName, result);
                    }                    
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

        protected static GenericTestFactory RunTestDiscovery(Assembly assembly, Func<Type, bool> typeFilter)
        {
            GenericTestFactory nativeTestFactory = new GenericTestFactory();

            var filteredTypes = assembly.GetTypes().Where(t => FixtureClassifier.IsTypeSpecification(t)).Where(typeFilter);

            foreach (var type in filteredTypes)
            {
                nativeTestFactory.GlobalSetupManager = SpecificationBuilder.BuildTestFixture(type, nativeTestFactory);
            }
            return nativeTestFactory;
        }
    }
}
