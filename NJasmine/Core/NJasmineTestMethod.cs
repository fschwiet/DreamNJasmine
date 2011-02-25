using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    public partial class NJasmineTestMethod : TestMethod, INJasmineTest
    {
        readonly Func<ISpecificationRunner> _fixtureFactory;
        readonly TestPosition _position;
        readonly PerFixtureSetupContext _fixtureSetupTeardown;

        List<Action> _allTeardowns = null;
        ISpecPositionVisitor _state = null;

        public NJasmineTestMethod(Func<ISpecificationRunner> fixtureFactory, TestPosition position, PerFixtureSetupContext fixtureSetupTeardown)
            : base(new Action(delegate() { }).Method)
        {
            _fixtureFactory = fixtureFactory;
            _position = position;
            _fixtureSetupTeardown = fixtureSetupTeardown;
            _state = new DescribeState(this);
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
            this._allTeardowns = new List<Action>();

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
                this._allTeardowns.Reverse();

                foreach (var action in this._allTeardowns)
                {
                    action();
                }
            }
            testResult.Success();
        }

        public void whileInState(ISpecPositionVisitor state, Action action)
        {
            var originalState = _state;
            _state = state;
            try
            {
                action();
            }
            finally
            {
                _state = originalState;
            }
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            _state.visitFork(origin, description, action, position);
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return _state.visitBeforeAll(origin,action, position);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _state.visitAfterAll(origin, action, position);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
        {
            return _state.visitBeforeEach<TArranged>(origin, description, factory, position);
        }
        
        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _state.visitAfterEach(origin, action, position);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            _state.visitTest(origin, description, action, position);
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
