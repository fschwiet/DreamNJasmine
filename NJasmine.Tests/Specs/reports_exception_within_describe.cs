using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    public class reports_exception_within_describe : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("outer test", delegate() { });

            describe("broken describe", delegate()
            {
                it("inner test", delegate()
                {
                });

                int j = 5;
                int i = 1 / (j - 5);
            });

            it("last test", delegate() { });
        }

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("NJasmineTests.Specs.reports_exception_within_describe, broken describe").thatFails()
                .withFailureMessage("Attempted to divide by zero.");
        }
    }
}