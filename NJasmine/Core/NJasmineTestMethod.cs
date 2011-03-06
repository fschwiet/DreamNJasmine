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
    public partial class NJasmineTestMethod : TestMethod, INJasmineTest, ISpecPositionVisitor
    {
        readonly Func<ISpecificationRunner> _fixtureFactory;
        readonly TestPosition _position;
        readonly PerFixtureSetupContext _fixtureSetupTeardown;

        NJasmineExecutionContext _executionContext;

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
            _executionContext = new NJasmineExecutionContext(this, _fixtureSetupTeardown);
            
            var fixture = this._fixtureFactory();

            fixture.UseVisitor(new VisitorPositionAdapter(this));

            try
            {
                fixture.Run();
            }

            catch (TestFinishedException)
            {
            }
            finally
            {
                _executionContext.RunAllPerTestTeardowns();
            }
            testResult.Success();
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            _executionContext.State.visitFork(origin, description, action, position);
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _executionContext.State.visitBeforeAll(origin, action, position);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _executionContext.State.visitAfterAll(origin, action, position);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
        {
            return _executionContext.State.visitBeforeEach<TArranged>(origin, description, factory, position);
        }
        
        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _executionContext.State.visitAfterEach(origin, action, position);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            _executionContext.State.visitTest(origin, description, action, position);
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
