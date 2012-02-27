using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineUnimplementedTestMethod : NJasmineNUnitTestMethod, INJasmineTest
    {
        public NJasmineUnimplementedTestMethod(TestPosition position) 
            : base(((Action)delegate() { }).Method, position)
        {
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            listener.TestStarted(base.TestName);
            long ticks = DateTime.Now.Ticks;
            TestResult result = new TestResult(this);
            result.SetResult(ResultState.NotRunnable, "Specification not implemented.", null, FailureSite.Test);                
            double num3 = ((double)(DateTime.Now.Ticks - ticks)) / 10000000.0;
            result.Time = num3;
            listener.TestFinished(result);
            return result;
        }
    }
}
