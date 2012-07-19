using System;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        INativeTest ForSuite(TestName testName, TestPosition position, Action onetimeCleanup);
        INativeTest ForTest(TestName name, Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager);
        INativeTest ForUnimplementedTest(TestName name, TestPosition position);
        INativeTest ForFailingSuite(TestName failingSuiteName, TestPosition position, Exception exception);
    }
}