using System;

namespace NJasmineTests.Export
{
    public interface ISuiteResult
    {
        ISuiteResult thatsInconclusive();
        ISuiteResult thatSucceeds();
        ISuiteResult thatHasNoResults();
        ISuiteResult hasTest(string expectedName, Action<ITestResult> handler);
        ISuiteResult withCategories(params string[] categories);
        ISuiteResult hasSuite(string name);
        ISuiteResult doesNotHaveTestContaining(string skipped);
    }
}