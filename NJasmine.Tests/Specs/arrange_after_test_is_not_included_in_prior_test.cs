using System;
using NJasmineTests.Core;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    public class arrange_after_test_is_not_included_in_prior_test : GivenWhenThenFixtureTracingToConsole, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeAll(ResetTracing);

            arrange(() => Trace("First setup"));
            afterEach(() => Trace("First cleanup"));

            it("runs a test", () => Trace("First test"));

            arrange(() => Trace("Second setup"));
            afterEach(() => Trace("Second cleanup"));

            it("runs another test", () => Trace("Second test"));
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.succeeds();
            fixtureResult.containsTrace(@"
First setup
First test
First cleanup
First setup
Second setup
Second test
Second cleanup
First cleanup
");
        }
    }
}
