using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Marshalled;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestMethod : TestMethod, IPrefailable
    {
        public TestPosition Position { get
            {
                return _testContext.Position;
            } 
        }

        readonly TestContext _testContext;
        private Exception _pendingException;

        public NJasmineTestMethod(TestContext testContext)
            : base(new Action(delegate() { }).Method)
        {
            _testContext = testContext;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            return RunTest(listener);
        }

        TestResult RunTest(EventListener listener)
        {
            listener.TestStarted(base.TestName);

            TestResult nunitTestResult = new TestResult(this);

            if (_pendingException != null)
            {
                nunitTestResult.Failure(_pendingException.Message, _pendingException.StackTrace, FailureSite.Test);
            }
            else if (RunState == RunState.NotRunnable)
            {
                nunitTestResult.SetResult(ResultState.NotRunnable, IgnoreReason, "", FailureSite.Test);
            }
            else
            {
                var testResult = SpecificationRunner.RunTest(this._testContext, new List<string>());

                NativeTestResult.ApplyToNunitResult(testResult, nunitTestResult);
            }

            listener.TestFinished(nunitTestResult);

            return nunitTestResult;
        }

        public void SetPendingException(Exception e)
        {
            _pendingException = e;
        }
    }

    public interface IPrefailable
    {
        void SetPendingException(Exception e);
    }
}
