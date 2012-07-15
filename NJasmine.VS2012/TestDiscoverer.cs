using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach(var source in sources)
            {
                Console.WriteLine("NJasmine.VS2012 source: " + source);
            }

            // LOL WAT DO
        }
    }
}
