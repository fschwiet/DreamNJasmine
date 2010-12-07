using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit]
    [RunExternal(false, 
        ExpectedStrings = new string[] {
                "Test Error : NJasmineTests.FailingFixtures.cannot_reenter_during_beforeEach",
                "System.InvalidOperationException : Called it() within beforeEach()."})]
    public class cannot_reenter_during_beforeEach : NJasmineFixture
    {
        public override void Tests()
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
