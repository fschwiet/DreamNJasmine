using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest
    {
        readonly TestPosition _position;

        public NJasmineTestSuite(string parentSuiteName, string name, TestPosition position) : base(parentSuiteName, name)
        {
            _position = position;
        }

        public TestPosition Position
        {
            get { return _position; }
        }
    }
}
