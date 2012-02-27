using System;
using NJasmine.Core;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.NUnit
{
    public class NativeTestFactory : INativeTestFactory
    {
        public INativeTestBuilder ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NativeTest(new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position));
            return result;
        }

        public INativeTestBuilder ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NativeTest(new NJasmineTestMethod(fixtureFactory, position, globalSetupManager));
            return result;
        }

        public INativeTestBuilder ForUnimplementedTest(TestPosition position)
        {
            var result = new NativeTest(new NJasmineUnimplementedTestMethod(position));
            return result;
        }

        public INativeTestBuilder ForFailingSuite(TestPosition position, Exception exception)
        {
            return new NativeTest(new NJasmineInvalidTestSuite(exception, position));
        }
    }
}