using System;
using System.Collections.Generic;
using NJasmine.Core.Elements;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NJasmine.Core.NativeWrappers;

namespace NJasmine.Core.Discovery
{
    class DiscoveryVisitor : ISpecPositionVisitor
    {
        private readonly INativeTest _parent;
        readonly FixtureContext _fixtureContext;
        private readonly IGlobalSetupManager _globalSetup;
        List<INativeTest> _accumulatedDescendants;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public DiscoveryVisitor(INativeTest parent, FixtureContext fixtureContext, IGlobalSetupManager globalSetup)
        {
            _parent = parent;
            _fixtureContext = fixtureContext;
            _globalSetup = globalSetup;
            _accumulatedDescendants = new List<INativeTest>();
            _accumulatedCategories = new List<string>();
            _ignoreReason = null;
        }

        public void VisitAccumulatedTests(Action<INativeTest> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        private void ApplyCategoryAndIgnoreIfSet(INativeTest result)
        {
            if (_ignoreReason != null)
            {
                result.MarkTestIgnored(_ignoreReason);
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
                    Name = _fixtureContext.NameReservations.GetReservedTestName(element.Description, _parent.Name),
                    Position = position,
                    GlobalSetupManager = _globalSetup
                };
                
                var result = _fixtureContext.NativeTestFactory.ForTest(_fixtureContext, testContext);
                result.MarkTestInvalid("Specification is not implemented.");

                ApplyCategoryAndIgnoreIfSet(result);

                _accumulatedDescendants.Add(result);
            }
            else
            {
                var testContext = new TestContext()
                {
                    Name = _fixtureContext.NameReservations.GetSharedTestName(element.Description, _parent.Name),
                    Position = position,
                    GlobalSetupManager = _globalSetup
                };

                var suiteResuilt = SpecificationBuilder.BuildSuiteForTextContext(_fixtureContext, testContext, element.Action, false);

                ApplyCategoryAndIgnoreIfSet(suiteResuilt);

                _accumulatedDescendants.Add(suiteResuilt);
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
                    Name = _fixtureContext.NameReservations.GetReservedTestName(element.Description, _parent.Name),
                    Position = position,
                    GlobalSetupManager = _globalSetup
                };

                var test = _fixtureContext.NativeTestFactory.ForTest(_fixtureContext, testContext);
                test.MarkTestInvalid("Specification is not implemented.");

                ApplyCategoryAndIgnoreIfSet(test);
                
                _accumulatedDescendants.Add(test);
            }
            else
            {
                var buildResult = _fixtureContext.CreateTest(this._globalSetup, _parent, position, element.Description);

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
                _parent.MarkTestIgnored(element.Reason);
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