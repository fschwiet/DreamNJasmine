using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using NJasmine.Core;
using NJasmine.Extras;
using NJasmine.Marshalled;

namespace NJasmine.VS2012
{
    [DefaultExecutorUri(TestDiscoverer.VSExecutorUri)]
    [ExtensionUri(TestDiscoverer.VSExecutorUri)]
    public class TestExecutor : ITestExecutor
    {
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            var sink = new TestExecutorSinkAdapter(frameworkHandle, tests);

            foreach(var group in tests.GroupBy(t => t.Source))
            {
                using (var appDomain = new AppDomainWrapper(group.Key))
                {
                    UsingAppDomain.RunTests(group.Key, appDomain, tests.Select(t => t.FullyQualifiedName).ToArray(), sink);
                }
            }
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }
    }
}
