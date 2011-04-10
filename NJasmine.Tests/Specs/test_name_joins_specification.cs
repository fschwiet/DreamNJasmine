using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    [RunExternal(true, ExpectedStrings = new string[]
    {
        @"NJasmineTests.Specs.test_name_joins_specification, simple test",
        @"NJasmineTests.Specs.test_name_joins_specification, simple describe, simple test",
    })]
    public class test_name_joins_specification : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("simple test");
            
            describe("simple describe", delegate
            {
                it("simple test");
            });
        }
    }
}
