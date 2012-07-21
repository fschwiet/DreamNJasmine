using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.NUnit.TestElements;

namespace NJasmine.NUnit
{
    class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        readonly INativeTestFactory _nativeTestFactory;
        private readonly TestBuilder _parent;
        readonly FixtureDiscoveryContext _buildContext;
        private readonly GlobalSetupManager _globalSetup;
        List<TestBuilder> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(INativeTestFactory nativeTestFactory, TestBuilder parent, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup)
        {
            _nativeTestFactory = nativeTestFactory;
            _parent = parent;
            _buildContext = buildContext;
            _globalSetup = globalSetup;
            _accumulatedDescendants = new List<TestBuilder>();
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<TestBuilder> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyCategoryAndIgnoreIfSet(TestBuilder result)
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

        public void visitFork(ForkElement element, TestPosition position)
        {
            if (element.Action == null)
            {
                var name = _buildContext.NameReservations.GetReservedTestName(element.Description, _parent.Name);

                var result = new TestBuilder(_nativeTestFactory.ForUnimplementedTest(name, position));

                ApplyCategoryAndIgnoreIfSet(result);

                _accumulatedDescendants.Add(result);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(position, _globalSetup, _buildContext);

                var forkName =_buildContext.NameReservations.GetSharedTestName(element.Description, _parent.Name);

                var resultBuilder = new TestBuilder(_nativeTestFactory.ForSuite(forkName , position, () => _globalSetup.Cleanup(position)), forkName );

                ApplyCategoryAndIgnoreIfSet(resultBuilder);

                var finalResultBuilder = subSuite.RunSuiteAction(element.Action, false, resultBuilder);

                _accumulatedDescendants.Add(finalResultBuilder);
            }
        }

        public TArranged visitBeforeAll<TArranged>(BeforeAllElement<TArranged> element, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterAll(AfterAllElement element, TestPosition position)
        {
        }

        public TArranged visitBeforeEach<TArranged>(BeforeEachElement<TArranged> element, TestPosition position)
        {
            return default(TArranged);
        }

        public void visitAfterEach(SpecificationElement element, Action action, TestPosition position)
        {
        }

        public void visitTest(TestElement element, TestPosition position)
        {
            if (element.Action == null)
            {
                var reservedName = _buildContext.NameReservations.GetReservedTestName(element.Description, _parent.Name);

                var buildResult = new TestBuilder(_nativeTestFactory.ForUnimplementedTest(reservedName, position), reservedName);

                ApplyCategoryAndIgnoreIfSet(buildResult);
                
                _accumulatedDescendants.Add(buildResult);
            }
            else
            {
                var buildResult = _buildContext.CreateTest(this._globalSetup, _parent, position, element.Description);

                ApplyCategoryAndIgnoreIfSet(buildResult);

                _accumulatedDescendants.Add(buildResult);
            }
        }

        public void visitIgnoreBecause(IgnoreElement element, TestPosition position)
        {
            if (_accumulatedDescendants.Count > 0)
            {
                _ignoreReason = element.Reason;
            }
            else
            {
                _parent.AddIgnoreReason(element.Reason);
            }
        }

        public void visitExpect(ExpectElement element, TestPosition position)
        {
        }

        public void visitWaitUntil(WaitUntilElement element, TestPosition position)
        {
        }

        public void visitWithCategory(WithCategoryElement element, TestPosition position)
        {
            _accumulatedCategories.Add(element.Category);
        }

        public void visitTrace(TraceElement element, TestPosition position)
        {
        }

        public void visitLeakDisposable(LeakDisposableElement element, TestPosition position)
        {
        }
    }
}