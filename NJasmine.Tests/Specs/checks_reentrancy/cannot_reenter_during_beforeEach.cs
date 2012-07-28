using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.checks_reentrancy
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

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("has a valid test").thatErrors()
                .withFailureMessage("System.InvalidOperationException : Called it() within beforeEach().");
        }
    }
}
