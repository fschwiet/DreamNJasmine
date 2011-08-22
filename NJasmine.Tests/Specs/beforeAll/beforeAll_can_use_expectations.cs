using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs.beforeAll
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

        public void Verify(FixtureResult fixtureResult)
        {
            fixtureResult.failed();
            fixtureResult.hasTest("works").thatSucceeds();
            fixtureResult.hasTest("when using expect within beforeAll, fails").thatErrors();
        }
    }
}
