using System;
using System.Reflection;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineInvalidTestSuite : TestMethod, INJasmineTest
    {
        readonly string _reason;
        string _stackTrace;

        public NJasmineInvalidTestSuite(Exception e, TestPosition position) 
            : base(((Action)delegate() { }).Method)
        {
            _reason = e.Message;
            _stackTrace = e.StackTrace;
            Position = position;
        }

        public override void RunTestMethod(TestResult testResult)
        {
            testResult.Failure(_reason, _stackTrace);
        }

        public TestPosition Position { get; private set; }
    }
}