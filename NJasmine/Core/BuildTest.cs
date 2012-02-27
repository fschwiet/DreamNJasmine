using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
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
}
