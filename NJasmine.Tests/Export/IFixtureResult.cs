namespace NJasmineTests.Export
{
    public interface IFixtureResult
    {
        IFixtureResult succeeds();
        IFixtureResult failed();
        void hasTrace(string expectedTrace);
        ITestResult hasTest(string name);
        ITestResult hasTestWithFullName(string name);
        ISuiteResult hasSuite(string name);
        string[] withStackTraces();
        bool WasRanByNUnit();
    }
}