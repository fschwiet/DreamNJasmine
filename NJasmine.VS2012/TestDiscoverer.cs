using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace NJasmine.VS2012
{
    public class TestDiscoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            Console.WriteLine("running NJasmine TestDiscoverer");

            foreach(var source in sources)
            {
                Console.WriteLine("NJasmine.VS2012 source: " + source);
            }

            foreach (var source in sources.Where(s => IsAlongsideNJasmineDll(s)))
            {
                var appDomain = CreateAppDomain(Path.GetFullPath(source), null, true);
                
            }

            // LOL WAT DO
        }

        static bool IsAlongsideNJasmineDll(string assemblyFileName)
        {
            string xunitPath = Path.Combine(Path.GetDirectoryName(assemblyFileName), "njasmine.dll");
            return File.Exists(xunitPath);
        }

        static AppDomain CreateAppDomain(string assemblyFilename, string configFilename, bool shadowCopy)
        {
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = Path.GetDirectoryName(assemblyFilename);
            setup.ApplicationName = Guid.NewGuid().ToString();

            if (shadowCopy)
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = setup.ApplicationBase;
                setup.CachePath = Path.Combine(Path.GetTempPath(), setup.ApplicationName);
            }

            setup.ConfigurationFile = configFilename;

            return AppDomain.CreateDomain(setup.ApplicationName, null, setup, new PermissionSet(PermissionState.Unrestricted));
        }
    }
}
