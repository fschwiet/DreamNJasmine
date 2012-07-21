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
        public INativeTest ForSuite(TestContext testContext, Action onetimeCleanup)
        {
            var nunitTest = new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, testContext.Position);
            
            ApplyNameToNUnitTest(testContext.Name, nunitTest);

            return new NativeTest(nunitTest);
        }

        public INativeTest ForTest(TestContext testContext, Func<SpecificationFixture> fixtureFactory)
        {
            var nunitTest = new NJasmineTestMethod(fixtureFactory, testContext.Position, testContext.GlobalSetupManager);
            
            ApplyNameToNUnitTest(testContext.Name, nunitTest);
            
            return new NativeTest(nunitTest);
        }

        public INativeTest ForUnimplementedTest(TestContext testContext)
        {
            var test = new NJasmineUnimplementedTestMethod(testContext.Position);
            ApplyNameToNUnitTest(testContext.Name, test);
            return new NativeTest(test);
        }

        public INativeTest ForFailingSuite(TestContext testContext, Exception exception)
        {
            var nunitTest = new NJasmineInvalidTestSuite(exception, testContext.Position);

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