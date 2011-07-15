using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class cannot_reenter_during_it : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("outer test", delegate()
            {
                it("inner test", delegate() { });
            });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("NJasmineTests.Specs.cannot_reenter_during_it, outer test").thatFailsInAnUnspecifiedManner()
                .withFailureMessage("System.InvalidOperationException : Called it() within it().");
        }
    }
}
