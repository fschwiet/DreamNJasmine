using System;
using NJasmineTests.Core;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{

    [Explicit, RunExternal(false, ExpectedExtraction = "this_will_be_seen")]
    public class skips_outer_teardown_on_outer_failure : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            TraceReset();

            afterEach(delegate()
            {
                Trace("this_shouldnt_be_seen");
            });

            it("failing test", delegate()
            {
                Trace("this_will_be_seen");

                throw new Exception("intended test failure");
            });
        }
    }
}