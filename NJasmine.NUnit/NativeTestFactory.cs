using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;
using NJasmine.Core.NativeWrappers;
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

        public INativeTest ForSuite(FixtureContext fixtureContext, TestContext testContext)
        {
            var nunitTest = new NJasmineTestSuiteNUnit(testContext);
            
            ApplyNameToNUnitTest(testContext.Name, nunitTest);

            return new NativeTest(nunitTest, testContext.Name);
        }

        public INativeTest ForTest(FixtureContext fixtureContext, TestContext testContext)
        {
            var nunitTest = new NJasmineTestMethod(testContext);
            
            ApplyNameToNUnitTest(testContext.Name, nunitTest);

            return new NativeTest(nunitTest, testContext.Name);
        }
        
        static void ApplyNameToNUnitTest(TestName testName, Test nJasmineTestSuiteNUnit)
        {
            nJasmineTestSuiteNUnit.TestName.Name = testName.Shortname;
            nJasmineTestSuiteNUnit.TestName.FullName = testName.FullName;
            TestExtensions.SetMultilineName(nJasmineTestSuiteNUnit, testName.MultilineName);
        }
    }
}