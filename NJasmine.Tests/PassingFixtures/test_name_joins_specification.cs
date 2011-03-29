using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.PassingFixtures
{
    [Explicit]
    [RunExternal(true, ExpectedStrings = new string[]
    {
        @"NJasmineTests.FailingFixtures.test_name_joins_specification, simple test",
        @"NJasmineTests.FailingFixtures.test_name_joins_specification, simple describe, simple test",
    })]
    public class test_name_joins_specification : NJasmineFixture
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
