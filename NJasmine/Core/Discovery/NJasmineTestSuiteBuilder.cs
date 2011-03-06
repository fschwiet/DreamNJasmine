using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Extras;
using NUnit.Core;

namespace NJasmine.Core.Discovery
{
    class NJasmineTestSuiteBuilder : ISpecPositionVisitor
    {
        readonly AllSuitesBuildContext _buildContext;
        PerFixtureSetupContext _fixtureSetupContext;
        List<Test> _accumulatedDescendants;
        private string _fullName;

        public NJasmineTestSuiteBuilder(NJasmineTestSuite test, AllSuitesBuildContext buildContext, PerFixtureSetupContext fixtureSetupContext)
        {
            _fullName = test.TestName.FullName;
            _buildContext = buildContext;
            _fixtureSetupContext = fixtureSetupContext;
            _accumulatedDescendants = new List<Test>();
        }

        public RunsActionOnDispose VisitSuiteFromPosition(TestPosition position)
        {
            return _buildContext._fixtureInstanceForDiscovery.UseVisitor(new VisitorPositionAdapter(position, this));
        }

        public void VisitAccumulatedTests(Action<Test> action)
        {
            foreach (var descendant in _accumulatedDescendants)
                action(descendant);
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                _buildContext._nameGenator.NameTest(_fullName, description, nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var describeSuite = new NJasmineTestSuite(position);

                _buildContext._nameGenator.NameTest(_fullName, description, describeSuite);

                var actualSuite = describeSuite.BuildNJasmineTestSuite(_buildContext, _fixtureSetupContext, action, false);

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
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                _buildContext._nameGenator.NameTest(_fullName, description, nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var testMethod = new NJasmineTestMethod(_buildContext._fixtureFactory, position, _fixtureSetupContext);

                _buildContext._nameGenator.NameTest(_fullName, description, testMethod);

                _accumulatedDescendants.Add(testMethod);
            }
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }
    }
}