using NJasmine.Core;
using NUnit.Core;

namespace NJasmine.NUnit
{
    public class NativeTest : INativeTestBuilder
    {
        readonly Test _test;

        public NativeTest(Test test)
        {
            _test = test;
        }

        public object GetNative(TestBuilder builder)
        {
            _test.TestName.Name = builder.Shortname;
            _test.TestName.FullName = builder.FullName;
            TestExtensions.SetMultilineName(_test, builder.MultilineName);

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

            return _test;
        }
    }
}
