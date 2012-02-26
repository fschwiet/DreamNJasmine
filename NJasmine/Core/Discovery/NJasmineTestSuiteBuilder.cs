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
        private readonly INJasmineBuildResult _parent;
        readonly FixtureDiscoveryContext _buildContext;
        private readonly GlobalSetupManager _globalSetup;
        List<INJasmineBuildResult> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, INJasmineBuildResult parent, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup)
        {
            _test = test;
            _parent = parent;
            _buildContext = buildContext;
            _globalSetup = globalSetup;
            _accumulatedDescendants = new List<INJasmineBuildResult>();
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<INJasmineBuildResult> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyCategoryAndIgnoreIfSet(INJasmineBuildResult result)
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
                var result = new NJasmineDirectBuildResult(new NJasmineUnimplementedTestMethod(position), position);

                _buildContext.NameGenator.NameTest(description, _parent, result);

                ApplyCategoryAndIgnoreIfSet(result);

                _accumulatedDescendants.Add(result);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(position, _globalSetup);

                var resultBuilder = new NJasmineDirectBuildResult(new NJasmineTestSuiteNUnit(_parent.FullName, description, () => _globalSetup.Cleanup(position), position), position);

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

                var buildResult = new NJasmineDirectBuildResult(unimplementedTest, position);

                _buildContext.NameGenator.NameTest(description, _parent, buildResult);

                ApplyCategoryAndIgnoreIfSet(buildResult);
                
                _accumulatedDescendants.Add(buildResult);
            }
            else
            {
                var buildResult = new NJasmineDirectBuildResult(_buildContext.CreateTest(this._globalSetup, _parent, position, description), position);

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
                _parent.AddIgnoreReason(reason);
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