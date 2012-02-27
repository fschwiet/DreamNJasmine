using System.Reflection;
using NJasmine.Core;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineNUnitTestMethod : TestMethod, INJasmineTest
    {
        public NJasmineNUnitTestMethod(MethodInfo method, TestPosition position) : base(method)
        {
            Position = position;
        }

        public TestPosition Position { get; private set; }
    }
}
