using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmine.Extras;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Extras
{
    [Explicit]
    public class can_check_for_expected_substrings : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("does nothing if all substrings are present", delegate
            {
                Expect.That("a b c").ContainsInOrder("a", "b", "c");
            });

            it("fails if a substring is missing", delegate
            {
                Expect.That("a b c").ContainsInOrder("d", "b", "c");
            });

            it("fails if substrings are out of order", delegate
            {
                Expect.That("a b c").ContainsInOrder("b", "a", "c");
            });
        }

        public void Verify_NJasmine_implementation(FixtureResult fixtureResult)
        {
            fixtureResult.hasTest("does nothing if all substrings are present").thatSucceeds();
            fixtureResult.hasTest("fails if a substring is missing").thatErrors();
            fixtureResult.hasTest("fails if substrings are out of order").thatErrors();
        }
    }
}
