using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using NJasmine.Core.Discovery;
using NJasmine.Core.GlobalSetup;

namespace NJasmine.Core
{
    public class NJasmineTestSuite
    {
        private readonly TestPosition _position;
        private GlobalSetupManager _globalSetup;

        public NJasmineTestSuite(TestPosition position, GlobalSetupManager globalSetup)
        {
            _position = position;
            _globalSetup = globalSetup;
        }

        public NJasmineBuildResult BuildNJasmineTestSuite(string parentName, string name, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification)
        {
            var resultSuite = new NJasmineTestSuiteNUnit(parentName, name, p => _globalSetup.Cleanup(p), _position);

            var resultBuilder = new NJasmineBuildResult(resultSuite);

            RunSuiteAction(buildContext, globalSetup, action, isOuterScopeOfSpecification, resultBuilder);

            return resultBuilder;
        }

        public void RunSuiteAction(FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action,
                                    bool isOuterScopeOfSpecification, NJasmineBuildResult resultBuilder)
        {
            var builder = new NJasmineTestSuiteBuilder(this, resultBuilder, buildContext, globalSetup);

            var exception = buildContext.RunActionWithVisitor(_position.GetFirstChildPosition(), action, builder);

            if (exception == null)
            {
                builder.VisitAccumulatedTests(v => resultBuilder.AddChildTest(v));
            }
            else
            {
                if (isOuterScopeOfSpecification)
                {
                    var subfailure = new NJasmineBuildResult(new NJasmineInvalidTestSuite(exception, _position));

                    subfailure.FullName = resultBuilder.FullName;
                    subfailure.Shortname = resultBuilder.Shortname;
                    subfailure.MultilineName = resultBuilder.MultilineName;

                    resultBuilder.AddChildTest(subfailure);
                }
                else
                {
                    resultBuilder.ReplaceNUnitResult(new NJasmineInvalidTestSuite(exception, _position));
                }
            }
        }
    }
}
