using System;
using NJasmine.Core;
using NUnit.Core;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestSuiteNUnit : global::NUnit.Core.TestSuite, INJasmineTest
    {
        public TestPosition Position { get; private set; }
        private readonly Action _oneTimeTeardown;

        public NJasmineTestSuiteNUnit(string parentSuiteName, string name, Action oneTimeTeardown, TestPosition position) : base(parentSuiteName, name)
        {
            _oneTimeTeardown = oneTimeTeardown;
            maintainTestOrder = true;
            Position = position;
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            try
            {
                _oneTimeTeardown();
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