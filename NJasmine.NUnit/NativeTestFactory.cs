using System;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;
using NJasmine.NUnit.TestElements;
using NUnit.Core;
using TestName = NJasmine.Core.TestName;

namespace NJasmine.NUnit
{
    public class NativeTestFactory : INativeTestFactory
    {
        public INativeTest ForSuite(TestName name, TestPosition position, Action onetimeCleanup)
        {
            var nunitTest = new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position);
            
            ApplyNameToNUnitTest(name, nunitTest);

            return new NativeTest(nunitTest);
        }

        public INativeTest ForTest(TestName name, Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var nunitTest = new NJasmineTestMethod(fixtureFactory, position, globalSetupManager);
            
            ApplyNameToNUnitTest(name, nunitTest);
            
            return new NativeTest(nunitTest);
        }

        public INativeTest ForUnimplementedTest(TestName name, TestPosition position)
        {
            var test = new NJasmineUnimplementedTestMethod(position);
            ApplyNameToNUnitTest(name, test);
            return new NativeTest(test);
        }

        public INativeTest ForFailingSuite(TestName name, TestPosition position, Exception exception)
        {
            var nunitTest = new NJasmineInvalidTestSuite(exception, position);

            ApplyNameToNUnitTest(name, nunitTest);
            
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