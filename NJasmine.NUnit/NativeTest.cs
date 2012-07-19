using NJasmine.Core;
using NUnit.Core;

namespace NJasmine.NUnit
{
    public class NativeTest : INativeTest
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
            if (builder.ReasonIgnored != null)
            {
                _test.RunState = RunState.Explicit;
                _test.IgnoreReason = builder.ReasonIgnored;
            }
            
            foreach (var category in builder.Categories)
                NUnitFrameworkUtil.ApplyCategoryToTest(category, _test);

            foreach (var child in builder.Children)
            {
                (_test as TestSuite).Add((child.GetUnderlyingTest() as NativeTest).GetNative(child));
            }
        }
    }
}
