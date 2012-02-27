using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;

namespace NJasmine.Core
{
    public interface INativeTestBuilder
    {
        void ApplyResultToTest(TestBuilder builder);
    }

    public class NativeTest : INativeTestBuilder
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

        public void ApplyResultToTest(TestBuilder builder)
        {
            _test.TestName.Name = builder.Shortname;
            _test.TestName.FullName = builder.FullName;
            _test.SetMultilineName(builder.MultilineName);

            if (builder.ReasonIgnored != null)
            {
                _test.RunState = RunState.Explicit;
                _test.IgnoreReason = builder.ReasonIgnored;
            }

            foreach (var category in builder.Categories)
                _test.Categories.Add(category);

            foreach (var child in builder.Children)
            {
                (_test as TestSuite).Add((child.GetUnderlyingTest() as NativeTest).GetNative(child));
            }
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
