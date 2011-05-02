using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;
using NUnit.Core;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest
    {
        private PerFixtureSetupContext _perFixtureSetupContext;

        public static Test CreateRootNJasmineSuite(Func<SpecificationFixture> fixtureFactory, Type type)
        {
            AllSuitesBuildContext buildContext = new AllSuitesBuildContext(fixtureFactory, new NameGenerator(), fixtureFactory());

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(new TestPosition());
            rootSuite.TestName.FullName = type.Namespace + "." + type.Name;
            rootSuite.TestName.Name = type.Name;

            return rootSuite.BuildNJasmineTestSuite(buildContext, new PerFixtureSetupContext(), buildContext._fixtureInstanceForDiscovery.Run, true);
        }

        public NJasmineTestSuite(TestPosition position)
            : base("thistestname", "willbeoverwritten")
        {
            Position = position;
            maintainTestOrder = true;
        }

        public TestPosition Position { get; private set; }

        public Test BuildNJasmineTestSuite(AllSuitesBuildContext buildContext, PerFixtureSetupContext fixtureSetupContext, Action action, bool isOuterScopeOfSpecification)
        {
            if (_perFixtureSetupContext != null)
                throw new NotSupportedException();

            _perFixtureSetupContext = new PerFixtureSetupContext(fixtureSetupContext);

            var builder = new NJasmineTestSuiteBuilder(this, buildContext, _perFixtureSetupContext);
            
            Exception exception = null;


            var originalVisitor = buildContext._fixtureInstanceForDiscovery.Visitor;

            buildContext._fixtureInstanceForDiscovery.CurrentPosition = Position;
            buildContext._fixtureInstanceForDiscovery.CurrentPosition = buildContext._fixtureInstanceForDiscovery.CurrentPosition.GetFirstChildPosition();
            buildContext._fixtureInstanceForDiscovery.Visitor = builder;

            try
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
                    builder.VisitAccumulatedTests(Add);
                }
                else
                {
                    var nJasmineInvalidTestSuite = new NJasmineInvalidTestSuite(exception, Position);

                    nJasmineInvalidTestSuite.TestName.FullName = TestName.FullName;
                    nJasmineInvalidTestSuite.TestName.Name = TestName.Name;

                    nJasmineInvalidTestSuite.SetMultilineName(this.GetMultilineName());

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
            finally
            {
                buildContext._fixtureInstanceForDiscovery.Visitor = originalVisitor;
            }

            return this;
        }

        protected override void DoOneTimeTearDown(TestResult suiteResult)
        {
            try
            {
                _perFixtureSetupContext.DoAllCleanup();
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
