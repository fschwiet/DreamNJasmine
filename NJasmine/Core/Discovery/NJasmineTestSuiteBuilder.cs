using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core.Discovery
{
    class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        private readonly NJasmineTestSuite _test;
        private readonly NJasmineBuildResult _parent;
        readonly FixtureDiscoveryContext _buildContext;
        private readonly GlobalSetupManager _globalSetup;
        List<NJasmineBuildResult> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, NJasmineBuildResult parent, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup)
        {
            _test = test;
            _parent = parent;
            _buildContext = buildContext;
            _globalSetup = globalSetup;
            _accumulatedDescendants = new List<NJasmineBuildResult>();
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<NJasmineBuildResult> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyCategoryAndIgnoreIfSet(NJasmineBuildResult result)
        {
            if (_ignoreReason != null)
            {
                result.AddIgnoreReason(_ignoreReason);
            }

            foreach (var category in _accumulatedCategories)
            {
                result.AddCategory(category);
            }
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var subSuiteAsFailedTest = new NJasmineUnimplementedTestMethod(position);

                _buildContext.NameGenator.NameTest(description, _parent, subSuiteAsFailedTest);

                var result = new NJasmineBuildResult(subSuiteAsFailedTest);

                ApplyCategoryAndIgnoreIfSet(result);

                _accumulatedDescendants.Add(result);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(position, _globalSetup);
                
                var _result = new NJasmineTestSuiteNUnit(_parent.FullName, description, p => _globalSetup.Cleanup(p), position);

                var resultBuilder = new NJasmineBuildResult(_result);

                ApplyCategoryAndIgnoreIfSet(resultBuilder);

                bool reusedName;

                _buildContext.NameGenator.NameFork(description, _parent, resultBuilder, out reusedName);

                subSuite.RunSuiteAction(_buildContext, _globalSetup, action, false, resultBuilder);

                if (reusedName)
                {
                    if (!resultBuilder.IsSuite())
                        _buildContext.NameGenator.MakeNameUnique(resultBuilder);
                }

                _accumulatedDescendants.Add(resultBuilder);
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

                _buildContext.NameGenator.NameTest(description, _parent, unimplementedTest);

                var buildResult = new NJasmineBuildResult(unimplementedTest);

                ApplyCategoryAndIgnoreIfSet(buildResult);
                
                _accumulatedDescendants.Add(buildResult);
            }
            else
            {
                var test = this._buildContext.CreateTest(this._globalSetup, _parent, position, description);

                var buildResult = new NJasmineBuildResult(test);

                ApplyCategoryAndIgnoreIfSet(buildResult);

                _accumulatedDescendants.Add(buildResult);
            }
        }

        public void visitIgnoreBecause(SpecElement origin, string reason, TestPosition position)
        {
            if (_accumulatedDescendants.Count > 0)
            {
                _ignoreReason = reason;
            }
            else
            {
                _parent.SetIgnoreReason(reason);
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