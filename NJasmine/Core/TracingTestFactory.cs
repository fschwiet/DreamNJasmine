using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public class TracingTestFactory : INativeTestFactory
    {
        public IEnumerable<string> Names
        {
            get { return Contexts.Select(kvp => kvp.Key); }
        }

        public Dictionary<string, TestContext> Contexts = new Dictionary<string, TestContext>();

        public class TracingTest : INativeTest
        {
        }

        public INativeTest ForSuite(TestContext testContext, Action onetimeCleanup)
        {
            return new TracingTest();
        }

        public INativeTest ForTest(SharedContext sharedContext, TestContext testContext)
        {
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
    }
}
