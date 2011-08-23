using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.expectations
{
    [Explicit]
    public class can_check_that_an_arbtirary_condition_is_true : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("passes the test if the condition passes", delegate
            {
                expect(() => true);
            });

            it("fails the test if if the condition fails", delegate
            {
                expect(() => false);
            });

            describe("expections can be set during discovery", delegate
            {
                expect(() => true);
                expect(() => false);

                it("fails this test", delegate
                {
                });
            });
        }

        public void Verify_NJasmine_implementation(FixtureResult fixtureResult)
        {
            fixtureResult.failed();

            fixtureResult.hasTest("passes the test if the condition passes").thatSucceeds();
            fixtureResult.hasTest("fails the test if if the condition fails").thatErrors();
            fixtureResult.hasTest("expections can be set during discovery, fails this test").thatErrors();
        }
    }
}
