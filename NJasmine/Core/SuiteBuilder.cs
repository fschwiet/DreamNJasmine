using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    internal class SuiteBuilder : ISpecPositionVisitor
    {
        private readonly NJasmineTestSuite _test;
        readonly AllSuitesBuildContext _buildContext;
        PerFixtureSetupContext _nunitImports;
        List<Test> _accumulatedDescendants;

        public SuiteBuilder(NJasmineTestSuite test, AllSuitesBuildContext buildContext, PerFixtureSetupContext parent)
        {
            _test = test;
            _buildContext = buildContext;
            _nunitImports = new PerFixtureSetupContext(parent);
            _accumulatedDescendants = new List<Test>();
        }

        public RunsActionOnDispose VisitSuiteFromPosition(NJasmineTestSuite nJasmineTestSuite, TestPosition position)
        {
            return _buildContext._fixtureInstanceForDiscovery.UseVisitor(new VisitorPositionAdapter(position, this));
        }

        public void AddAccumulatedDescendents()
        {
            foreach (var sibling in _accumulatedDescendants)
                _test.Add(sibling);
        }

        public void DoFixtureCleanup()
        {
            _nunitImports.DoAllCleanup();
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                _buildContext._nameGenator.NameTest(_test, description, nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                string baseName = _test.TestName.FullName;

                var describeSuite = new NJasmineTestSuite(_buildContext,position, _nunitImports);

                _buildContext._nameGenator.NameTest(_test, description, describeSuite);

                var actualSuite = describeSuite.BuildNJasmineTestSuite(action, false);

                _accumulatedDescendants.Add(actualSuite);
            }
        }

        public TArranged visitBeforeAll<TArranged>(SpecElement origin, Func<TArranged> action, TestPosition position)
        {
            _nunitImports.AddFixtureSetup(position, action);
            return default(TArranged);
        }

        public void visitAfterAll(SpecElement origin, Action action, TestPosition position)
        {
            _nunitImports.AddFixtureTearDown(position, action);
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

                _buildContext._nameGenator.NameTest(_test, description, nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var testMethod = new NJasmineTestMethod(_buildContext._fixtureFactory, position, _nunitImports);

                _buildContext._nameGenator.NameTest(_test, description, testMethod);

                _accumulatedDescendants.Add(testMethod);
            }
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }
    }
}