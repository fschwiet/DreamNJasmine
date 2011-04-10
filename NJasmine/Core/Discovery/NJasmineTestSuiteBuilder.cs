using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;
using NUnit.Core;

namespace NJasmine.Core.Discovery
{
    class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        string _fullName;
        private readonly NJasmineTestSuite _test;
        readonly AllSuitesBuildContext _buildContext;
        PerFixtureSetupContext _fixtureSetupContext;
        List<Test> _accumulatedDescendants;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, AllSuitesBuildContext buildContext, PerFixtureSetupContext fixtureSetupContext)
        {
            _fullName = test.TestName.FullName;
            _test = test;
            _buildContext = buildContext;
            _fixtureSetupContext = fixtureSetupContext;
            _accumulatedDescendants = new List<Test>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<Test> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyIgnoreIfSet(Test test)
        {
            if (_ignoreReason != null)
            {
                test.RunState = RunState.Explicit;
                test.IgnoreReason = _ignoreReason;
            }
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var subSuiteAsFailedTest = new NJasmineUnimplementedTestMethod(position);

                _buildContext._nameGenator.NameTest(_fullName, description, subSuiteAsFailedTest);

                ApplyIgnoreIfSet(subSuiteAsFailedTest);

                _accumulatedDescendants.Add(subSuiteAsFailedTest);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(position);

                _buildContext._nameGenator.NameTest(_fullName, description, subSuite);

                ApplyIgnoreIfSet(subSuite);

                var actualSuite = subSuite.BuildNJasmineTestSuite(_buildContext, _fixtureSetupContext, action, false);

                _accumulatedDescendants.Add(actualSuite);
            }
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            _fixtureSetupContext.AddFixtureSetup(position, action);
            return default(TArranged);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _fixtureSetupContext.AddFixtureTearDown(position, action);
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, Func<TArranged> factory, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var unimplementedTest = new NJasmineUnimplementedTestMethod(position);

                _buildContext._nameGenator.NameTest(_fullName, description, unimplementedTest);

                ApplyIgnoreIfSet(unimplementedTest);

                _accumulatedDescendants.Add(unimplementedTest);
            }
            else
            {
                var test = new NJasmineTestMethod(_buildContext._fixtureFactory, position, _fixtureSetupContext);

                _buildContext._nameGenator.NameTest(_fullName, description, test);

                ApplyIgnoreIfSet(test);

                _accumulatedDescendants.Add(test);
            }
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            if (_accumulatedDescendants.Count > 0)
            {
                _ignoreReason = reason;
            }
            else if (string.IsNullOrEmpty(this._test.IgnoreReason))
            {
                this._test.RunState = RunState.Explicit;
                this._test.IgnoreReason = reason;
            }
        }

        public void visitExpect(Expression<Func<bool>> expectation, TestPosition position)
        {
        }

        public void visitWaitUntil(Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
        }
    }
}