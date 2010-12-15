using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineUnimplementedTestMethod : TestMethod, INJasmineTest
    {
        public NJasmineUnimplementedTestMethod(string containingTestElementFullName, string containingTestName, TestPosition position) 
            : base(((Action)delegate() { }).Method)
        {
            TestName.Name = containingTestName;
            TestName.FullName = containingTestElementFullName;
            Position = position;
        }

        public override void RunTestMethod(TestResult testResult)
        {
            testResult.Skip("Specification not implemented.");
        }

        public TestPosition Position { get; private set; }

    }
}
