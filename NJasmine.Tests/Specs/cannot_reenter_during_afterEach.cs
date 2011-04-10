using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit, RunExternal(false, ExpectedStrings = new[] {
        "Test Error : NJasmineTests.Specs.cannot_reenter_during_afterEach", 
        "System.InvalidOperationException : Called it() within afterEach()."})]
    public class cannot_reenter_during_afterEach : GivenWhenThenFixture
    {
        public override void Specify()
        {
            afterEach(delegate()
            {
                it("nested it() is not allowed", delegate() { });
            });

            it("has a valid test", delegate()
            {
            });
        }
    }
}
