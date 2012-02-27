using System;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        INativeTestBuilder ForSuite(TestPosition position, Action onetimeCleanup);
        INativeTestBuilder ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager);
        INativeTestBuilder ForUnimplementedTest(TestPosition position);
        INativeTestBuilder ForFailingSuite(TestPosition position, Exception exception);
    }
}