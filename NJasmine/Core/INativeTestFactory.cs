using System;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        INativeTest ForSuite(TestContext testContext, Action onetimeCleanup);
        INativeTest ForTest(TestContext testContext, Func<SpecificationFixture> fixtureFactory);
        INativeTest ForUnimplementedTest(TestContext testContext);
        INativeTest ForFailingSuite(TestContext testContext, Exception exception);
    }
}