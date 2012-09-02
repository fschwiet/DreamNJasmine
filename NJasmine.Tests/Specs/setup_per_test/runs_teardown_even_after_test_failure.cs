using System;
using NJasmine.Marshalled;
using NJasmineTests.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.setup_per_test
{
    [Explicit]
    public class runs_teardown_even_after_test_failure : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            afterEach(delegate()
            {
                Trace("failure_in_test_doesnt_prevent_cleanup");
            });

            describe("failng describe", delegate()
            {
                afterEach(delegate()
                {
                    Trace("failure_in_test_doesnt_prevent_cleanup_in_same_scope");
                });

                it("failing test", delegate()
                {
                    throw new Exception("intended test failure");
                });
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTrace(@"
failure_in_test_doesnt_prevent_cleanup_in_same_scope
failure_in_test_doesnt_prevent_cleanup");
        }
    }
}