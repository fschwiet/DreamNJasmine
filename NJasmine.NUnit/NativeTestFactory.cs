using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;
using NJasmine.NUnit.TestElements;
using NUnit.Core;
using TestName = NJasmine.Core.TestName;

namespace NJasmine.NUnit
{
    public class NativeTestFactory : INativeTestFactory
    {
        public INativeTest RootTest;

        public void SetRoot(INativeTest test)
        {
            RootTest = test;
        }

        public INativeTest ForSuite(TestContext testContext)
        {
            var nunitTest = new NJasmineTestSuiteNUnit(testContext);
            
            ApplyNameToNUnitTest(testContext.Name, nunitTest);

            return new NativeTest(nunitTest);
        }

        public INativeTest ForTest(SharedContext sharedContext, TestContext testContext)
        {
            var nunitTest = new NJasmineTestMethod(sharedContext.FixtureFactory, testContext);
            
            ApplyNameToNUnitTest(testContext.Name, nunitTest);
            
            return new NativeTest(nunitTest);
        }

        public INativeTest ForFailingSuite(TestContext testContext, Exception exception)
        {
            var nunitTest = new NJasmineInvalidTestSuite(exception);

            ApplyNameToNUnitTest(testContext.Name, nunitTest);
            
            return new NativeTest(nunitTest);
        }

        static void ApplyNameToNUnitTest(TestName testName, Test nJasmineTestSuiteNUnit)
        {
            nJasmineTestSuiteNUnit.TestName.Name = testName.Shortname;
            nJasmineTestSuiteNUnit.TestName.FullName = testName.FullName;
            TestExtensions.SetMultilineName(nJasmineTestSuiteNUnit, testName.MultilineName);
        }
    }
}