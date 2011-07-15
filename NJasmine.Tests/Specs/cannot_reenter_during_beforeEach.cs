using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class cannot_reenter_during_beforeEach : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeEach(delegate()
            {
                it("nested it() is not allowed", delegate() { });
            });

            it("has a valid test", delegate()
            {
            });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("NJasmineTests.Specs.cannot_reenter_during_beforeEach").thatErrors()
                .withFailureMessage("System.InvalidOperationException : Called it() within beforeEach().");
        }
    }
}
