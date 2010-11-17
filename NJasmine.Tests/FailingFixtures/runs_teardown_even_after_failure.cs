using System;
using NJasmineTests.Core;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{

    [Explicit, RunExternal(false, ExpectedExtraction = "failure_in_describe_doesnt_prevent_cleanup")]
    public class runs_teardown_even_after_failure : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            TraceReset();

            afterEach(delegate()
            {
                Trace("failure_in_describe_doesnt_prevent_cleanup");
            });

            describe("failng describe", delegate()
            {
                it("failing test", delegate()
                {
                    throw new Exception("intended test failure");
                });
            });
        }
    }
}