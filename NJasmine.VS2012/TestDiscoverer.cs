using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using NJasmine.Core;
using NJasmine.Extras;
using NJasmine.Marshalled;

namespace NJasmine.VS2012
{
    [FileExtension(".dll")]
    [FileExtension(".exe")]
    [DefaultExecutorUri(TestDiscoverer.VSExecutorUri)]
    [ExtensionUri(TestDiscoverer.VSExecutorUri)]
    public class TestDiscoverer : ITestDiscoverer
    {
        public const string VSExecutorUri = "executor:http://nuget.org/packages/NJasmine";

        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            foreach (var source in sources.Where(s => IsAlongsideNJasmineDll(s)))
            {
                using(var appDomain = new AppDomainWrapper(source))
                {
                    foreach (var result in UsingAppDomain.LoadTestNames(appDomain, source))
                    {
                        discoverySink.SendTestCase(new TestCase(result, new Uri(VSExecutorUri), source));
                    }
                }
            }
        }

        static bool IsAlongsideNJasmineDll(string assemblyFileName)
        {
            string xunitPath = Path.Combine(Path.GetDirectoryName(assemblyFileName), "njasmine.dll");
            return File.Exists(xunitPath);
        }
    }
}
