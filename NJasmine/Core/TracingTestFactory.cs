using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public class TracingTestFactory : INativeTestFactory, IDisposable
    {
        public IEnumerable<string> Names
        {
            get { return Contexts.Select(kvp => kvp.Key); }
        }

        public SpecificationBuilder.ExecutionContext ExecutionContext;

        public Dictionary<string, Func<SpecificationFixture>> FixtureBuilders =
            new Dictionary<string, Func<SpecificationFixture>>(); 
        public Dictionary<string, TestContext> Contexts = new Dictionary<string, TestContext>();

        public class TracingTest : INativeTest
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

        public INativeTest ForUnimplementedTest(TestContext testContext)
        {
            Contexts[testContext.Name.FullName] = testContext;
            return new TracingTest();
        }

        public INativeTest ForFailingSuite(TestContext testContext, Exception exception)
        {
            return new TracingTest();
        }

        public void Dispose()
        {
            if (ExecutionContext != null)
            {
                ExecutionContext.SetupManager.Close();
                ExecutionContext = null;
            }
        }
    }
}
