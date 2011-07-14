using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class stacktrace_has_NJasmine_internal_calls_removed : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            given("some context", delegate
            {
                when("some action", delegate
                {
                    then("it fails", delegate()
                    {
                        expect(() => 1 + 2 == 4);
                    });
                });
            });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasStackTracesThat(s => !s.Contains("NJasmine.Core"));
        }
    }
}
