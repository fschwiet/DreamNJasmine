using System;
using System.Reflection;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineInvalidTestSuite : TestMethod
    {
        string _stackTrace;

        public NJasmineInvalidTestSuite(string locationOfFailure, Exception e) 
            : base(((Action)delegate() { }).Method)
        {
            TestName.Name = locationOfFailure + ": " + e.Message;
            _stackTrace = e.StackTrace;
        }

        public override void RunTestMethod(TestResult testResult)
        {
            testResult.Failure(TestName.Name, _stackTrace);
        }
    }
}