using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NJasmineTests.Integration;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit]
    [RunExternal(false, 
        ExpectedStrings = new string[] {
                "Test Error : NJasmineTests.FailingFixtures.ReentersDuringBeforeEach",
                "System.InvalidOperationException : Called it() within beforeEach()."})]
    public class ReentersDuringBeforeEach : NJasmineFixture
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
