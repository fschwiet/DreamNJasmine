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
            "Called beforeEach() after test started, within a call to it()."})]
    public class cannot_call_beforeEach_after_test : NJasmineFixture
    {
        public override void Specify()
        {
            it("hi", delegate{ });

            beforeEach(delegate { });
        }
    }
}
