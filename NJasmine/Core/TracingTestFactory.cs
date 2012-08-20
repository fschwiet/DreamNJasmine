using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public class TracingTestFactory : INativeTestFactory, IDisposable
    {
        public IEnumerable<string> Names
        {
            get { return Contexts.Select(kvp => kvp.Key); }
        }

        public GlobalSetupManager GlobalSetupManager;

        public Dictionary<string, Func<SpecificationFixture>> FixtureBuilders =
            new Dictionary<string, Func<SpecificationFixture>>(); 
        public Dictionary<string, TestContext> Contexts = new Dictionary<string, TestContext>();

        public class TracingTest : INativeTest
        {
            public void AddCategory(string category)
            {
            }

            public void AddChild(TestBuilder test)
            {
            }

            public void MarkTestIgnored(string reasonIgnored)
            {
            }

            public void MarkTestInvalid(string reason)
            {
            }

            public void MarkTestFailed(Exception exception)
            {
            }
        }

        public void SetRoot(INativeTest test)
        {
        }

        public INativeTest ForSuite(TestContext testContext)
        {
            return new TracingTest();
        }

        public INativeTest ForTest(SharedContext sharedContext, TestContext testContext)
        {
            FixtureBuilders[testContext.Name.FullName] = sharedContext.FixtureFactory;
            Contexts[testContext.Name.FullName] = testContext;
            return new TracingTest();
        }

        public void Dispose()
        {
            if (GlobalSetupManager != null)
            {
                GlobalSetupManager.Close();
                GlobalSetupManager = null;
            }
        }
    }
}
