using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.Specs
{
    [Explicit, RunExternal(false, ExpectedStrings = new[]{
        "Test Error : NJasmineTests.Specs.cannot_reenter_during_it",
        "System.InvalidOperationException : Called it() within it()."})]
    public class cannot_reenter_during_it : GivenWhenThenFixture
    {
        public override void Specify()
        {
            it("outer test", delegate()
            {
                it("inner test", delegate() { });
            });
        }
    }
}
