using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest
    {
        readonly SuiteBuilder _builder;
        readonly TestPosition _position;

        public static Test CreateRootNJasmineSuite(Func<ISpecificationRunner> fixtureFactory, Type type)
        {
            AllSuitesBuildContext buildContext = new AllSuitesBuildContext(fixtureFactory, new NameGenerator(), fixtureFactory());

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(buildContext, new TestPosition(), new PerFixtureSetupContext());
            rootSuite.TestName.FullName = type.Namespace + "." + type.Name;
            rootSuite.TestName.Name = type.Name;

            return rootSuite.BuildNJasmineTestSuite(buildContext._fixtureInstanceForDiscovery.Run, true);
        }

        public NJasmineTestSuite(AllSuitesBuildContext buildContext, TestPosition position, PerFixtureSetupContext parent)
            : base("thistestname", "willbeoverwritten")
        {
            _position = position;
            _builder = new SuiteBuilder(this, buildContext, parent);

            maintainTestOrder = true;
        }

        public TestPosition Position
        {
            get { return _position; }
        }

        public Test BuildNJasmineTestSuite(Action action, bool isOuterScopeOfSpecification)
        {
            Exception exception = null;

            using (_builder.VisitSuiteFromPosition(this, _position.GetFirstChildPosition()))
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
                    _builder.AddAccumulatedDescendents();
                }
                else
                {
                    var nJasmineInvalidTestSuite = new NJasmineInvalidTestSuite(this.TestName, exception, _position);

                    if (isOuterScopeOfSpecification)
                    {
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
                _builder.DoFixtureCleanup();
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
