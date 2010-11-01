using System;
using System.Reflection;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineInvalidTestSuite : TestMethod, INJasmineTest
    {
        string _stackTrace;

        public NJasmineInvalidTestSuite(string containingTestElementFullName, string locationOfFailure, Exception e, TestPosition position) 
            : base(((Action)delegate() { }).Method)
        {
            TestName.Name = locationOfFailure + ": " + e.Message;
            TestName.FullName = containingTestElementFullName;
            _stackTrace = e.StackTrace;
            Position = position;
        }

        public override void RunTestMethod(TestResult testResult)
        {
            testResult.Failure(TestName.Name, _stackTrace);
        }

        public TestPosition Position { get; private set; }
    }
}