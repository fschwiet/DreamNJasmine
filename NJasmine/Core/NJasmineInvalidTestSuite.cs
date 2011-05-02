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

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            listener.TestStarted(base.TestName);
            long ticks = DateTime.Now.Ticks;
            TestResult testResult = new TestResult(this);
            testResult.Failure(_reason, _stackTrace);
            double num3 = ((double)(DateTime.Now.Ticks - ticks)) / 10000000.0;
            testResult.Time = num3;
            listener.TestFinished(testResult);
            return testResult;
        }

        public TestPosition Position { get; private set; }
    }
}