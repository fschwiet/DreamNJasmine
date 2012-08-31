using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Extras;

namespace NJasmine.Core
{
    public class UsingAppDomain
    {
        public static string[] LoadTestNames(AppDomainWrapper appDomainWrapper, string dllPath)
        {
            var o = appDomainWrapper.CreateObject<Marshalled.Executor.SpecEnumerator>("NJasmine.dll");

            var result = o.GetTestNames(AssemblyName.GetAssemblyName(dllPath).FullName);

            return result;
        }

        public static void RunTests(string dllPath, AppDomainWrapper appDomainWrapper, string[] testNames, string explictlyIncluding, ITestResultListener sink)
        {
            var o = appDomainWrapper.CreateObject<Marshalled.Executor.SpecRunner>("NJasmine.dll");

            o.RunTests(AssemblyName.GetAssemblyName(dllPath).FullName, testNames, explictlyIncluding, sink);
        }
    }
}
