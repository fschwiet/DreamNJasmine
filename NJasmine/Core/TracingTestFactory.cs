using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public class TracingTestFactory : INativeTestFactory
    {
        public List<string> Names = new List<string>(); 

        public class TracingTest : INativeTest
        {
        }

        public INativeTest ForSuite(TestContext testContext, Action onetimeCleanup)
        {
            return new TracingTest();
        }

        public INativeTest ForTest(SharedContext sharedContext, TestContext testContext)
        {
            Names.Add(testContext.Name.FullName);
            return new TracingTest();
        }

        public INativeTest ForUnimplementedTest(TestContext testContext)
        {
            Names.Add(testContext.Name.FullName);
            return new TracingTest();
        }

        public INativeTest ForFailingSuite(TestContext testContext, Exception exception)
        {
            return new TracingTest();
        }
    }
}
