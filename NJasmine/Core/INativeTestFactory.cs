using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        void SetRoot(INativeTest test);
        INativeTest ForSuite(TestContext testContext);
        INativeTest ForTest(SharedContext sharedContext, TestContext testContext);
        INativeTest ForFailingSuite(TestContext testContext, Exception exception);
    }
}