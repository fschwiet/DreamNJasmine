using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace NJasmineTests.FailingFixtures
{
    [Explicit, RunExternal(false, ExpectedStrings = new[] 
            {"Test Failure : NJasmineTests.FailingFixtures.cannot_call_beforeEach_after_test", 
            "Exception thrown within test definition: Called beforeEach() after disposing()."})]
    public class cannot_call_beforeEach_after_test : NJasmineFixture
    {
        public override void Tests()
        {
            it("hi", delegate{ });

            beforeEach(delegate { });
        }
    }
}
