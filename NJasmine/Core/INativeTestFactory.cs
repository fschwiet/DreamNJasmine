using System;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public interface INativeTestFactory
    {
        NativeTest ForSuite(TestPosition position, Action onetimeCleanup);
        NativeTest ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager);
        NativeTest ForUnimplementedTest(TestPosition position);
        NativeTest ForFailingSuite(TestPosition position, Exception exception);
    }
}