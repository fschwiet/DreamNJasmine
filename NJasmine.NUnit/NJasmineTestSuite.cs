using System;
using NJasmine.Core;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.NUnit
{
    public class NJasmineTestSuite
    {
        readonly INativeTestFactory _nativeTestFactory;
        private readonly TestPosition _position;
        private GlobalSetupManager _globalSetup;

        public NJasmineTestSuite(INativeTestFactory nativeTestFactory, TestPosition position, GlobalSetupManager globalSetup)
        {
            _nativeTestFactory = nativeTestFactory;
            _position = position;
            _globalSetup = globalSetup;
        }

        public TestBuilder BuildNJasmineTestSuite(string parentName, string name, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification)
        {
            var position = _position;

            var resultBuilder = new TestBuilder(_nativeTestFactory.ForSuite(position, () => _globalSetup.Cleanup(position)));
            resultBuilder.FullName = parentName + "." + name;
            resultBuilder.Shortname = name;
            resultBuilder.MultilineName = resultBuilder.FullName;

            return RunSuiteAction(buildContext, globalSetup, action, isOuterScopeOfSpecification, resultBuilder);
        }

        public TestBuilder RunSuiteAction(FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action,
                                    bool isOuterScopeOfSpecification, TestBuilder resultBuilder)
        {
            var builder = new NJasmineTestSuiteBuilder(_nativeTestFactory, resultBuilder, buildContext, globalSetup);

            var exception = buildContext.RunActionWithVisitor(_position.GetFirstChildPosition(), action, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(v => resultBuilder.AddChildTest(v));
            }
            else
            {
                var failingSuiteAsTest = new TestBuilder(_nativeTestFactory.ForFailingSuite(_position, exception));

                failingSuiteAsTest.FullName = resultBuilder.FullName;
                failingSuiteAsTest.Shortname = resultBuilder.Shortname;
                failingSuiteAsTest.MultilineName = resultBuilder.MultilineName;

                buildContext.NameGenator.ReserveName(failingSuiteAsTest);

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
