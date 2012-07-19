using System;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        INativeTest ForSuite(TestName name, TestPosition position, Action onetimeCleanup);
        INativeTest ForTest(TestName name, Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager);
        INativeTest ForUnimplementedTest(TestName name, TestPosition position);
        INativeTest ForFailingSuite(TestName name, TestPosition position, Exception exception);
    }
}