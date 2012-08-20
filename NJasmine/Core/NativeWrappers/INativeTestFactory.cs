using NJasmine.Core.Discovery;

namespace NJasmine.Core.NativeWrappers
{
    public interface INativeTestFactory
    {
        void SetRoot(INativeTest test);
        INativeTest ForSuite(TestContext testContext);
        INativeTest ForTest(SharedContext sharedContext, TestContext testContext);
    }
}