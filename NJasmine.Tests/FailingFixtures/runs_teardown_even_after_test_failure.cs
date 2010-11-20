using System;
using NJasmine;
using NJasmineTests.Core;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedExtraction = @"
failure_in_test_doesnt_prevent_cleanup_in_same_scope
failure_in_test_doesnt_prevent_cleanup")]

    public class runs_teardown_even_after_test_failure : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            TraceReset();

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
    }
}