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
        private readonly NJasmineTestSuite _test;
        readonly AllSuitesBuildContext _buildContext;
        private readonly GlobalSetupManager _globalSetup;
        List<Test> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, AllSuitesBuildContext buildContext, GlobalSetupManager globalSetup)
        {
            _test = test;
            _buildContext = buildContext;
            _globalSetup = globalSetup;
            _accumulatedDescendants = new List<Test>();
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<Test> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyCategoryAndIgnoreIfSet(Test test)
        {
            if (_ignoreReason != null)
            {
                test.RunState = RunState.Explicit;
                test.IgnoreReason = _ignoreReason;
            }

            foreach (var category in _accumulatedCategories)
            {
                NUnitFrameworkUtil.ApplyCategoryToTest(category, test);
            }
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var subSuiteAsFailedTest = new NJasmineUnimplementedTestMethod(position);

                _buildContext._nameGenator.NameTest(description, _test, subSuiteAsFailedTest);

                ApplyCategoryAndIgnoreIfSet(subSuiteAsFailedTest);

                _accumulatedDescendants.Add(subSuiteAsFailedTest);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(position, _globalSetup);

                bool reusedName;

                _buildContext._nameGenator.NameFork(description, _test, subSuite, out reusedName);

                ApplyCategoryAndIgnoreIfSet(subSuite);

                var actualSuite = subSuite.BuildNJasmineTestSuite(_buildContext, _globalSetup, action, false);

                if (!actualSuite.IsSuite && reusedName)
                {
                    _buildContext._nameGenator.MakeNameUnique((INJasmineTest)actualSuite);
                }

                _accumulatedDescendants.Add(actualSuite);
            }
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
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

                _buildContext._nameGenator.NameTest(description, _test, unimplementedTest);

                ApplyCategoryAndIgnoreIfSet(unimplementedTest);

                _accumulatedDescendants.Add(unimplementedTest);
            }
            else
            {
                var test = new NJasmineTestMethod(_buildContext._fixtureFactory, position, _globalSetup);

                _buildContext._nameGenator.NameTest(description, _test, test);

                ApplyCategoryAndIgnoreIfSet(test);

                _accumulatedDescendants.Add(test);
            }
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
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

        public void visitExpect(SpecElement origin, Expression<Func<bool>> expectation, TestPosition position)
        {
        }

        public void visitWaitUntil(SpecElement origin, Expression<Func<bool>> expectation, int totalWaitMs, int waitIncrementMs, TestPosition position)
        {
        }

        public void visitWithCategory(SpecElement origin, string category, TestPosition position)
        {
            _accumulatedCategories.Add(category);
        }
    }
}