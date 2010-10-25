using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        readonly TestPosition _position;

        public NJasmineTestMethod(MethodInfo method, TestPosition position) : base(method)
        {
            _position = position;
        }

        public TestPosition Position
        {
            get { return _position; }
        }
    }
}
