using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit]
    [RunExternal(false, 
        ExpectedStrings = new string[] {
                "Test Error : NJasmineTests.Specs.cannot_reenter_during_beforeEach",
                "System.InvalidOperationException : Called it() within beforeEach()."})]
    public class cannot_reenter_during_beforeEach : GivenWhenThenFixture
    {
        public override void Specify()
        {
            beforeEach(delegate()
            {
                it("nested it() is not allowed", delegate() { });
            });

            it("has a valid test", delegate()
            {
            });
        }
    }
}
