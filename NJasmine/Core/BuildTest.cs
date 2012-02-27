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
        readonly Test _test;

        public NativeTest(Test test)
        {
            _test = test;
        }

        public Test GetNative(TestBuilder builder)
        {
            ApplyResultToTest(builder);
            return _test;
        }

        public Test ApplyResultToTest(TestBuilder builder)
        {
            Test result = _test;

            result.TestName.Name = builder.Shortname;
            result.TestName.FullName = builder.FullName;
            result.SetMultilineName(builder.MultilineName);

            if (builder.ReasonIgnored != null)
            {
                result.RunState = RunState.Explicit;
                result.IgnoreReason = builder.ReasonIgnored;
            }

            foreach (var category in builder.Categories)
                result.Categories.Add(category);

            foreach (var child in builder.Children)
            {
                (result as TestSuite).Add(child.GetUnderlyingTest());
            }

            return result;
        }
    }

    public class NativeTestFactory
    {
        public static NativeTest ForSuite(TestPosition position, Action onetimeCleanup)
        {
            var result = new NativeTest(new NJasmineTestSuiteNUnit("hi", "there", onetimeCleanup, position));
            return result;
        }

        public static NativeTest ForTest(Func<SpecificationFixture> fixtureFactory, TestPosition position, GlobalSetupManager globalSetupManager)
        {
            var result = new NativeTest(new NJasmineTestMethod(fixtureFactory, position, globalSetupManager));
            return result;
        }

        public static NativeTest ForUnimplementedTest(TestPosition position)
        {
            var result = new NativeTest(new NJasmineUnimplementedTestMethod(position));
            return result;
        }

        public static NativeTest ForFailingSuite(TestPosition position, Exception exception)
        {
            return new NativeTest(new NJasmineInvalidTestSuite(exception, position));
        }
    }
}
