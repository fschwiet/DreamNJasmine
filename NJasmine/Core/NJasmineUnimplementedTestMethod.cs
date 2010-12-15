using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineUnimplementedTestMethod : TestMethod, INJasmineTest
    {
        public NJasmineUnimplementedTestMethod(TestPosition position) 
            : base(((Action)delegate() { }).Method)
        {
            Position = position;
        }

        public override void RunTestMethod(TestResult testResult)
        {
            testResult.Skip("Specification not implemented.");
        }

        public TestPosition Position { get; private set; }

    }
}
