using System;
using System.Collections.Generic;
using System.Linq;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Marshalled;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestMethod : TestMethod
    {
        public TestPosition Position { get
            {
                return _testContext.Position;
            } 
        }

        readonly Func<SpecificationFixture> _fixtureFactory;
        readonly TestContext _testContext;

        public NJasmineTestMethod(Func<SpecificationFixture> fixtureFactory, TestContext testContext)
            : base(new Action(delegate() { }).Method)
        {
            _fixtureFactory = fixtureFactory;
            _testContext = testContext;
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            return RunTest(listener);
        }

        TestResult RunTest(EventListener listener)
        {
            listener.TestStarted(base.TestName);
            
            var testResult = SpecificationRunner.RunTest(this._testContext, this._fixtureFactory, new List<string>());

            var nunitTestResult = new TestResult(this);

            NativeTestResult.ApplyToNunitResult(testResult, nunitTestResult);

            listener.TestFinished(nunitTestResult);

            return nunitTestResult;
        }
    }
}
