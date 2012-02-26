using System;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineTestSuiteNUnit : TestSuite, INJasmineTest
    {
        public TestPosition Position { get; private set; }
        private readonly Action<TestPosition> _oneTimeTeardown;

        public NJasmineTestSuiteNUnit(string parentSuiteName, string name, Action<TestPosition> oneTimeTeardown, TestPosition position) : base(parentSuiteName, name)
        {
            _oneTimeTeardown = oneTimeTeardown;
            maintainTestOrder = true;
            Position = position;
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            try
            {
                _oneTimeTeardown(Position);
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
                shim.ApplyToNunitResult(suiteResult);
            }
        }

        public string Shortname
        {
            get { return this.TestName.Name; }
            set { this.TestName.Name = value; }
        }

        public string FullName
        {
            get { return this.TestName.FullName; }
            set { this.TestName.FullName = value; }
        }

        public string MultilineName
        {
            get { return TestExtensions.GetMultilineName(this); }
            set { TestExtensions.SetMultilineName(this, value); }
        }
    }
}