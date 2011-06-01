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
        public TestPosition Position { get; private set; }

        private GlobalSetupManager _setupManager;

        public static Test CreateRootNJasmineSuite(Func<SpecificationFixture> fixtureFactory, Type type)
        {
            AllSuitesBuildContext buildContext = new AllSuitesBuildContext(fixtureFactory, new NameGenerator(), fixtureFactory());

            var globalSetup = new GlobalSetupManager();

            globalSetup.Initialize(fixtureFactory());

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(new TestPosition(), globalSetup);
            rootSuite.TestName.FullName = type.Namespace + "." + type.Name;
            rootSuite.TestName.Name = type.Name;

            return rootSuite.BuildNJasmineTestSuite(buildContext, globalSetup, buildContext._fixtureInstanceForDiscovery.Run, true);
        }

        public NJasmineTestSuite(TestPosition position, GlobalSetupManager setupManager)
            : base("thistestname", "willbeoverwritten")
        {
            Position = position;
            _setupManager = setupManager;
            maintainTestOrder = true;
        }

        public Test BuildNJasmineTestSuite(AllSuitesBuildContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification)
        {
            var builder = new NJasmineTestSuiteBuilder(this, buildContext, globalSetup);
            
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
                _setupManager.Cleanup();
            }
            catch (Exception innerException)
            {
                NUnitException exception2 = innerException as NUnitException;
                if (exception2 != null)
                {
                    innerException = exception2.InnerException;
                }

                TestResultUtil.Error(suiteResult, innerException, FailureSite.TearDown);
            }
        }
    }
}
