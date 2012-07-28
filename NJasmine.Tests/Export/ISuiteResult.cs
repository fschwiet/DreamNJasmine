namespace NJasmineTests.Export
{
    public interface ISuiteResult
    {
        ISuiteResult thatsInconclusive();
        ISuiteResult thatSucceeds();
        ISuiteResult thatHasNoResults();
        ITestResult hasTest(string expectedName);
        ISuiteResult withCategories(params string[] categories);
        ISuiteResult hasSuite(string name);
    }
}