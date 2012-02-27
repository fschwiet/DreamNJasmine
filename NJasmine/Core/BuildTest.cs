using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public class BuildTest
    {
        public static NJasmineBuildResult ForUnimplementedTest(TestPosition position)
        {
            var result = new NJasmineBuildResult(() => new NJasmineUnimplementedTestMethod(position));
            return result;
        }

        public static NJasmineBuildResult ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NJasmineBuildResult(() => new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position));
            return result;
        }

        public static NJasmineBuildResult ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NJasmineBuildResult(() => new NJasmineTestMethod(fixtureFactory, position, globalSetupManager));
            return result;
        }
    }
}
