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
    public class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        private readonly NJasmineTestSuite _test;
        readonly FixtureDiscoveryContext _fixtureContext;
        readonly BranchDestiny _branchDestiny;
        readonly Action<Test> _testVisitor;
        bool _haveVisitedTest;
        List<string> _accumulatedCategories;
        string _ignoreReason;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, FixtureDiscoveryContext fixtureContext, BranchDestiny branchDestiny, Action<Test> testVisitor)
        {
            _test = test;
            _fixtureContext = fixtureContext;
            _branchDestiny = branchDestiny;

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

                _fixtureContext.NameGenator.NameTest(description, _test, subSuiteAsFailedTest);

                ApplyCategoryAndIgnoreIfSet(subSuiteAsFailedTest);

                _testVisitor(subSuiteAsFailedTest);
            }
            else
            {
                var subSuite = new NJasmineTestSuite(_fixtureContext);

                bool reusedName;

                _fixtureContext.NameGenator.NameFork(description, _test, subSuite, out reusedName);

                ApplyCategoryAndIgnoreIfSet(subSuite);

                var actualSuite = subSuite.BuildNJasmineTestSuite(action, false, position);

                if (!actualSuite.IsSuite && reusedName)
                {
                    _fixtureContext.NameGenator.MakeNameUnique((INJasmineTest)actualSuite);
                }

                _testVisitor(actualSuite);
            }
        }

        public void visitEither(SpecElement origin, Action<Action>[] options, TestPosition position, Action<TestPosition> updatePositionHandler)
        {
            int? destiny = _branchDestiny.GetDestinedPath(position);

            if (!destiny.HasValue)
            {
                _branchDestiny.AddRemainingOptions(position, options);
                destiny = 0;
            }

            updatePositionHandler(position.GetChildPositionByIndex(destiny.Value));
            InlineBranching.RunBranchOption(options[destiny.Value]);
            updatePositionHandler(position.GetChildPositionByIndex(destiny.Value).GetFirstChildPosition());
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

                _fixtureContext.NameGenator.NameTest(description, _test, unimplementedTest);

                ApplyCategoryAndIgnoreIfSet(unimplementedTest);

                _testVisitor(unimplementedTest);
            }
            else
            {
                var test = new NJasmineTestMethod(_fixtureContext.FixtureFactory, position, _fixtureContext.GlobalSetup);

                _fixtureContext.NameGenator.NameTest(description, _test, test);

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