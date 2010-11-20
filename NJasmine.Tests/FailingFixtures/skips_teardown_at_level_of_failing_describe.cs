using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Core;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedExtraction = @"
beforeEach
cleanup in outer scope still runs
")]
    public class skips_teardown_at_level_of_failing_describe : TraceableNJasmineFixture
    {
        public override void Tests()
        {
            TraceReset();

            afterEach(delegate()
            {
                Trace("cleanup in outer scope still runs");
            });

            describe("scope of failure", delegate()
            {
                beforeEach(delegate()
                {
                    Trace("beforeEach");
                });

                afterEach(delegate()
                {
                    Trace("afterEach_shouldnt_run");
                });

                it("failing test", delegate()
                {
                    throw new Exception("Intended test failure.");
                });
            });

        }
    }
}
