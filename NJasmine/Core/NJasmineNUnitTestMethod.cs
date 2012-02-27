using System.Reflection;
using NUnit.Core;

namespace NJasmine.Core
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
