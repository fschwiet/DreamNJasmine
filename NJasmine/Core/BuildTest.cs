using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;

namespace NJasmine.Core
{
    public class BuildTest
    {
        public static NJasmineBuilder ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NJasmineBuilder(() => new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position));
            return result;
        }

        public static NJasmineBuilder ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NJasmineBuilder(() => new NJasmineTestMethod(fixtureFactory, position, globalSetupManager));
            return result;
        }

        public static NJasmineBuilder ForUnimplementedTest(TestPosition position)
        {
            var result = new NJasmineBuilder(() => new NJasmineUnimplementedTestMethod(position));
            return result;
        }

        public static NJasmineBuilder ForFailingSuite(TestPosition position, Exception exception)
        {
            return new NJasmineBuilder(() => new NJasmineInvalidTestSuite(exception, position));
        }

        public static void AddChildrenToTest(Test result, List<INJasmineBuildResult> children)
        {
            foreach (var childTest in children)
            {
                (result as TestSuite).Add(childTest.GetNUnitResult());
            }
        }

        public static Test GetNUnitResultInternal(NJasmineBuilder nJasmineBuilder, Func<Test> creationStrategy)
        {
            Test result;

            result = creationStrategy();

            result.TestName.Name = nJasmineBuilder.Shortname;
            result.TestName.FullName = nJasmineBuilder.FullName;
            result.SetMultilineName(nJasmineBuilder.MultilineName);

            if (nJasmineBuilder.ReasonIgnored != null)
            {
                result.RunState = RunState.Explicit;
                result.IgnoreReason = nJasmineBuilder.ReasonIgnored;
            }

            foreach (var category in nJasmineBuilder.Categories)
                result.Categories.Add(category);

            AddChildrenToTest(result, nJasmineBuilder.Children);

            return result;
        }
    }
}
