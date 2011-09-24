using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.Extras;
using NUnit.Core;

namespace NJasmine.Core.Discovery
{
    class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        private readonly NJasmineTestSuite _test;
        readonly AllSuitesBuildContext _buildContext;
        private readonly GlobalSetupManager _globalSetup;
        readonly Action<Test> _testVisitor;
        bool _haveVisitedTest;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, AllSuitesBuildContext buildContext, GlobalSetupManager globalSetup, Action<Test> testVisitor)
        {
            _test = test;
            _buildContext = buildContext;
            _globalSetup = globalSetup;

            _testVisitor = t =>
            {
                _haveVisitedTest = true;
                testVisitor(t);
            };

            _haveVisitedTest = false;
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
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

                _testVisitor(subSuiteAsFailedTest);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(_globalSetup);

                bool reusedName;

                _buildContext._nameGenator.NameFork(description, _test, subSuite, out reusedName);

                ApplyCategoryAndIgnoreIfSet(subSuite);

                var actualSuite = subSuite.BuildNJasmineTestSuite(_buildContext, _globalSetup, action, false, position);

                if (!actualSuite.IsSuite && reusedName)
                {
                    _buildContext._nameGenator.MakeNameUnique((INJasmineTest)actualSuite);
                }

                _testVisitor(actualSuite);
            }
        }

        public void visitEither(SpecElement origin, Action<Action>[] options, TestPosition position)
        {
            var destiny = _buildContext.GetDestinedPath(position);

            if (destiny.HasValue)
            {
                var option = options[destiny.Value];

                InlineBranching.RunBranchOption(option);
            }
            else
            {
                InlineBranching.HandleInlineBranches(position, options, (branch, branchPosition) =>
                {
                    _buildContext._pendingDiscoveryBranches.Enqueue(new PendingDiscoveryBranches()
                    {
                        ChosenPath = branchPosition
                    });
                });
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

                _testVisitor(unimplementedTest);
            }
            else
            {
                var test = new NJasmineTestMethod(_buildContext._fixtureFactory, position, _globalSetup);

                _buildContext._nameGenator.NameTest(description, _test, test);

                ApplyCategoryAndIgnoreIfSet(test);

                _testVisitor(test);
            }
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
            if (_haveVisitedTest)
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

        public void visitTrace(SpecElement origin, string message, TestPosition position)
        {
        }

        public void visitLeakDisposable(SpecElement origin, IDisposable disposable, TestPosition position)
        {
        }
    }
}