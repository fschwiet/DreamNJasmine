using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NativeTest
    {
        readonly Func<Test> _factory;

        public NativeTest(Func<Test> factory)
        {
            _factory = factory;
        }

        public Test GetNative()
        {
            return _factory();
        }
    }

    public class NativeTestFactory
    {
        public static NativeTest ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NativeTest(() => new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position));
            return result;
        }

        public static NativeTest ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NativeTest(() => new NJasmineTestMethod(fixtureFactory, position, globalSetupManager));
            return result;
        }

        public static NativeTest ForUnimplementedTest(TestPosition position)
        {
            var result = new NativeTest(() => new NJasmineUnimplementedTestMethod(position));
            return result;
        }

        public static NativeTest ForFailingSuite(TestPosition position, Exception exception)
        {
            return new NativeTest(() => new NJasmineInvalidTestSuite(exception, position));
        }
    }

    public class BuildTest : NativeTestFactory
    {
        public static void AddChildrenToTest(Test result, List<INJasmineBuildResult> children)
        {
            foreach (var childTest in children)
            {
                (result as TestSuite).Add(childTest.GetNUnitResult());
            }
        }

        public static Test GetNUnitResultInternal(NJasmineBuilder nJasmineBuilder, NativeTest test)
        {
            Test result;

            result = test.GetNative();

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
