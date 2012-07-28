using System;
using NJasmine;
using NJasmine.Marshalled;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    public class specification_text_becomes_test_name : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("simple test", delegate {});
            
            describe("simple describe", delegate
            {
                it("simple test", delegate {});
            });
        }

        public void Verify_NJasmine_implementation(IFixtureResult fixtureResult)
        {
            fixtureResult.succeeds();

            fixtureResult.hasTest("simple test");
            fixtureResult.hasTest("simple describe, simple test");
        }
    }
}
