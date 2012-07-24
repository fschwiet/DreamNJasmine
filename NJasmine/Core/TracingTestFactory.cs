using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public class TracingTestFactory : INativeTestFactory
    {
        public List<string> Names
        {
            get
            {
                return Positions.Keys.ToList();
            }
        }

        public Dictionary<string, TestPosition> Positions = new Dictionary<string, TestPosition>(); 

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
            Positions[testContext.Name.FullName] = testContext.Position;
            return new TracingTest();
        }

        public INativeTest ForUnimplementedTest(TestContext testContext)
        {
            Names.Add(testContext.Name.FullName);
            Positions[testContext.Name.FullName] = testContext.Position;
            return new TracingTest();
        }

        public INativeTest ForFailingSuite(TestContext testContext, Exception exception)
        {
            return new TracingTest();
        }
    }
}
