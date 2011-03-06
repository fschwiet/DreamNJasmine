using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : NJasmineTestSuiteBase, INJasmineTest
    {
        readonly TestPosition _position;

        public static Test CreateRootNJasmineSuite(Func<ISpecificationRunner> fixtureFactory, Type type)
        {
            NJasmineTestSuite rootSuite = new NJasmineTestSuite(fixtureFactory, new TestPosition(), new PerFixtureSetupContext(), new NameGenerator(), fixtureFactory());
            rootSuite.TestName.FullName = type.Namespace + "." + type.Name;
            rootSuite.TestName.Name = type.Name;

            return rootSuite.BuildNJasmineTestSuite(rootSuite._fixtureInstanceForDiscovery.Run, true);
        }

        public NJasmineTestSuite(Func<ISpecificationRunner> fixtureFactory, TestPosition position, PerFixtureSetupContext parent, NameGenerator nameGenerator, ISpecificationRunner fixtureInstanceForDiscovery)
            : base(fixtureFactory, parent, nameGenerator, fixtureInstanceForDiscovery)
        {
            _position = position;

            maintainTestOrder = true;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public Test BuildNJasmineTestSuite(Action action, bool isOuterScopeOfSpecification)
        {
            Exception exception = null;

            var visitorPositionAdapter = new VisitorPositionAdapter(_position.GetFirstChildPosition(), this);

            using (_fixtureInstanceForDiscovery.UseVisitor(visitorPositionAdapter))
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
                        _nameGenator.MakeNameUnique(nJasmineInvalidTestSuite);
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
