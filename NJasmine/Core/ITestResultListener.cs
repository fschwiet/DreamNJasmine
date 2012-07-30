using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public interface ITestResultListener
    {
        void NotifyStart(TestContext testContext);
        void NotifyEnd(TestContext testContext, TestResultShim TestResult);
    }
}