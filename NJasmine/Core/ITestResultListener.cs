using NJasmine.Core.Discovery;

namespace NJasmine.Core
{
    public interface ITestResultListener
    {
        void NotifyStart(string testFullName);
        void NotifyEnd(string testFullName, TestResultShim TestResult);
    }
}