using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;

namespace NJasmineTests.Specs.expectations
{
    public class should_ignore_expectations_after_test : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("has a passing test", () => {});

            expect(() => false);
            expectEventually(() => false);

            it("has a failing test", () => {});
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.hasTest("has a passing test").thatSucceeds();
            fixtureResult.hasTest("has a failing test").thatErrors();
        }
    }
}
