using NUnit.Core;

namespace NJasmine.NUnit
{
    public static class TestExtensions
    {
        public static string GetMultilineName(this ITest test)
        {
            if (test.Properties.Contains(MultilineNameProperty))
                return (string)test.Properties[MultilineNameProperty];
            else
                return test.TestName.FullName;
        }

        public static void SetMultilineName(this ITest test, string value)
        {
            test.Properties[MultilineNameProperty] = value;
        }

        public const string MultilineNameProperty = "MultilineName";
    }
}
