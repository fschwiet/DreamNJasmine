using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[]{
        "Test Error : NJasmineTests.FailingFixtures.cannot_reenter_during_it",
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
