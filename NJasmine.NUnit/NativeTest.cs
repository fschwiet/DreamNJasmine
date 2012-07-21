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
            if (builder.ReasonIgnored != null)
            {
                _test.RunState = RunState.Explicit;
                _test.IgnoreReason = builder.ReasonIgnored;
            }
            
            foreach (var category in builder.Categories)
            {
                _test.Categories.Add(category);

                if (category.IndexOfAny(new char[] { ',', '!', '+', '-' }) >= 0)
                {
                    _test.RunState = RunState.NotRunnable;
                    _test.IgnoreReason = "Category name must not contain ',', '!', '+' or '-'";
                }
            }

            foreach (var child in builder.Children)
            {
                (_test as TestSuite).Add((child.GetUnderlyingTest() as NativeTest).GetNative(child));
            }
            return _test;
        }
    }
}
