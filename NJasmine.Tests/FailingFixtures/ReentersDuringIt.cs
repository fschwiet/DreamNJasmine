using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[]{
        "Test Error : NJasmineTests.FailingFixtures.ReentersDuringIt",
        "System.InvalidOperationException : Called it() within it()."})]
    public class ReentersDuringIt : NJasmineFixture
    {
        public override void Tests()
        {
            it("outer test", delegate()
            {
                it("inner test", delegate() { });
            });
        }
    }
}
