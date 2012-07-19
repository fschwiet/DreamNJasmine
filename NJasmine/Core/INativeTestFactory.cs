using System;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        INativeTest ForSuite(TestName testName, TestPosition position, Action onetimeCleanup);
        INativeTest ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager);
        INativeTest ForUnimplementedTest(TestName name, TestPosition position);
        INativeTest ForFailingSuite(TestPosition position, Exception exception);
    }
}