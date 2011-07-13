using System;
using NJasmine;
using NJasmineTests.Export;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    public class test_name_joins_specification : GivenWhenThenFixture, INJasmineInternalRequirement
    {
        public override void Specify()
        {
            it("simple test", delegate {});
            
            describe("simple describe", delegate
            {
                it("simple test", delegate {});
            });
        }

        public void Verify(TestResult testResult)
        {
            testResult.succeeds();

            testResult.hasTest("NJasmineTests.Specs.test_name_joins_specification, simple test");
            testResult.hasTest("NJasmineTests.Specs.test_name_joins_specification, simple describe, simple test");
        }
    }
}
