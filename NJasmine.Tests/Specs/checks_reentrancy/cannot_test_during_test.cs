using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.checks_reentrancy
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

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("outer test").thatFailsInAnUnspecifiedManner()
                .withFailureMessage("System.InvalidOperationException : Called it() within it().");
        }
    }
}
