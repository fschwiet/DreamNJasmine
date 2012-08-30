using NJasmine.Core.Discovery;

namespace NJasmine.Core.NativeWrappers
{
    public interface INativeTestFactory
    {
        void SetRoot(INativeTest test);
        INativeTest ForSuite(FixtureContext fixtureContext, TestContext testContext);
        INativeTest ForTest(FixtureContext fixtureContext, TestContext testContext);
    }
}