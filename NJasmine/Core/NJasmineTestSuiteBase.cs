using System;
using System.Collections.Generic;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    internal class NJasmineTestSuiteBase : TestSuite, ISpecPositionVisitor
    {
        Func<ISpecificationRunner> _fixtureFactory;
        protected ISpecificationRunner _fixtureInstanceForDiscovery;
        protected PerFixtureSetupContext _nunitImports;
        protected List<Test> _accumulatedDescendants;
        protected NameGenerator _nameGenator;

        public NJasmineTestSuiteBase(Func<ISpecificationRunner> fixtureFactory, PerFixtureSetupContext parent, NameGenerator nameGenerator, ISpecificationRunner fixtureInstanceForDiscovery)
            : base("thistestname", "willbeoverwritten")
        {
            _fixtureFactory = fixtureFactory;
            _nameGenator = nameGenerator;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
            _nunitImports = new PerFixtureSetupContext(parent);
            _accumulatedDescendants = new List<Test>();
        }

        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                _nameGenator.NameTest(this, description, nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                string baseName = TestName.FullName;

                var describeSuite = new NJasmineTestSuite(_fixtureFactory, position, _nunitImports, _nameGenator, _fixtureInstanceForDiscovery);

                _nameGenator.NameTest(this, description, describeSuite);

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

                _nameGenator.NameTest(this, description, nJasmineUnimplementedTestMethod);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var testMethod = new NJasmineTestMethod(_fixtureFactory, position, _nunitImports);

                _nameGenator.NameTest(this, description, testMethod);

                _accumulatedDescendants.Add(testMethod);
            }
        }

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }
    }
}