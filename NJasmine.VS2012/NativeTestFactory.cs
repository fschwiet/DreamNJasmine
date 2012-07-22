using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.VS2012
{
    public class NativeTestFactory : INativeTestFactory
    {
        public INativeTest ForSuite(TestContext testContext, Action onetimeCleanup)
        {
            //return new TestCase();
            throw new NotImplementedException();
        }

        public INativeTest ForTest(SharedContext sharedContext, TestContext testContext)
        {
            throw new NotImplementedException();
        }

        public INativeTest ForUnimplementedTest(TestContext testContext)
        {
            throw new NotImplementedException();
        }

        public INativeTest ForFailingSuite(TestContext testContext, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
