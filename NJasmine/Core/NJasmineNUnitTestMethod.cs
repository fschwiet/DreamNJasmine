using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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
