using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[] {
        "Test Error : NJasmineTests.FailingFixtures.cannot_reenter_during_afterEach", 
        "System.InvalidOperationException : Called it() within afterEach()."})]
    public class cannot_reenter_during_afterEach : NJasmineFixture
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
