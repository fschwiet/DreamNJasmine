using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.Discovery;
using NJasmine.Core.Execution;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        readonly TestPosition _position;
        readonly Func<SpecificationFixture> _fixtureFactory;
        readonly PerFixtureSetupContext _fixtureSetupContext;

        public NJasmineTestMethod(Func<SpecificationFixture> fixtureFactory, TestPosition position, PerFixtureSetupContext fixtureSetupContext)
            : base(new Action(delegate() { }).Method)
        {
            _position = position;
            _fixtureFactory = fixtureFactory;
            _fixtureSetupContext = fixtureSetupContext;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public override TestResult Run(EventListener listener, ITestFilter filter)
        {
            listener.TestStarted(base.TestName);
            long ticks = DateTime.Now.Ticks;
            TestResult testResult = new TestResult(this);

            try
            {
                _fixtureSetupContext.DoCleanupFor(Position);
            }
            catch (Exception e)
            {
                testResult.Error(new Exception(
                        "Exception thrown during cleanup of previous test, see inner exception for details", 
                        e));
            }

            try
            {
                if (!testResult.HasResults)
                    RunTestMethod(testResult);
            }
            catch (Exception e)
            {
                TestResultUtil.Error(testResult, e);
            }

            double num3 = ((double)(DateTime.Now.Ticks - ticks)) / 10000000.0;
            testResult.Time = num3;
            listener.TestFinished(testResult);
            return testResult;
        }

        public void RunTestMethod(TestResult testResult)
        {
            var executionContext = new NJasmineTestRunContext(Position, _fixtureSetupContext);
            var runner = new NJasmineTestRunner(executionContext);

            SpecificationFixture fixture = this._fixtureFactory();

            fixture.CurrentPosition = new TestPosition(0);
            fixture.Visitor = runner;
            try
            {
                fixture.Run();
            }

            catch (TestFinishedException)
            {
            }
            finally
            {
                executionContext.RunAllPerTestTeardowns();
            }
            testResult.Success();
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
