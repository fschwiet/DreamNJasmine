using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.Execution;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        readonly Func<ISpecificationRunner> _fixtureFactory;
        readonly TestPosition _position;
        readonly PerFixtureSetupContext _fixtureSetupTeardown;

        public NJasmineTestMethod(Func<ISpecificationRunner> fixtureFactory, TestPosition position, PerFixtureSetupContext fixtureSetupTeardown)
            : base(new Action(delegate() { }).Method)
        {
            _fixtureFactory = fixtureFactory;
            _position = position;
            _fixtureSetupTeardown = fixtureSetupTeardown;
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
                _fixtureSetupTeardown.DoCleanupFor(Position);
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
                testResult.Error(e);
            }

            double num3 = ((double)(DateTime.Now.Ticks - ticks)) / 10000000.0;
            testResult.Time = num3;
            listener.TestFinished(testResult);
            return testResult;
        }

        public void RunTestMethod(TestResult testResult)
        {
            var executionContext = new NJasmineExecutionContext(this, _fixtureSetupTeardown);
            var runner = new NJasmineTestRunner(executionContext);
            
            var fixture = this._fixtureFactory();

            fixture.UseVisitor(new VisitorPositionAdapter(runner));

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
