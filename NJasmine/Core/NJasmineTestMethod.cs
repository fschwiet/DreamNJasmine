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
        readonly Func<SkeleFixture> _fixtureFactory;
        readonly TestPosition _position;
        readonly NUnitFixtureCollection _nUnitImports;

        List<Action> _allTeardowns = null;
        ISpecPositionVisitor _state = null;

        public NJasmineTestMethod(Func<SkeleFixture> fixtureFactory, TestPosition position, NUnitFixtureCollection nUnitImports) : base(new Action(delegate() { }).Method)
        {
            _fixtureFactory = fixtureFactory;
            _position = position;
            _nUnitImports = nUnitImports;
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
                fixture.Specify();
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

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            _state.visitAfterEach(origin, action, position);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            _state.visitTest(origin, description, action, position);
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            return _state.visitImportNUnit<TFixture>(position);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
        {
            return _state.visitBeforeEach<TArranged>(origin, description, factory, position);
        }

        public class TestFinishedException : Exception
        {
        }
    }
}
