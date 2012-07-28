using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.setup_shared_across_tests
{
    [Explicit]
    public class beforeAll_can_use_expectations : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            beforeAll(delegate
            {
                expect(() => true);
            });

            it("works", delegate
            {

            });

            when("using expect within beforeAll", delegate
            {
                beforeAll(delegate
                {
                    expect(() => false);
                });

                it("fails", delegate
                {

                });
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.failed();
            fixtureResult.hasTest("works").thatSucceeds();
            fixtureResult.hasTest("when using expect within beforeAll, fails").thatErrors();
        }
    }
}
