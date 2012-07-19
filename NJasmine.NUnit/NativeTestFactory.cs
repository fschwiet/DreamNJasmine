using System;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;
using NJasmine.NUnit.TestElements;

namespace NJasmine.NUnit
{
    public class NativeTestFactory : INativeTestFactory
    {
        public INativeTest ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NativeTest(new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position));
            return result;
        }

        public INativeTest ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NativeTest(new NJasmineTestMethod(fixtureFactory, position, globalSetupManager));
            return result;
        }

        public INativeTest ForUnimplementedTest(TestName name, TestPosition position)
        {
            var result = new NativeTest(new NJasmineUnimplementedTestMethod(position));
            return result;
        }

        public INativeTest ForFailingSuite(TestPosition position, Exception exception)
        {
            return new NativeTest(new NJasmineInvalidTestSuite(exception, position));
        }
    }
}