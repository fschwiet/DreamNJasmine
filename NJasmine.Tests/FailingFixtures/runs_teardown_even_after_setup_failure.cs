using System;
using NJasmine;
using NJasmineTests.Core;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedExtraction = @"
failure_in_setup_doesnt_prevent_cleanup_in_same_scope
failure_in_setup_doesnt_prevent_cleanup")]
    public class runs_teardown_even_after_setup_failure : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            importNUnit<PerClassTraceResetFixture>();

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
    }
}