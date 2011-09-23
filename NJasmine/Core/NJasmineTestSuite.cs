using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NJasmine.Core.Discovery;
using NJasmine.Core.FixtureVisitor;
using NJasmine.Core.GlobalSetup;
using NUnit.Core;
using NUnit.Framework;

namespace NJasmine.Core
{
    class NJasmineTestSuite : TestSuite, INJasmineTest
    {
        public TestPosition Position { get { return _position; } }

        readonly TestPosition _position;
        GlobalSetupManager _setupManager;

        public static Test CreateRootNJasmineSuite(Type type)
        {
            var fixtureFactory = GetFixtureFactoryForType(type);

            AllSuitesBuildContext buildContext = new AllSuitesBuildContext(fixtureFactory, new NameGenerator(), fixtureFactory());

            var globalSetup = new GlobalSetupManager();

            globalSetup.Initialize(fixtureFactory);

            NJasmineTestSuite rootSuite = new NJasmineTestSuite(new TestPosition(), globalSetup);
            rootSuite.TestName.FullName = type.Namespace + "." + type.Name;
            rootSuite.TestName.Name = type.Name;

            return rootSuite.BuildNJasmineTestSuite(buildContext, globalSetup, buildContext._fixtureInstanceForDiscovery.Run, true);
        }

        public NJasmineTestSuite(TestPosition position, GlobalSetupManager setupManager)
            : base("thistestname", "willbeoverwritten")
        {
            _position = position;
            _setupManager = setupManager;
            maintainTestOrder = true;
        }

        public Test BuildNJasmineTestSuite(AllSuitesBuildContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification)
        {
            List<Test> accumulatedTests = new List<Test>();

            var builder = new NJasmineTestSuiteBuilder(this, buildContext, globalSetup, test => accumulatedTests.Add(test));
            
            Exception exception = null;

            var originalVisitor = buildContext._fixtureInstanceForDiscovery.Visitor;

            buildContext._fixtureInstanceForDiscovery.CurrentPosition = _position.GetFirstChildPosition();
            buildContext._fixtureInstanceForDiscovery.Visitor = builder;

            try
            {
                try
                {
                    action();

                    buildContext.RunPendingDiscoveryBranches(action);
                }
                catch (Exception e)
                {
                    exception = e;
                }

                if (exception == null)
                {
                    foreach (var test in accumulatedTests)
                        Add(test);
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
                _setupManager.Cleanup(Position);
            }
            catch (Exception innerException)
            {
                NUnitException exception2 = innerException as NUnitException;
                if (exception2 != null)
                {
                    innerException = exception2.InnerException;
                }

                TestResultUtil.Error(suiteResult, innerException, null, FailureSite.TearDown);
            }
        }

        public static Func<SpecificationFixture> GetFixtureFactoryForType(Type type)
        {
            var constructor = type.GetConstructor(new Type[0]);

            return delegate()
            {
                var fixture = constructor.Invoke(new object[0]) as SpecificationFixture;
                return fixture;
            };
        }
    }
}
