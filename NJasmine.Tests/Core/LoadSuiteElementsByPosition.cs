using System.Collections.Generic;
using NJasmine;
using NJasmine.Core;
using NUnit.Core;

namespace NJasmineTests.Core
{
    public class LoadSuiteElementsByPosition
    {
        static public Dictionary<TestPosition, INJasmineTest> ForType<TFixture>()
        {
            var sut = new NJasmineSuiteBuilder();

            var rootTest = sut.BuildFrom(typeof(TFixture));

            Dictionary<TestPosition, INJasmineTest> testsByPosition = new Dictionary<TestPosition, INJasmineTest>();

            LoadTestsByPosition(testsByPosition, rootTest);
            return testsByPosition;
        }

        static void LoadTestsByPosition(Dictionary<TestPosition, INJasmineTest> tests, ITest test)
        {
            if (test is INJasmineTest)
            {
                tests[(test as INJasmineTest).Position] = test as INJasmineTest;
            }

            if (test is TestSuite)
            {
                foreach (ITest childTest in (test as TestSuite).Tests)
                {
                    LoadTestsByPosition(tests, childTest);
                }
            }
        }
    }
}