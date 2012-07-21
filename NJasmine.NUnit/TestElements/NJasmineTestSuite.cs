using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.NUnit.TestElements
{
    public class NJasmineTestSuite
    {
        private readonly TestPosition _position;
        private GlobalSetupManager _globalSetup;
        readonly FixtureDiscoveryContext _discoveryContext;

        public NJasmineTestSuite(TestPosition position, GlobalSetupManager globalSetup, FixtureDiscoveryContext discoveryContext)
        {
            _position = position;
            _globalSetup = globalSetup;
            _discoveryContext = discoveryContext;
        }

        public TestBuilder BuildNJasmineTestSuite(string parentName, string name, Action action, bool isOuterScopeOfSpecification)
        {
            var testName = new TestName
            {
                FullName = parentName + "." + name,
                Shortname = name,
                MultilineName = parentName + "." + name
            };

            var resultBuilder = new TestBuilder(_discoveryContext.NativeTestFactory.ForSuite(testName, _position, () => _globalSetup.Cleanup(_position)), testName);

            return RunSuiteAction(action, isOuterScopeOfSpecification, resultBuilder);
        }

        public TestBuilder RunSuiteAction(Action action, bool isOuterScopeOfSpecification, TestBuilder resultBuilder)
        {
            var builder = new NJasmineTestSuiteBuilder(_discoveryContext.NativeTestFactory, resultBuilder, _discoveryContext, _globalSetup);

            var exception = _discoveryContext.RunActionWithVisitor(_position.GetFirstChildPosition(), action, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(v => resultBuilder.AddChildTest(v));
            }
            else
            {
                var failingSuiteName = _discoveryContext.NameReservations.GetReservedNameLike(resultBuilder.Name);

                var failingSuiteAsTest = new TestBuilder(_discoveryContext.NativeTestFactory.ForFailingSuite(failingSuiteName, _position, exception), failingSuiteName);

                if (isOuterScopeOfSpecification)
                {
                    resultBuilder.AddChildTest(failingSuiteAsTest);
                }
                else
                {
                    return failingSuiteAsTest;
                }
            }
            return resultBuilder;
        }
    }
}
