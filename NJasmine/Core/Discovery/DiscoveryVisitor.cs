using System;
using System.Collections.Generic;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core.Discovery
{
    class DiscoveryVisitor : ISpecPositionVisitor
    {
        private readonly TestBuilder _parent;
        readonly SharedContext _sharedContext;
        private readonly IGlobalSetupManager _globalSetup;
        List<TestBuilder> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public DiscoveryVisitor(TestBuilder parent, SharedContext sharedContext, IGlobalSetupManager globalSetup)
        {
            _parent = parent;
            _sharedContext = sharedContext;
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
                var testContext = new TestContext()
                {
                    Name = _sharedContext.NameReservations.GetReservedTestName(element.Description, _parent.Name),
                    Position = position,
                    GlobalSetupManager = _globalSetup
                };
                
                var result = new TestBuilder(_sharedContext.NativeTestFactory.ForTest(_sharedContext, testContext));
                result.GetUnderlyingTest().MarkTestInvalid("Specification is not implemented.");

                ApplyCategoryAndIgnoreIfSet(result);

                _accumulatedDescendants.Add(result);
            }
            else
            {
                var testContext = new TestContext()
                {
                    Name = _sharedContext.NameReservations.GetSharedTestName(element.Description, _parent.Name),
                    Position = position,
                    GlobalSetupManager = _globalSetup
                };

                var finalResultBuilder = TestBuilder.BuildSuiteForTextContext(_sharedContext, testContext, element.Action, false);

                ApplyCategoryAndIgnoreIfSet(finalResultBuilder);

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
                var testContext = new TestContext()
                {
                    Name = _sharedContext.NameReservations.GetReservedTestName(element.Description, _parent.Name),
                    Position = position,
                    GlobalSetupManager = _globalSetup
                };

                var buildResult = new TestBuilder(_sharedContext.NativeTestFactory.ForTest(_sharedContext, testContext), testContext.Name);
                buildResult.GetUnderlyingTest().MarkTestInvalid("Specification is not implemented.");

                ApplyCategoryAndIgnoreIfSet(buildResult);
                
                _accumulatedDescendants.Add(buildResult);
            }
            else
            {
                var buildResult = _sharedContext.CreateTest(this._globalSetup, _parent, position, element.Description);

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