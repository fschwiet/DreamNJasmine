using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestSuiteNUnit : global::NUnit.Core.TestSuite
    {
        private readonly TestContext _testContext;

        public NJasmineTestSuiteNUnit(TestContext testContext) : base("hi", "there")
        {
            _testContext = testContext;
            maintainTestOrder = true;
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            try
            {
                _testContext.FixtureContext.GlobalSetupManager.Cleanup(_testContext.Position);
            }
            catch (Exception innerException)
            {
                NUnitException exception2 = innerException as NUnitException;
                if (exception2 != null)
                {
                    innerException = exception2.InnerException;
                }

                var shim = new TestResultShim();
                TestResultUtil.Error(shim, suiteResult.Test.GetMultilineName(), innerException, null, TestResultShim.Site.TearDown);
                NativeTestResult.ApplyToNunitResult(shim, suiteResult);
            }
        }
    }
}