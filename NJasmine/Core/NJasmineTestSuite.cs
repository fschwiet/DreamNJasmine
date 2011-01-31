using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest, ISpecPositionVisitor
    {
        public Test BuildNJasmineTestSuite()
        {
            return BuildNJasmineTestSuite(_fixtureInstanceForDiscovery.Specify, true);
        }

        private Test BuildNJasmineTestSuite(Action action, bool isOuterScopeOfSpecification)
        {
            _baseNameForChildTests = TestName.FullName;

            Exception exception = null;

            using (_fixtureInstanceForDiscovery.UseVisitor(new VisitorPositionAdapter(_position.GetFirstChildPosition(), this)))
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    exception = e;
                }

                if (exception == null)
                {
                    foreach (var sibling in _accumulatedDescendants)
                        Add(sibling);
                }
                else
                {
                    var nJasmineInvalidTestSuite = new NJasmineInvalidTestSuite(exception, _position);

                    nJasmineInvalidTestSuite.TestName.FullName = this.TestName.FullName;
                    nJasmineInvalidTestSuite.TestName.Name = this.TestName.Name;

                    if (isOuterScopeOfSpecification)
                    {
                        MakeNameUnique(nJasmineInvalidTestSuite);
                        Add(nJasmineInvalidTestSuite);
                    }
                    else
                    {
                        return nJasmineInvalidTestSuite;
                    }
                }
            }

            return this;
        }

        readonly Func<SkeleFixture> _fixtureFactory;
        readonly SkeleFixture _fixtureInstanceForDiscovery;
        readonly TestPosition _position;
        readonly NUnitFixtureCollection _nunitImports;
        readonly List<Test> _accumulatedDescendants;
        readonly List<string> _globallyAccumulatedTestNames;

        string _baseNameForChildTests;
        SpecElement? _testTypeReached;

        public NJasmineTestSuite(Func<SkeleFixture> fixtureFactory, string baseName, string name, TestPosition position, NUnitFixtureCollection parentNUnitImports, List<string> globallyAccumulatedTestNames)
            : base(baseName, name)
        {
            _fixtureFactory = fixtureFactory;
            _fixtureInstanceForDiscovery = fixtureFactory();
            _position = position;
            _nunitImports = new NUnitFixtureCollection(parentNUnitImports);
            _globallyAccumulatedTestNames = globallyAccumulatedTestNames;

            _accumulatedDescendants = new List<Test>();
            _testTypeReached = null;

            maintainTestOrder = true;
        }

        public NJasmineTestSuite(Func<SkeleFixture> fixtureFactory, SkeleFixture fixtureInstanceForDiscovery, string baseName, string name, TestPosition position, NUnitFixtureCollection parentNUnitImports, List<string> globallyAccumulatedTestNames)
            : base(baseName, name)
        {
            _fixtureFactory = fixtureFactory;
            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
            _position = position;
            _nunitImports = new NUnitFixtureCollection(parentNUnitImports);
            _globallyAccumulatedTestNames = globallyAccumulatedTestNames;

            _accumulatedDescendants = new List<Test>();
            _testTypeReached = null;

            maintainTestOrder = true;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        private void MakeNameUnique(Test test)
        {
            var name = test.TestName.FullName;

            if (_globallyAccumulatedTestNames.Contains(name))
            {
                var nextIndex = 1;
                string suffix;
                string nextName;

                do
                {
                    suffix = "`" + ++nextIndex;
                    nextName = name + suffix;
                } while (_globallyAccumulatedTestNames.Contains(nextName));


                test.TestName.Name = test.TestName.Name + suffix;
                test.TestName.FullName = test.TestName.FullName + suffix;
            }

            _globallyAccumulatedTestNames.Add(test.TestName.FullName);
        }

        private void NameTest(Test test, string name)
        {
            test.TestName.FullName = _baseNameForChildTests + ", " + name;
            test.TestName.Name = name;

            MakeNameUnique(test);
        }
        
        public void visitFork(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                NameTest(nJasmineUnimplementedTestMethod, description);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                string baseName = TestName.FullName;

                var describeSuite = new NJasmineTestSuite(_fixtureFactory, _fixtureInstanceForDiscovery, baseName, description, position, _nunitImports, _globallyAccumulatedTestNames);

                NameTest(describeSuite, description);

                var actualSuite = describeSuite.BuildNJasmineTestSuite(action, false);

                _accumulatedDescendants.Add(actualSuite);
            }
        }

        public void visitAfterEach(SpecElement origin, Action action, TestPosition position)
        {
            if (_testTypeReached.HasValue)
                throw WrongMethodAfterItMethod(_testTypeReached.Value, origin);
        }

        public void visitTest(SpecElement origin, string description, Action action, TestPosition position)
        {
            if (action == null)
            {
                var nJasmineUnimplementedTestMethod = new NJasmineUnimplementedTestMethod(position);

                NameTest(nJasmineUnimplementedTestMethod, description);

                _accumulatedDescendants.Add(nJasmineUnimplementedTestMethod);
            }
            else
            {
                var testMethod = new NJasmineTestMethod(_fixtureFactory, position, _nunitImports);

                NameTest(testMethod, description);

                _accumulatedDescendants.Add(testMethod);
            }

            _testTypeReached = origin;
        }

        public TFixture visitImportNUnit<TFixture>(TestPosition position) where TFixture: class, new()
        {
            if (_testTypeReached.HasValue)
                throw WrongMethodAfterItMethod(_testTypeReached.Value, SpecElement.importNUnit);

            _nunitImports.AddFixture(position, typeof(TFixture));

            return null;
        }

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
        {
            if (_testTypeReached.HasValue)
                throw WrongMethodAfterItMethod(_testTypeReached.Value, origin);

            if (description != null)
                _baseNameForChildTests = _baseNameForChildTests + ", " + description;

            return default(TArranged);
        }

        protected override void DoOneTimeSetUp(TestResult suiteResult)
        {
            _nunitImports.DoOnetimeSetUp();
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            _nunitImports.DoOnetimeTearDown();
        }

        InvalidOperationException WrongMethodAfterItMethod(SpecElement testElement, SpecElement innerSpecElement)
        {
            return new InvalidOperationException("Called " + innerSpecElement + "() after test started, within a call to " + testElement + "().");
        }
    }
}
