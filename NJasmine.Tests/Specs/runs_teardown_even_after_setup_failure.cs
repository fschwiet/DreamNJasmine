using System;
using NJasmineTests.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class runs_teardown_even_after_setup_failure : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            afterEach(delegate()
            {
                Trace("failure_in_setup_doesnt_prevent_cleanup");
            });

            describe("failng describe", delegate()
            {
                afterEach(delegate()
                {
                    Trace("failure_in_setup_doesnt_prevent_cleanup_in_same_scope");
                });

                beforeEach(delegate
                {
                    throw new Exception("Intentionally failing during setup");
                });

                afterEach(delegate()
                {
                    Trace("failure_in_setup_does_prevent_cleanup_defined_after_failure");
                });

                it("some test", delegate()
                {
                });
            });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();
            fixtureResult.containsTrace(@"
failure_in_setup_doesnt_prevent_cleanup_in_same_scope
failure_in_setup_doesnt_prevent_cleanup");
        }
    }
}