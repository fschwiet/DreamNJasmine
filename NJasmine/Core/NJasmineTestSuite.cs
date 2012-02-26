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

        public INJasmineBuildResult BuildNJasmineTestSuite(string parentName, string name, FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action, bool isOuterScopeOfSpecification)
        {
            var position = _position;

            var resultBuilder = NJasmineBuildResult.ForSuite(position, () => _globalSetup.Cleanup(position));
            resultBuilder.FullName = parentName + "." + name;
            resultBuilder.Shortname = name;
            resultBuilder.MultilineName = resultBuilder.FullName;

            RunSuiteAction(buildContext, globalSetup, action, isOuterScopeOfSpecification, resultBuilder);

            return resultBuilder;
        }

        public void RunSuiteAction(FixtureDiscoveryContext buildContext, GlobalSetupManager globalSetup, Action action,
                                    bool isOuterScopeOfSpecification, INJasmineBuildResult resultBuilder)
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
                    var subfailure = NJasmineBuildResult.ForSuite(_position, () => { });
                    subfailure.TurnIntoAFailingSuite(exception);

                    subfailure.FullName = resultBuilder.FullName;
                    subfailure.Shortname = resultBuilder.Shortname;
                    subfailure.MultilineName = resultBuilder.MultilineName;

                    resultBuilder.AddChildTest(subfailure);
                }
                else
                {
                    resultBuilder.TurnIntoAFailingSuite(exception);
                }
            }
        }
    }
}
