﻿using System;
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
                    var nJasmineInvalidTestSuite = new NJasmineInvalidTestSuite(this.TestName, exception, _position);

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

        readonly Func<ISpecificationRunner> _fixtureFactory;
        readonly ISpecificationRunner _fixtureInstanceForDiscovery;
        readonly TestPosition _position;
        readonly PerFixtureSetupContext _nunitImports;
        readonly List<Test> _accumulatedDescendants;
        readonly List<string> _globallyAccumulatedTestNames;

        string _baseNameForChildTests;
        SpecElement? _testTypeReached;

        public static Test CreateRootNJasmineSuite(Func<ISpecificationRunner> fixtureFactory, string baseName, string name, TestPosition position, PerFixtureSetupContext parent, List<string> globallyAccumulatedTestNames)
        {
            NJasmineTestSuite rootSuite = new NJasmineTestSuite(fixtureFactory, baseName, name, position, parent, globallyAccumulatedTestNames, fixtureFactory());
            
            return rootSuite.BuildNJasmineTestSuite(rootSuite._fixtureInstanceForDiscovery.Run, true);
        }

        public NJasmineTestSuite(Func<ISpecificationRunner> fixtureFactory, string baseName, string name, TestPosition position, PerFixtureSetupContext parent, List<string> globallyAccumulatedTestNames, ISpecificationRunner fixtureInstanceForDiscovery)
            : base(baseName, name)
        {
            _fixtureFactory = fixtureFactory;
            _position = position;
            _nunitImports = new PerFixtureSetupContext(parent);
            _globallyAccumulatedTestNames = globallyAccumulatedTestNames;
            _accumulatedDescendants = new List<Test>();
            _testTypeReached = null;

            maintainTestOrder = true;

            _fixtureInstanceForDiscovery = fixtureInstanceForDiscovery;
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

                var describeSuite = new NJasmineTestSuite(_fixtureFactory, baseName, description, position, _nunitImports, _globallyAccumulatedTestNames, _fixtureInstanceForDiscovery);

                NameTest(describeSuite, description);

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

        public TArranged visitBeforeEach<TArranged>(SpecElement origin, string description, Func<TArranged> factory, TestPosition position)
        {
            if (description != null)
                _baseNameForChildTests = _baseNameForChildTests + ", " + description;

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

        public void visitIgnoreBecause(string reason, TestPosition position)
        {
            throw new NotImplementedException();
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            try
            {
                _nunitImports.DoAllCleanup();
            }
            catch (Exception innerException)
            {
                NUnitException exception2 = innerException as NUnitException;
                if (exception2 != null)
                {
                    innerException = exception2.InnerException;
                }
                suiteResult.Failure(innerException.Message, innerException.StackTrace, FailureSite.TearDown);
            }
        }
    }
}
