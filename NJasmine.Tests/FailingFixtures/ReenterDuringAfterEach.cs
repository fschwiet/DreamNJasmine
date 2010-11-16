using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[] {
        "Test Error : NJasmineTests.FailingFixtures.ReenterDuringAfterEach", 
        "System.InvalidOperationException : Called it() within afterEach()."})]
    public class ReenterDuringAfterEach : NJasmineFixture
    {
        public override void Tests()
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
