using System;

namespace NJasmineTests.Export
{
    public interface ITestResult
    {
        ITestResult withFailureMessage(string expectedMessage);
        ITestResult thatSucceeds();
        ITestResult thatErrors();
        ITestResult thatIsNotRunnable();
        ITestResult thatFails();
        ITestResult thatFailsInAnUnspecifiedManner();
        ITestResult withDetailedMessageThat(Action<string> handler);
        ITestResult withCategories(params string[] categories);
    }
}