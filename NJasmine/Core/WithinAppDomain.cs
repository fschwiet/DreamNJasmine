using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Extras;

namespace NJasmine.Core
{
    public class WithinAppDomain
    {
        public static string[] LoadTestNames(string assemblyName, AppDomainWrapper appDomainWrapper)
        {
            var o = appDomainWrapper.CreateObject<Marshalled.Executor.SpecEnumerator>("NJasmine.dll");

            var result = o.GetTestNames(assemblyName);

            return result;
        }

        public static void RunTests(string dllPath, AppDomainWrapper appDomainWrapper, string[] testNames, ITestResultListener sink)
        {
            var o = appDomainWrapper.CreateObject<Marshalled.Executor.SpecRunner>("NJasmine.dll");

            o.RunTests(dllPath, testNames, sink);
        }
    }
}
