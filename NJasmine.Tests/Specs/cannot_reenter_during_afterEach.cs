using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class cannot_reenter_during_afterEach : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        [Explicit]
        public override void Specify()
        {
            afterEach(delegate()
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

            fixtureResult.hasTest("NJasmineTests.Specs.cannot_reenter_during_afterEach")
                .thatErrors()
                .withMessage("System.InvalidOperationException : Called it() within afterEach().");
        }
    }
}
