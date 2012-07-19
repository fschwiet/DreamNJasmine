using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.VS2012
{
    public class NativeTestFactory : INativeTestFactory
    {
        public INativeTest ForSuite(TestName name, TestPosition position, Action onetimeCleanup)
        {
            //return new TestCase();
            throw new NotImplementedException();
        }

        public INativeTest ForTest(TestName name, Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            throw new NotImplementedException();
        }

        public INativeTest ForUnimplementedTest(TestName testName, TestPosition position)
        {
            throw new NotImplementedException();
        }

        public INativeTest ForFailingSuite(TestName name, TestPosition position, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
