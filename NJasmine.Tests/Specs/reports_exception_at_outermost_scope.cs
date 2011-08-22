using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class reports_exception_at_outermost_scope : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("outer test", delegate() { });

            describe("broken describe", delegate()
            {
                it("inner test", delegate()
                {
                });
            });

            it("last test", delegate() { });

            int j = 5;
            int i = 1 / (j - 5);
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTestWithFullName("NJasmineTests.Specs.reports_exception_at_outermost_scope").thatFails()
                .withFailureMessage("Attempted to divide by zero.");
        }
    }
}