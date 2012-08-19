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
            foreach (var category in builder.Categories)
            {
                _test.Categories.Add(category);

                if (category.IndexOfAny(new char[] { ',', '!', '+', '-' }) >= 0)
                {
                    _test.RunState = RunState.NotRunnable;
                    _test.IgnoreReason = "Category name must not contain ',', '!', '+' or '-'";
                }
            }

            return _test;
        }

        public void AddChild(TestBuilder test)
        {
            (_test as global::NUnit.Core.TestSuite).Add((test.GetUnderlyingTest() as NativeTest).GetNative(test));
        }

        public void SetIgnoreReason(string reasonIgnored)
        {
            _test.RunState = RunState.Explicit;
            _test.IgnoreReason = reasonIgnored;
        }
    }
}
